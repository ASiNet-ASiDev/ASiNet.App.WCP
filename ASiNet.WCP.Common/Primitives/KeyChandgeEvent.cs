using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class KeyChandgeEvent : Package
{
    public KeyCode Code { get; set; }

    public KeyState State { get; set; }
}
