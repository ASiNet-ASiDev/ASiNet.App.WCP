using System.Net.Sockets;
using ASiNet.Data.Serialization;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Core.Primitives;

namespace ASiNet.WCP.Core;
public class WcpClient : IDisposable
{

    public WcpClient()
    {
        _mediaManager = new(this);
    }
    public event Action<bool>? ConnectedStatusChandged;

    public MediaManager MediaManager => _mediaManager;

    public bool Connected => IsConnectedCheck();

    public int ProtocolVersion => WCPProtocolVersion.VERSION;

    private int _lastPort;

    private string? _lastAddress;

    private TcpClient? _client;
    private NetworkStream? _stream;

    private List<Package> _buffer = [];

    private MediaManager _mediaManager;

    private readonly object _lock = new();

    public async Task<bool> Connect(string address, int port)
    {
        try
        {
            if(Connected)
                return true;
            lock (_lock)
            {
                _client = new TcpClient();
            }
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            await _client.ConnectAsync(address, port, cts.Token);
            _stream = _client.GetStream();

            _lastAddress = address;
            _lastPort = port;
            ConnectedStatusChandged?.Invoke(_client?.Connected ?? false);
            return _client?.Connected ?? false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> Reconnect()
    {
        try
        {
            if(_lastPort == 0 || _lastAddress is null)
                return false;
            await Disconnect();
            return await Connect(_lastAddress, _lastPort);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task Disconnect()
    {
        try
        {
            if (Connected)
            {
                var request = new DisconnectionRequest();
                var response = await SendAndAccept<DisconnectionRequest, DisconectionResponce>(request);
            }

            lock (_lock)
                _client?.Dispose();
            _client = null;
            ConnectedStatusChandged?.Invoke(false);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<LanguageCode> GetLanguage()
    {
        try
        {
            var request = new GetLanguageRequest();
            var response = await SendAndAccept<GetLanguageRequest, LanguageResponse>(request);

            if (response is not null && response.Success)
            {
                return response.LanguageCode;
            }
            return LanguageCode.None;
        }
        catch (Exception)
        {
            return LanguageCode.None;
        }
    }

    public async Task<bool> SetLanguage(LanguageCode languageCode)
    {
        try
        {
            var request = new SetLanguageRequest() { LanguageCode = languageCode };
            var response = await SendAndAccept<SetLanguageRequest, LanguageResponse>(request);

            return response is not null && response.Success;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool SendKeyEvent(in KeyChandgeEvent input)
    {
        try
        {
            return Send(input);
        }
        catch
        {
            return false;
        }
    }

    public async Task<(string[]? Dirs, string[]? Files, GetDirectiryStatus Status)> GetFileSystemEntris(string? root, bool getFiles = true)
    {
        try
        {
            var response = await SendAndAccept<GetRemoteDirectoryRequest, GetRemoteDirectoryResponse>(new GetRemoteDirectoryRequest() { Root = root, GetFiles = getFiles, });
            if(response is null)
                return (null, null, GetDirectiryStatus.Failed);
            return (response.Directories, response.Files, response.Status);
        }
        catch
        {
            return (null, null, GetDirectiryStatus.Failed);
        }
    }

    public async Task<(string[]? Dirs, GetDirectiryStatus Status)> GetFileSystemRoots()
    {
        try
        {
            var response = await SendAndAccept<GetRemoteDirectoryRequest, GetRemoteDirectoryResponse>(new GetRemoteDirectoryRequest() { GetFiles = false, GetRoots = true });
            if (response is null)
                return (null, GetDirectiryStatus.Failed);
            return (response.Directories, response.Status);
        }
        catch
        {
            return (null, GetDirectiryStatus.Failed);
        }
    }


    public bool SendMouseMoveEvent(in MouseChangedEvent input)
    {
        try
        {
            return Send(input);
        }
        catch
        {
            return false;
        }
    }

    public async Task<Taccept?> SendAndAccept<Tsend, Taccept>(Tsend message) where Taccept : Package where Tsend : Package
    {
        return await Task.Run(() =>
        {
            if (!Connected)
                throw new Exception();
            if (Send(message))
            {
                return Accept<Taccept>();
            }
            return null;
        });
    }


    private bool Send<T>(T message) where T : Package
    {
        try
        {
            if(!Connected && !Reconnect().Result)
                return false;
            lock (_lock)
            {
                BinarySerializer.Serialize<Package>(message, _stream!);
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    private T? Accept<T>() where T : Package
    {
        try
        {
            lock (_lock)
            {
                var attempts = 100;
                do
                {
                    if(_buffer.Count > 0)
                    {
                        var package = _buffer.FirstOrDefault(x => x is T) as T;
                        if(package is not null)
                        {
                            _buffer.Remove(package);
                            return package;
                        }
                    }
                    if (_stream!.DataAvailable)
                    {
                        var package = BinarySerializer.Deserialize<Package>(_stream!);
                        return package as T;
                    }
                    Task.Delay(50).Wait();
                    attempts--;
                } while (attempts > 0);
                return null;
            }
        }
        catch (Exception)
        {
            return null;
        }
    }

    private bool IsConnectedCheck()
    {
        try
        {
            if (_client != null && _client.Client != null && _client.Client.Connected)
            {
                if (_client.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buff = new byte[1];
                    if (_client.Client.Receive(buff, SocketFlags.Peek) == 0)
                        return false;
                }
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
        _mediaManager?.Dispose();
    }
}
