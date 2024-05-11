using Java.Net;

namespace ASiNet.App.WCP.Viewe;

public partial class InfoPage : ContentPage
{
	public InfoPage()
	{
		InitializeComponent();
	}

    private async void TapGestureRecognizer_OpenWebsite(object sender, TappedEventArgs e)
    {
        await Launcher.OpenAsync("https://asinet-asidev.github.io/");
    }
    private async void TapGestureRecognizer_OpenGitHub(object sender, TappedEventArgs e)
    {
        await Launcher.OpenAsync("https://github.com/ASiNet-ASiDev/ASiNet.App.WCP");
    }
    private async void TapGestureRecognizer_OpenPP(object sender, TappedEventArgs e)
    {
        await Launcher.OpenAsync("https://asinet-asidev.github.io/?page=ppolicy_ru");
    }
}