using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using ASiNet.Data.Serialization;
using ASiNet.Data.Serialization.Attributes;
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
    public MediaClient(int id, MediaManager manager, string address, int port, string path, MediaAction action)
    {
        Id = id;
        Port = port;
        _manager = manager;
        _action = action;
        _path = path;
        _tempPath = $"{path}{TEMP_FILE_EXE}";
        _cts = new();
        try
        {
            Waiting = true;
            _client = new(address, port);
            _stream = _client.GetStream();
            
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
            _client = null!;
            _stream = null!;
            Failed = true;
            Dispose();
        }
        finally
        {
            Waiting = false;
        }
    }

    public MediaClient(TcpClient client, int port, string path, MediaAction action)
    {
        Port = port;
        _action = action;
        _path = path;
        _tempPath = $"{path}{TEMP_FILE_EXE}";
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

    public int Id { get; }

    public int Port { get; set; }

    public bool Working { get; private set; }

    public bool Waiting { get; private set; }

    public bool Failed { get; private set; }

    public bool Disposed { get; private set; }

    public bool Connected { get; private set; }

    public long TotalSize { get; private set; }

    public string FilePath => _path;

    private const string TEMP_FILE_EXE = ".wcptemp";

    public long ProccessingSize
    {
        get => _proccessingSize;
        private set
        {
            _proccessingSize = value;
            Changed?.Invoke(this, TotalSize, value);
        }
    }

    public event Action<MediaClient, long, long>? Changed;
    public event Action<MediaClient, MediaClientStatus>? StatusChanged;

    private string _path;
    private string _tempPath;

    private TcpClient _client;
    private NetworkStream _stream;

    private CancellationTokenSource _cts;

    private string? _fileHash;

    private long _proccessingSize;

    private MediaAction _action;
    private MediaManager? _manager;

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
                    StatusChanged?.Invoke(this, MediaClientStatus.StartFailed);
                    Failed = true;
                    return;
                }
                StatusChanged?.Invoke(this, MediaClientStatus.StartOk);
                using (var hashStream = File.Open(_path, FileMode.Open))
                {
                    TotalSize = hashStream.Length;
                    var hash = SHA256.HashData(hashStream);
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
                    }
                }
                var result = Accept();
                if (result.Operation != OperationStatus.FinishOk)
                {
                    Failed = true;
                    StatusChanged?.Invoke(this, MediaClientStatus.FinishOk);
                }
                else
                {
                    Failed = false;
                    StatusChanged?.Invoke(this, MediaClientStatus.FinishFailed);
                }
            }
            catch
            {
                Failed = true;
                StatusChanged?.Invoke(this, MediaClientStatus.Failed);
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
                        StatusChanged?.Invoke(this, MediaClientStatus.StartFailed);
                        throw new Exception();
                    }
                    TotalSize = start.TotalSize ?? 0;
                    _fileHash = Convert.ToHexString(start.Data!);
                }
                StatusChanged?.Invoke(this, MediaClientStatus.StartOk);
                using (var data = File.Create(_tempPath))
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        var dataPack = Accept();
                        if (dataPack.Operation == OperationStatus.Finish)
                            break;
                        data.Write(dataPack.Data!);
                        ProccessingSize += dataPack.Data!.Length;
                    }
                }
                try
                {
                    using (var hashStream = File.Open(_tempPath, FileMode.Open))
                    {
                        var hash = Convert.ToHexString(SHA256.HashData(hashStream));
                        if (hash == _fileHash)
                        {
                            Send(new() { Operation = OperationStatus.FinishOk });
                            StatusChanged?.Invoke(this, MediaClientStatus.FinishOk);
                        }
                        else
                        {
                            Send(new() { Operation = OperationStatus.FinishFailed });
                            StatusChanged?.Invoke(this, MediaClientStatus.FinishFailed);
                        }
                    }
                }
                catch
                {
                    Send(new() { Operation = OperationStatus.FinishFailed });
                    StatusChanged?.Invoke(this, MediaClientStatus.FinishFailed);
                }
            }
            catch
            {
                Failed = true;
                StatusChanged?.Invoke(this, MediaClientStatus.Failed);
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
        if(Disposed) 
            return;
        try
        {
            _client?.Dispose();
            _stream?.Dispose();
            Changed = null;
            StatusChanged?.Invoke(this, Failed ? MediaClientStatus.FinishFailed : MediaClientStatus.FinishOk);
            StatusChanged = null;
            if(_action == MediaAction.Get)
            {
                if (Failed)
                {
                    File.Delete(_tempPath);
                }
                else
                {
                    if(File.Exists(_path))
                        File.Delete(_path);
                    File.Move(_tempPath, _path);
                }
            }
        }
        catch { }
        finally
        {
            Disposed = true;
            _manager?.ClosedClient(this);
        }
    }
}

[PreGenerate]
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