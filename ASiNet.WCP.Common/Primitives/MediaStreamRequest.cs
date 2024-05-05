using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class MediaStreamRequest : Package
{
    public MediaAction Action { get; set; }

    public string? Path { get; set; }
}


public class MediaStreamResponse : Package
{

    public MediaStreamStatus Status { get; set; }

    public string? Address { get; set; }

    public int Port { get; set; }
}