using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASiNet.App.WCP.VieweModels;
public class ChatMessagesGroup : ObservableCollection<ChatMessageVieweModel>
{

    public ChatMessagesGroup(DateOnly time, List<ChatMessageVieweModel> messages) : base(messages.OrderBy(x => x.Date))
    {
        Time = time;
    }

    public DateOnly Time { get; set; }

}