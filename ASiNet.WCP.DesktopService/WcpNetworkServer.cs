using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Interfaces;
using ASiNet.WCP.Core;
using ASiNet.WCP.DesktopService;
using ASiNet.WCP.WinApi;

namespace ASiNet.WCP.Server;
public class WcpNetworkServer
{
    public WcpNetworkServer()
    {
        _config = ServerConfig.LoadOrEmpty();
        _listener = new(IPAddress.Any, _config.Port);
    }
    
    private TcpListener _listener;

    private IVirtualKeyboardLayout _keyboardLayout = new KeyboardLayout();
    private IVirtualKeyboard _virtualKeyboard = new WindowsKeyboard();
    private IVirtualMouse _virtualMouse = new WindowsMouse();

    private ServerConfig _config;


    public void Start()
    {
        _keyboardLayout.LoadLanguage(LanguageCode.RussianRU);
        _keyboardLayout.LoadLanguage(LanguageCode.EnglishUS);
        _listener.Start();
    }

    public void Stop()
    {
        _listener.Stop();
    }

    public async Task Updater(CancellationToken token)
    {
        await Task.Run(() =>
        {
            ServerClient? client = null;
            while (!token.IsCancellationRequested)
            {
                if (client is null || !client.Connected)
                {
                    var tcpclient = WaitClient(token);
                    if (tcpclient is null)
                        continue;
                    client?.Dispose();
                    client = new(tcpclient, _virtualKeyboard, _virtualMouse, _keyboardLayout, _config);      
                }
                else
                    if (!client.Update())
                    Task.Delay(10).Wait();
            }
        });
    }


    private TcpClient? WaitClient(CancellationToken token)
    {
        try
        {
            var task = _listener.AcceptTcpClientAsync(token);
            while (!task.IsCompleted)
            {
                Task.Delay(500, token).Wait(token);
            }
            return task.Result;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
