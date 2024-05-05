using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using ASiNet.Data.Serialization;
using ASiNet.WCP.Common.Enums;

namespace ASiNet.WCP.Core;

public enum MediaClientStatus
{
    StartOk,
    StartFailed,
    FinishOk,
    FinishFailed,
    Failed,
}

public class MediaClient : IDisposable
{
    public MediaClient(string address, int port, string path, MediaAction action)
    {
        try
        {
            _action = action;
            _path = path;
            _tempPath = $"{path}{TEMP_FILE_EXE}";
            Waiting = true;
            _client = new(address, port);
            _stream = _client.GetStream();
            _cts = new();
            switch (action)
            {
                case MediaAction.Post:
                    _ = Post();
                    break;
                case MediaAction.Get:
                    _ = Get();
                    break;
            }
        }
        catch
        {
            Failed = true;
            Dispose();
        }
        finally
        {
            Waiting = false;
        }
    }

    public MediaClient(int port, string path, MediaAction action)
    {
        _action = action;
        _path = path;
        _tempPath = $"{path}{TEMP_FILE_EXE}";
        _ = Wait(port, action);
    }

    public int Port { get; set; }

    public bool Working { get; private set; }

    public bool Waiting { get; private set; }

    public bool Failed { get; private set; }

    public bool Connected { get; private set; }

    public long TotalSize { get; private set; }

    private const string TEMP_FILE_EXE = ".wcptemp";

    public long ProccessingSize
    {
        get => _proccessingSize;
        private set
        {
            _proccessingSize = value;
            Changed?.Invoke(TotalSize, value);
        }
    }

    public event Action<long, long>? Changed;
    public event Action<MediaClientStatus>? StatusChanged;

    private string _path = null!;
    private string _tempPath = null!;

    private TcpClient _client = null!;
    private NetworkStream _stream = null!;

    private CancellationTokenSource _cts = null!;

    private string? _fileHash;

    private long _proccessingSize;

    private MediaAction _action;

    private async Task Wait(int port, MediaAction action)
    {
        await Task.Run(async () =>
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(10_000);
            try
            {
                Waiting = true;
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                var client = await listener.AcceptTcpClientAsync(cts.Token);
                listener.Stop();

                _client = client;

                _stream = _client.GetStream();
                _cts = new();
                switch (action)
                {
                    case MediaAction.Post:
                        _ = Post();
                        break;
                    case MediaAction.Get:
                        _ = Get();
                        break;
                }
            }
            catch
            {
                Failed = true;
            }
            finally
            {
                Waiting = false;
            }
        });
    }

    private async Task Post()
    {
        await Task.Run(() =>
        {
            try
            {
                Working = true;
                if (!File.Exists(_path))
                {
                    Send(new MediaPackage { Operation = OperationStatus.FinishFailed });
                    StatusChanged?.Invoke(MediaClientStatus.StartFailed);
                    Failed = true;
                    return;
                }
                StatusChanged?.Invoke(MediaClientStatus.StartOk);
                using (var hashStream = File.Open(_path, FileMode.Open))
                {
                    TotalSize = hashStream.Length;
                    var hash = SHA512.HashData(hashStream);
                    Send(new MediaPackage { Operation = OperationStatus.Start, Data = hash, TotalSize = hashStream.Length });
                }
                using (var data = File.Open(_path, FileMode.Open))
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        var buffer = new byte[64000];
                        var size = data.Read(buffer);
                        ProccessingSize += size;
                        if (size == 0)
                        {
                            Send(new MediaPackage { Operation = OperationStatus.Finish });
                            break;
                        }
                        Send(new MediaPackage { Operation = OperationStatus.Fragment, Data = buffer[..size] });
                        Task.Delay(10).Wait();
                    }
                }
                var result = Accept();
                if (result.Operation != OperationStatus.FinishOk)
                {
                    Failed = true;
                    StatusChanged?.Invoke(MediaClientStatus.FinishOk);
                }
                else
                {
                    Failed = false;
                    StatusChanged?.Invoke(MediaClientStatus.FinishFailed);
                }
            }
            catch
            {
                Failed = true;
                StatusChanged?.Invoke(MediaClientStatus.Failed);
            }
            finally
            {
                Working = false;
                Dispose();
            }
        });
    }

    private async Task Get()
    {
        await Task.Run(() =>
        {
            try
            {
                Working = true;
                {
                    var start = Accept();
                    if (start.Operation != OperationStatus.Start)
                    {
                        StatusChanged?.Invoke(MediaClientStatus.StartFailed);
                        throw new Exception();
                    }
                    TotalSize = start.TotalSize ?? 0;
                    _fileHash = Convert.ToHexString(start.Data!);
                }
                StatusChanged?.Invoke(MediaClientStatus.StartOk);
                using (var data = File.Create(_tempPath))
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        var dataPack = Accept();
                        if (dataPack.Operation == OperationStatus.Finish)
                            break;
                        data.Write(dataPack.Data!);
                        ProccessingSize += dataPack.Data!.Length;
                        Task.Delay(10).Wait();
                    }
                }
                try
                {
                    using (var hashStream = File.Open(_tempPath, FileMode.Open))
                    {
                        var hash = Convert.ToHexString(SHA512.HashData(hashStream));
                        if (hash == _fileHash)
                        {
                            Send(new() { Operation = OperationStatus.FinishOk });
                            StatusChanged?.Invoke(MediaClientStatus.FinishOk);
                        }
                        else
                        {
                            Send(new() { Operation = OperationStatus.FinishFailed });
                            StatusChanged?.Invoke(MediaClientStatus.FinishFailed);
                        }
                    }
                }
                catch
                {
                    Send(new() { Operation = OperationStatus.FinishFailed });
                    StatusChanged?.Invoke(MediaClientStatus.FinishFailed);
                }
            }
            catch
            {
                Failed = true;
                StatusChanged?.Invoke(MediaClientStatus.Failed);
            }
            finally
            {
                Working = false;
                Dispose();
            }
        });
    }

    private void Send(MediaPackage package)
    {
        BinarySerializer.Serialize(package, _stream);
    }

    private MediaPackage Accept()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            if (_stream.DataAvailable)
            {
                var result = BinarySerializer.Deserialize<MediaPackage>(_stream);
                if (result is not null)
                    return result;
            }
            Task.Delay(50).Wait();
        }
        throw new OperationCanceledException();
    }

    public void Dispose()
    {
        try
        {
            _client?.Dispose();
            _stream?.Dispose();
            Changed = null;
            StatusChanged?.Invoke(Failed ? MediaClientStatus.FinishFailed : MediaClientStatus.FinishOk);
            StatusChanged = null;
            if(_action == MediaAction.Get)
            {
                if (Failed)
                {
                    File.Delete(_tempPath);
                }
                else
                {
                    File.Move(_tempPath, _path);
                }
            }
        }
        catch
        {

        }
    }
}

public class MediaPackage
{
    public long? TotalSize { get; set; }

    public OperationStatus Operation { get; set; }
    public byte[]? Data { get; set; }
}

public enum OperationStatus : byte
{
    Start,
    Fragment,
    Finish,
    FinishOk,
    FinishFailed,
}