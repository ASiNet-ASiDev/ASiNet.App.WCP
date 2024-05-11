using System.ComponentModel.DataAnnotations;

namespace ASiNet.WCP.History.Primitives;
public class HistoryRecord
{
    public HistoryRecord()
    {
        
    }

    public HistoryRecord(DateTime createdTime)
    {
        CreatedTime = createdTime;
    }

    [Key]
    public long Id { get; set; }

    public DateTime CreatedTime { get; set; }
}
