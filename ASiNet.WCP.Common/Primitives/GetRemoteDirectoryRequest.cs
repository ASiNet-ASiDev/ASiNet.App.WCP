using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class GetRemoteDirectoryRequest : Package
{
    public bool GetRoots { get; set; }
    public string? Root { get; set; }

    public bool GetFiles { get; set; }
}

