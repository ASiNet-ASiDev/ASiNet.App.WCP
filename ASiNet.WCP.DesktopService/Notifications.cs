using System.Text;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.History;
using ASiNet.WCP.History.Actions;
using ASiNet.WCP.WinApi;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Notifications;

namespace ASiNet.WCP.DesktopService;
public class Notifications : IDisposable
{
    public Notifications()
    {
        ToastNotificationManagerCompat.OnActivated += OnAction;
    }



    public void Chandge(NotificationEvent @event)
    {
        switch (@event.Type)
        {
            case Common.Enums.NotificationType.OnNewString:
                OnStringEvent(@event.Data);
                break;
            case Common.Enums.NotificationType.OnOldString:
                OnOldStringEvent(@event.Id);
                break;
        }
    }

    private void OnStringEvent(byte[]? utf8Data)
    {
        try
        {
            if (utf8Data is null)
                return;
            var str = Encoding.UTF8.GetString(utf8Data);
            var notifyText = str.Length > 64 ? $"{str[..64]}..." : str;

            var id = 0L;

            using (var context = new HistoryContext())
            {
                id = context.AddRecord(new TextRecord(str, DateTime.Now)).Id;
            }

            new ToastContentBuilder()
                .AddText($"WCP Accepted text:")
                .AddText(notifyText)
                .AddButton("Copy to clipboard", ToastActivationType.Foreground, id.ToString())
                .Show();
        }
        catch (Exception)
        {
            new ToastContentBuilder()
                .AddText($"WCP Accept text error")
                .Show();
        }
    }

    private void OnOldStringEvent(long? id)
    {
        try
        {
            if (id is null)
                return;

            string? text = null;
            using (var context = new HistoryContext())
            {
                text = context.TextRecords.Find(id.Value)?.Text;
            }

            if(text is null )
                return;

            var notifyText = text.Length > 64 ? $"{text[..64]}..." : text;

            new ToastContentBuilder()
                .AddText($"WCP Accepted text:")
                .AddText(notifyText)
                .AddButton("Copy to clipboard", ToastActivationType.Foreground, id.ToString())
                .Show();
        }
        catch (Exception)
        {
            new ToastContentBuilder()
                .AddText($"WCP Accept text error")
                .Show();
        }
    }


    private void OnAction(ToastNotificationActivatedEventArgsCompat e)
    {
        using (var context = new HistoryContext())
        {
            var record = context.GetRecord(long.Parse(e.Argument));
            
            if(record is TextRecord tr)
            {
                WindowsClipboard.SetTextToClipboard(tr.Text);
                
            }
        }
    }

    public void Dispose()
    {
        ToastNotificationManagerCompat.OnActivated -= OnAction;
    }
}
