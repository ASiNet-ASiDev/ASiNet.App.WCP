using System.Net.Sockets;
using ASiNet.Data.Serialization;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Core.Primitives;

namespace ASiNet.WCP.Core;
public class WcpClient : IDisposable
{
    public event Action<bool>? ConnectedStatusChandged;

    public bool Connected => _client?.Connected ?? false;

    private TcpClient? _client;
    private NetworkStream? _stream;

    private List<Package> _buffer = [];

    private MediaManager _mediaManager = new();

    private readonly object _lock = new();

    public async Task<bool> Connect(string address, int port)
    {
        return await Task.Run(() =>
        {
            try
            {
                lock (_lock)
                {
                    _client = new TcpClient(address, port);
                    _stream = _client.GetStream();

                    ConnectedStatusChandged?.Invoke(_client?.Connected ?? false);
                    return _client?.Connected ?? false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        });
    }

    public async Task<bool> Reconnect(string address, int port)
    {
        try
        {
            await Disconnect();
            return await Connect(address, port);
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
            if (_client?.Connected ?? false)
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

    public async Task<(string[]? Dirs, GetDirectiryStatus Status)> GetDirectories(string? root)
    {
        try
        {
            var response = await SendAndAccept<GetRemoteDirectoryRequest, GetRemoteDirectoryResponse>(new GetRemoteDirectoryRequest() { RootPath = root });
            if(response is null)
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

    public async Task<MediaClient?> OpenMediaStream(string remotePath, string localPath, MediaAction action)
    {
        try
        {
            var request = new MediaStreamRequest()
            { 
                Action = action switch
                { 
                    MediaAction.Get => MediaAction.Post,
                    MediaAction.Post => MediaAction.Get,
                    _ => throw new NotImplementedException(),
                },
                Path = remotePath,
            };
            var response = await SendAndAccept<MediaStreamRequest, MediaStreamResponse>(request);
            if(response is null)
                return null;
            if(response.Status == MediaStreamStatus.Ok)
            {
                return _mediaManager.RunNew(response.Address!, response.Port, localPath, action);
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }


    public IEnumerable<MediaTask> RefreshMediaTasks() => 
        _mediaManager.Refresh();
    public IEnumerable<MediaTask> GetMediaTasks() =>
        _mediaManager.GetMediaTasks();

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

    public void Dispose()
    {
        _client?.Dispose();
        _mediaManager?.Dispose();
    }
}
