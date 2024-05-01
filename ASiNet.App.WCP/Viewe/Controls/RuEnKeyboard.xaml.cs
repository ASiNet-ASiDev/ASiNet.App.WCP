namespace ASiNet.App.WCP.Viewe.Controls;

public partial class RuEnKeyboard : ContentPage
{
    public RuEnKeyboard()
    {
        InitializeComponent();
        Disappearing += OnDisappearing;
        Appearing += OnAppearing;
    }

    private void OnAppearing(object? sender, EventArgs e)
    {
        OrientationService.Current.SetOrientation(DisplayOrientation.Landscape, App.Config.ReverseOrientation);
    }

    private void OnDisappearing(object? sender, EventArgs e)
    {
        OrientationService.Current.SetOrientation(DisplayOrientation.Portrait, App.Config.ReverseOrientation);
    }
}