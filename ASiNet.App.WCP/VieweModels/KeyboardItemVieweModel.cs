namespace ASiNet.App.WCP.VieweModels;
public partial class KeyboardItemVieweModel<T>(KeyboardVieweModel keyboardVieweModel) : ComponentVieweModel where T : Page, new()
{
    private KeyboardVieweModel _keyboardVieweModel = keyboardVieweModel;

    public override async Task Hide()
    {
        Shell.Current.Dispatcher.Dispatch(() => Working = true);
        await Shell.Current.Navigation.PopModalAsync();
        Shell.Current.Dispatcher.Dispatch(() => Working = false);
    }

    public override async Task Show()
    {
        Shell.Current.Dispatcher.Dispatch(() =>  Working = true);
        await Shell.Current.Navigation.PushModalAsync(new T() { BindingContext = _keyboardVieweModel });
        Shell.Current.Dispatcher.Dispatch(() => Working = false);
    }
}
