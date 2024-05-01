using System.ComponentModel;
using System.Text.RegularExpressions;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class ConnectPageVieweModel : ObservableObject
{
    public ConnectPageVieweModel(WcpClient client)
    {
        _client = client;

        if (App.Config.AutoConnect && App.Config.Address is not null)
        {
            Address = App.Config.Address;
            Port = App.Config.Port.ToString();
        }
    }

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private string? _port;

    [ObservableProperty]
    private bool _incorrectAddress;

    [ObservableProperty]
    private bool _incorectPort;

    [ObservableProperty]
    private bool _working;

    [ObservableProperty]
    private bool _interact = true;

    [ObservableProperty]
    private bool _connectionFailed;

    [ObservableProperty]
    private bool _rememberData;

    [ObservableProperty]
    private bool _autoConnect;

    private int _validPort;

    private WcpClient _client;


    [RelayCommand]
    public async Task Exit()
    {
        SetWorking(true);
        await Shell.Current.Navigation.PopAsync();
        SetWorking(false);
    }

    [RelayCommand]
    private async Task Continue()
    {
        try
        {
            if (IncorrectAddress || IncorectPort)
            {
                SetFailed(true);
                return;
            }
            SetFailed(false);
            SetWorking(true);
            if (await _client.Connect(Address!, _validPort))
            {
                if (RememberData)
                {
                    App.Config.AutoConnect = AutoConnect;
                    App.Config.Port = _validPort;
                    App.Config.Address = Address;
                    App.Config.SaveOrUpdate();
                }
                await Exit();
            }
            else
                SetFailed(true);
        }
        finally
        {
            SetWorking(false);
        }
    }

    private void SetWorking(bool working)
    {
        Shell.Current.Dispatcher.Dispatch(() =>
        {
            Interact = !working;
            Working = working;
        });
    }

    private void SetFailed(bool failed)
    {
        Shell.Current.Dispatcher.Dispatch(() =>
        {
            ConnectionFailed = failed;
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
        else if (e.PropertyName == nameof(Port))
        {
            IncorectPort = !int.TryParse(Port, out _validPort) || _validPort == 0 || _validPort > ushort.MaxValue;
        }
        base.OnPropertyChanged(e);
    }
}
