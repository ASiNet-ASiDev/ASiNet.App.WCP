using System.Windows.Input;

namespace ASiNet.App.WCP.Viewe;

public partial class SendTextPage : ContentPage
{
	public SendTextPage()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SendTextPage));

    public ICommand? Command
    {
        get { return (ICommand?)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    private async void Button_FillFromClipboard(object sender, EventArgs e)
    {
        TextEditor.Text = (await Clipboard.GetTextAsync()) ?? string.Empty;
    }

    private async void Button_Pressed_SendFromClipboard(object sender, EventArgs e)
    {
        var text = await Clipboard.GetTextAsync();
        Command?.Execute(text);
    }
}