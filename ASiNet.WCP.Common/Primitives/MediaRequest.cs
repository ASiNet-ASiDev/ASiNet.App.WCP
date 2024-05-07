using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class MediaRequest : Package
{
    public MediaAction Action { get; set; }

    public string? DirectoryPath { get; set; }

    public string FileName { get; set; } = null!;
}


public class MediaResponse : Package
{

    public MediaStatus Status { get; set; }

    public string? Address { get; set; }

    public int Port { get; set; }
}