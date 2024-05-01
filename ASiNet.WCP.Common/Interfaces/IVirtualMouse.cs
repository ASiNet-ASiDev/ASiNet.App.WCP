using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.Common.Interfaces;
public interface IVirtualMouse
{
    public bool SendMouseEvent(MouseChangedEvent change);

    public bool SendMouseEvent(double xOffset, double yOffset, ushort speedMultiplier, short mouseWheel, MouseButtons button);
}
