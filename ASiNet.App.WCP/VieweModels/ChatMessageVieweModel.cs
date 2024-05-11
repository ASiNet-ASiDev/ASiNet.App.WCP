using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kotlin.Properties;

namespace ASiNet.App.WCP.VieweModels;
public partial class ChatMessageVieweModel : ObservableObject
{
    public ChatMessageVieweModel(string text, DateTime date)
    {
        Client = null!;
        Text = text;
        Date = date;
    }

    public WcpClient Client;

    [ObservableProperty]
    private long _id;

    [ObservableProperty]
    private string? _text;

    [ObservableProperty]
    private string? _subText;


    [ObservableProperty]
    private DateTime _date;

    [RelayCommand]
    private void ClipboardNotification()
    {
        Client.SendNotifyChandgedEvent(new() { Id = Id, Type = NotificationType.OnOldString });
    }


    public async Task<bool> Send()
    {
        Id = await Client.PostHistory(new() { Item = new() { Text = Text, ItemType = HistoryItemType.Text, SendedTime = Date } });

        return Id != 0;
    }
}
