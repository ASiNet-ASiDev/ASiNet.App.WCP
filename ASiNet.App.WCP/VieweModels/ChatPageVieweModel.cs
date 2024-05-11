using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class ChatPageVieweModel : ObservableObject
{
    public ChatPageVieweModel()
    {
        _client = ((ShellVieweModel)Shell.Current.BindingContext).WcpClient;
    }

    public ObservableCollection<ChatMessagesGroup> Messages { get; } = [];

    [ObservableProperty]
    private bool _loading;

    private WcpClient _client;

    [RelayCommand]
    private async Task LoadMessages()
    {
        try
        {
            if (Loading)
                return;
            Loading = true;
            var request = new GetHistoryRequest()
            { 
                MaxCount = 16, 
                StartId = Messages.FirstOrDefault()?.FirstOrDefault()?.Id ?? long.MaxValue 
            };
            Debug.WriteLine($"GET: {request.StartId}");
            var history = await _client.GetHistory(request);
            if(history?.Items is null)
                return;
            foreach (var item in history.Items)
            {
                var g = Messages.FirstOrDefault(x => x.Time == DateOnly.FromDateTime(item.SendedTime));
                if(g is null)
                {
                    g = new ChatMessagesGroup(DateOnly.FromDateTime(item.SendedTime), []);
                    Messages.Insert(0, g);
                }

                g.Insert(0, new(item.Text!, item.SendedTime) { Id = item.Id, Client = _client });
            }
        }
        catch (Exception)
        {

        }
        finally
        {
            Loading = false;
        }
    }

    [RelayCommand]
    private async Task SendMessage(ChatMessageVieweModel? msg)
    {
        if (msg is null)
            return;
        msg.Client = _client;
        if (await msg.Send())
        {
            var g = Messages.FirstOrDefault(x => x.Time == DateOnly.FromDateTime(msg.Date));
            if (g is null)
            {
                Messages.Add(new(DateOnly.FromDateTime(msg.Date), [msg]));
            }
            else
            {
                g.Add(msg);
            }
        }
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("//root");
    }
}
