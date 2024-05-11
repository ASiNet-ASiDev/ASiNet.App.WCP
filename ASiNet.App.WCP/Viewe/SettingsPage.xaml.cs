using ASiNet.App.WCP.VieweModels;

namespace ASiNet.App.WCP.Viewe;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        AutoConnectCheckBox.IsChecked = App.Config.AutoConnect;
	}

    protected override void OnAppearing()
    {
        PortEntry.Text = App.Config.Port.ToString();
        AddressEntry.Text = App.Config.Address;
        ReverseCheckBox.IsChecked = App.Config.ReverseOrientation;
        AutoConnectCheckBox.IsChecked = App.Config.AutoConnect;
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        App.Config.SaveOrUpdate();
        base.OnDisappearing();
    }

    private void ReverseCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        App.Config.ReverseOrientation = e.Value;
    }

    private void PortEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(int.TryParse(e.NewTextValue, out var port) || port == 0 || port > ushort.MaxValue)
        {
            App.Config.Port = port;
        }
    }

    private void AddressEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(ConnectionPageVieweModel.AddresValidation().IsMatch(e.NewTextValue))
        {
            App.Config.Address = e.NewTextValue;
        }
    }

    private void AutoConnectCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        App.Config.AutoConnect = e.Value;
    }

    private async void Button_Pressed(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new InfoPage());
    }
}