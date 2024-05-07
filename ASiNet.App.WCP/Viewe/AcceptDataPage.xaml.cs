namespace ASiNet.App.WCP.Viewe;

public partial class AcceptDataPage : ContentPage
{
	public AcceptDataPage()
	{
		InitializeComponent();
	}


    private async void Button_Pressed(object sender, EventArgs e)
    {
        var options = new PickOptions() { PickerTitle = "Send file" };
        var result = await FilePicker.Default.PickAsync(options);
        if (result is not null)
        {
            LocalFile.Text = result.FullPath;
        }
    }
}