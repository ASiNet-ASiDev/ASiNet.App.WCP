using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;

public abstract partial class ToolVieweModel : ObservableObject
{

    [ObservableProperty]
    private string? _title;
    [ObservableProperty]
    private string? _description;
    [ObservableProperty]
    private string? _author;

    [ObservableProperty]
    private bool _working;

    [RelayCommand]
    public abstract Task Show();

    [RelayCommand]
    public abstract Task Hide();
}

public partial class ToolVieweModel<T, T1>(T1? vm) : ToolVieweModel where T : Page, new()
{
    protected T1? _vm = vm;

    public override async Task Show()
    {
        Shell.Current.Dispatcher.Dispatch(() => Working = true);
        await Shell.Current.Navigation.PushModalAsync(new T() { BindingContext = _vm });
        Shell.Current.Dispatcher.Dispatch(() => Working = false);
    }

    public override async Task Hide()
    {
        Shell.Current.Dispatcher.Dispatch(() => Working = true);
        await Shell.Current.Navigation.PopModalAsync();
        Shell.Current.Dispatcher.Dispatch(() => Working = false);
    }
}
