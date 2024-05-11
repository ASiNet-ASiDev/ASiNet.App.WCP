using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class GetHistoryRequest : Package
{
    public long StartId { get; set; }

    public int MaxCount { get; set; }
}

public class GetHistoryResponse : Package
{
    public HistoryItem[]? Items { get; set; }
}
