using ASiNet.App.WCP.Viewe;
using ASiNet.WCP.Core;
using ASiNet.WCP.Core.Macroses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class MacrosesVieweModel : ObservableObject
{
    public MacrosesVieweModel()
    {
        _client = ((ShellVieweModel)Shell.Current.BindingContext).WcpClient;
    }



    private WcpClient _client;

    public void AddMacros(MacrosData md)
    {

    }

    [RelayCommand]
    private async Task CreateNewMacros()
    {
        await Shell.Current.Navigation.PushAsync(new MacrosBuilder() { BindingContext = new MacrosBuilderVieweModel(this) });
    }

}
