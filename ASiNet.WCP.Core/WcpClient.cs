using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
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

    internal bool SendTransportPackage(in TransportDataRequest input)
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
    }
}
