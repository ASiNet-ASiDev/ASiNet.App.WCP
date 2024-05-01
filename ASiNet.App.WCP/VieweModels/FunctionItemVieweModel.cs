using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public abstract partial class ComponentVieweModel : ObservableObject
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
