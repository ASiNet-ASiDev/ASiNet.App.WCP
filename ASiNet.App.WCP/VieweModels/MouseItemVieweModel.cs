namespace ASiNet.App.WCP.VieweModels;
public class MouseItemVieweModel<T>(MouseVieweModel mouseVieweModel) : ComponentVieweModel where T : Page, new()
{

    private MouseVieweModel _mouseVieweModel = mouseVieweModel;

    public override async Task Hide()
    {
        Shell.Current.Dispatcher.Dispatch(() => Working = true);
        await Shell.Current.Navigation.PopModalAsync();
        Shell.Current.Dispatcher.Dispatch(() => Working = false);
    }

    public override async Task Show()
    {
        Shell.Current.Dispatcher.Dispatch(() => Working = true);
        await Shell.Current.Navigation.PushModalAsync(new T() { BindingContext = _mouseVieweModel });
        Shell.Current.Dispatcher.Dispatch(() => Working = false);
    }
}
