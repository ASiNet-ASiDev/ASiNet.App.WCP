using System.Text;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kotlin.Properties;

namespace ASiNet.App.WCP.VieweModels;
public partial class SendTextPageVieweModel : ObservableObject
{
    public SendTextPageVieweModel(WcpClient client)
    {
        _client = client;
    }

    [ObservableProperty]
    private string? _text;

    private WcpClient _client;

    [RelayCommand]
    private void SendText()
    {
        if (string.IsNullOrEmpty(Text))
            return;
        var ne = new NotificationEvent()
        {
            Type = ASiNet.WCP.Common.Enums.NotificationType.OnNewString,
            Data = Encoding.UTF8.GetBytes(Text)
        };
        _client.SendNotifyChandgedEvent(ne);
    }

    [RelayCommand]
    private void SendFromClipboardText(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return;
        var ne = new NotificationEvent()
        {
            Type = ASiNet.WCP.Common.Enums.NotificationType.OnNewString,
            Data = Encoding.UTF8.GetBytes(text)
        };
        _client.SendNotifyChandgedEvent(ne);
    }
}
