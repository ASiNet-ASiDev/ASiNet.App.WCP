using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.Desktop.VieweModels;
public partial class MainWindowVieweModel : ObservableObject
{
    public MainWindowVieweModel()
    {
        UpdateServiceInfo();
    }

    [ObservableProperty]
    private string? _isServiseRunText;

    [ObservableProperty]
    public string? _pcIpAddress;

    [ObservableProperty]
    private bool _autorun;

    [RelayCommand]
    private async Task RunService()
    {
        if (await ServiceContext.RunService())
        {
            App.Current.Dispatcher.Invoke(() => IsServiseRunText = "Running");
        }
    }

    [RelayCommand]
    private async Task StopService()
    {
        if (await ServiceContext.StopService())
        {
            App.Current.Dispatcher.Invoke(() => IsServiseRunText = "Stopped");
        }
    }


    [RelayCommand]
    private void UpdateServiceInfo()
    {
        try
        {
            Autorun = ServiceContext.Autorun;
            PcIpAddress = $"\n{ServiceContext.IpAddresses.Select(x => x.ToString()).FirstOrDefault()}";
            if (ServiceContext.IsRun)
                IsServiseRunText = "Running";
            else
                IsServiseRunText = "Stopped";
        }
        catch (Exception)
        {

        }
    }

    [RelayCommand]
    private void InstallOrReinstallService()
    {
        try
        {
            ServiceContext.InstallOrReinstallService();
        }
        catch (Exception)
        {

        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(Autorun))
        {
            ServiceContext.Autorun = Autorun;
        }
        base.OnPropertyChanged(e);
    }
}
