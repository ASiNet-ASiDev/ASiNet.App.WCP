using ASiNet.WCP.Common.Enums;

namespace ASiNet.WCP.Common.Primitives;
public class HistoryItem
{
    public HistoryItemType ItemType { get; set; }

    public long Id { get; set; }

    public DateTime SendedTime { get; set; }

    public string? Text { get; set; }
}
