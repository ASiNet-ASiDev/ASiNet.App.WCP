using System.Windows.Input;

namespace ASiNet.App.WCP.Viewe.Messages;

public partial class ChatTextMessage : Border
{
	public ChatTextMessage()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty SendClipboardNotifyCommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ChatTextMessage));

    public ICommand? SendClipboardNotifyCommand
    {
        get { return (ICommand?)GetValue(SendClipboardNotifyCommandProperty); }
        set { SetValue(SendClipboardNotifyCommandProperty, value); }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var result = await App.AlertSvc.ShowActionSheet(WCP.Resources.Localization.AppResources.msg_actions_title, null, null,
            WCP.Resources.Localization.AppResources.msg_send_to_desktop);

        if(result == WCP.Resources.Localization.AppResources.msg_send_to_desktop)
            SendClipboardNotifyCommand?.Execute(null);
    }
}