using System.Diagnostics.Eventing.Reader;
using System.Net.Sockets;
using ASiNet.Data.Serialization;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Interfaces;
using ASiNet.WCP.Common.Network;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Core.Primitives;

namespace ASiNet.WCP.DesktopService;
public class ServerClient(TcpClient client, IVirtualKeyboard keyboard, IVirtualMouse mouse, IVirtualKeyboardLayout layout, ServerConfig config) : IDisposable
{

    public bool Connected => _client.Connected && IsConnectedCheck();

    private IVirtualKeyboard _keyboard = keyboard;
    private IVirtualMouse _mouse = mouse;
    private IVirtualKeyboardLayout _keyboardLayout = layout;

    private ServerConfig _config = config;

    private TcpClient _client = client;

    private NetworkStream _stream = client.GetStream();

    private TransportEndPointsManadgerServer _transportEndPointsManadger = new();

    private RemoteDirectoryAccess _directoryAccess = new();

    public bool Update()
    {
        try
        {
            if (!_stream.DataAvailable)
                return false;
            var package = BinarySerializer.Deserialize<Package>(_stream);
            if (package is null)
                return false;
            if (package is KeyChandgeEvent keyEvent)
                KeyChandgedEvent(keyEvent);
            else if (package is MouseChangedEvent mouseEvent)
                MouseChandgedEvent(mouseEvent);
            else if (package is SetLanguageRequest slr)
                SetLanguageCode(slr);
            else if (package is GetLanguageRequest glr)
                GetLanguageCode(glr);
            else if(package is GetRemoteDirectoryRequest grd)
                BinarySerializer.Serialize<Package>(_directoryAccess.GetDirectory(grd), _stream);
            else if (package is TransportDataRequest tdr)
                if(_transportEndPointsManadger?.Chandge(tdr) is TransportDataResponse tdrres)
                    BinarySerializer.Serialize<Package>(tdr, _stream);
            else if (package is DisconnectionRequest dc)
                DisconectedRequest(dc);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void GetLanguageCode(GetLanguageRequest request)
    {
        var result = new LanguageResponse
        {
            Success = true,
            LanguageCode = _keyboardLayout.GetCurrentLanguage()
        };
        BinarySerializer.Serialize<Package>(result, _stream);
    }

    private void SetLanguageCode(SetLanguageRequest request)
    {
        var slr = _keyboardLayout.SetLanguage(request.LanguageCode);
        var result = new LanguageResponse
        {
            Success = slr,
            LanguageCode = LanguageCode.None
        };

        BinarySerializer.Serialize<Package>(result, _stream);
    }

    private void DisconectedRequest(DisconnectionRequest request)
    {
        var responce = new DisconectionResponce() { Success = true };
        BinarySerializer.Serialize<Package>(responce, _stream);
        Dispose();
    }

    private void KeyChandgedEvent(KeyChandgeEvent keyEvent)
    {
        if(keyEvent.IsDirect)
            _keyboard.SendKeyEventDirect(keyEvent);
        else
            _keyboard.SendKeyEvent(keyEvent);
    }

    private void MouseChandgedEvent(MouseChangedEvent mouseEvent)
    {
        _mouse.SendMouseEvent(mouseEvent);
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
        catch (Exception ex)
        {
            return false;
        }
    }

    public void Dispose()
    {
        _stream.Dispose();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
