using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ASiNet.WCP.Core;
using ASiNet.WCP.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class ConnectionPageVieweModel : ObservableObject
{
    public ConnectionPageVieweModel()
    {
        _client = ((ShellVieweModel)Shell.Current.BindingContext).WcpClient;
        _client.ConnectedStatusChandged += OnConnectedStatusChandged;

        Port = App.Config.Port.ToString();
        Address = string.Empty;
        if (App.Config.Address is not null)
        {
            Address = App.Config.Address;
        }

        _ = BacgroundAutoConnect();
    }

    public ObservableCollection<RemoteServerInfo> RemoteServers { get; } = [];

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private string _address;

    [ObservableProperty]
    private string _port;

    [ObservableProperty]
    private bool _incorrectAddress;

    [ObservableProperty]
    private bool _working;

    [ObservableProperty]
    private bool _findingServices;

    [ObservableProperty]
    private bool _showRemoteServers;

    [ObservableProperty]
    private bool _interactConnectControl = true;

    [ObservableProperty]
    private bool _interactDisconnectControl = false;

    [ObservableProperty]
    private bool _rememberData;

    [ObservableProperty]
    private bool _autoConnectRequest;

    private WcpClient _client;


    [RelayCommand]
    private async Task Disconnect()
    {
        SetWorking(true);
        await _client.Disconnect();
        SetWorking(false);
    }

    [RelayCommand]
    private async Task Connect(RemoteServerInfo? info)
    {
        try
        {
            if(info is null && IncorrectAddress)
                return;
            info ??= new RemoteServerInfo(null, App.Config.Port, Address, -1);

            SetWorking(true);
            if (await _client.Connect(info.Address, info.Port))
            {
                Address = info.Address;
                if (App.Config.Port !=  info.Port || App.Config.Address != info.Address)
                {
                    if (await App.AlertSvc.ShowConfirmationAsync(Resources.Localization.AppResources.alert_save_cnf_title, Resources.Localization.AppResources.alert_save_cnf_text))
                    {
                        App.Config.AutoConnect = true;
                        App.Config.Port = info.Port;
                        App.Config.Address = info.Address;
                        App.Config.SaveOrUpdate();
                    }
                }
            }
            else
                App.AlertSvc.ShowAlert(Resources.Localization.AppResources.alert_connection_failed_title, Resources.Localization.AppResources.alert_connection_failed_text);
        }
        catch
        {
            App.AlertSvc.ShowAlert(Resources.Localization.AppResources.alert_connection_failed_title, Resources.Localization.AppResources.alert_connection_failed_text);
        }
        finally
        {
            SetWorking(false);
        }
    }

    private async Task BacgroundAutoConnect()
    {
        try
        {
            SetWorking(true);
            if (App.Config.AutoConnect && App.Config.Address is not null)
            {
                await _client.Connect(App.Config.Address, App.Config.Port);
            }
        }
        catch
        {
            App.AlertSvc.ShowAlert(Resources.Localization.AppResources.alert_connection_failed_title, Resources.Localization.AppResources.alert_connection_failed_text);
        }
        finally
        {
            SetWorking(false);
        }
    }


    [RelayCommand]
    private void ShowServers()
    {
        ShowRemoteServers = true;
        _ = FindServer();
    }
    
    [RelayCommand]
    private void ShowConnectionElement() => ShowRemoteServers = false;
    

    [RelayCommand]
    public async Task FindServer()
    {
        if(FindingServices)
            return;
        try
        {
            FindingServices = true;
            RemoteServers.Clear();
            using var client = new MulticastClient(44516, "230.44.5.16");

            await foreach (var item in client.FindServer())
            {
                RemoteServers.Add(item);
            }
        }
        catch
        {
            App.AlertSvc.ShowAlert(Resources.Localization.AppResources.alert_find_server_failed_title, Resources.Localization.AppResources.alert_find_server_failed_text);
        }
        finally
        {
            FindingServices = false;
        }
    }

    private void SetWorking(bool working)
    {
        Shell.Current.Dispatcher.Dispatch(() =>
        {
            Working = working;

            if (working)
            {
                InteractConnectControl = false;
                InteractDisconnectControl = false;
            }
            else
            {
                if (_client.Connected)
                {
                    InteractConnectControl = false;
                    InteractDisconnectControl = true;
                }
                else
                {
                    InteractConnectControl = true;
                    InteractDisconnectControl = false;
                }
            }
        });
    }


    private void OnConnectedStatusChandged(bool obj)
    {
        Shell.Current.Dispatcher.Dispatch(() =>
        {
            IsConnected = obj;
            Port = App.Config.Port.ToString();
        });
    }


    [GeneratedRegex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")]
    public static partial Regex AddresValidation();

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Address) && Address is not null)
        {
            IncorrectAddress = !AddresValidation().IsMatch(Address);
        }
        base.OnPropertyChanged(e);
    }
}
