using ASiNet.App.WCP.Viewe;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class ConnectionPageVieweModel : ObservableObject
{
    public ConnectionPageVieweModel()
    {
        _client = ((ShellVieweModel)Shell.Current.BindingContext).WcpClient;
        _client.ConnectedStatusChandged += OnConnectedStatusChandged;

        _ = BacgroundAutoConnect();
    }

    [ObservableProperty]
    private bool _isConnected;
    [ObservableProperty]
    private bool _autoConnect;

    [ObservableProperty]
    private string? _autoConnectAddress;
    [ObservableProperty]
    private string? _autoConnectPort;

    [ObservableProperty]
    private bool _autoConnectError;

    private WcpClient _client;

    [RelayCommand]
    private void Connect()
    {
        _ = Shell.Current.Navigation.PushModalAsync(new ConnectPage() { BindingContext = new ConnectPageVieweModel(_client) });
    }

    [RelayCommand]
    private async Task Disconnect()
    {
        await _client.Disconnect();
    }

    private async Task BacgroundAutoConnect()
    {
        try
        {
            if (App.Config.AutoConnect && App.Config.Address is not null)
            {
                Shell.Current.Dispatcher.Dispatch(() =>
                {
                    AutoConnectError = false;
                    AutoConnect = true;
                    AutoConnectAddress = App.Config.Address;
                    AutoConnectPort = App.Config.Port.ToString();
                });

                if (await _client.Connect(App.Config.Address, App.Config.Port))
                {
                    Shell.Current.Dispatcher.Dispatch(() => AutoConnectError = false);
                }
                else
                {
                    Shell.Current.Dispatcher.Dispatch(() => AutoConnectError = true);
                }
            }
        }
        finally
        {
            Shell.Current.Dispatcher.Dispatch(() =>
            {
                AutoConnect = false;
            });
        }
    }

    private void OnConnectedStatusChandged(bool obj)
    {
        Shell.Current.Dispatcher.Dispatch(() =>
        {
            IsConnected = obj;
        });
    }
}
