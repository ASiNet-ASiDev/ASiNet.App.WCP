using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.History.Primitives;

namespace ASiNet.WCP.History.Actions;
public class TextRecord : HistoryRecord
{
    public TextRecord()
    {
        
    }

    public TextRecord(string text, DateTime createdTime) : base(createdTime)
    {
        Text = text;
    }

    public string Text { get; set; } = null!;

}
