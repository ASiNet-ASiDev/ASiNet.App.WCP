using ASiNet.App.WCP.VieweModels;

namespace ASiNet.App.WCP.Viewe;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        AutoConnectCheckBox.IsChecked = App.Config.AutoConnect;
        PortEntry.Text = App.Config.Port.ToString();
        AddressEntry.Text = App.Config.Address;
        ReverseCheckBox.IsChecked = App.Config.ReverseOrientation;
	}

    private void ReverseCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        App.Config.ReverseOrientation = e.Value;
        App.Config.SaveOrUpdate();
    }

    private void PortEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(int.TryParse(e.NewTextValue, out var port) || port == 0 || port > ushort.MaxValue)
        {
            App.Config.Port = port;
            App.Config.SaveOrUpdate();
        }
    }

    private void AddressEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(ConnectPageVieweModel.AddresValidation().IsMatch(e.NewTextValue))
        {
            App.Config.Address = e.NewTextValue;
            App.Config.SaveOrUpdate();
        }
    }

    private void AutoConnectCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        App.Config.AutoConnect = e.Value;
        App.Config.SaveOrUpdate();
    }
}