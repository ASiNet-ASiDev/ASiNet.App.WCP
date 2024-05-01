using ASiNet.Data.Serialization.Attributes;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
[PreGenerate]
public class MouseChangedEvent : Package
{
    public MouseChangedEvent()
    {

    }

    public MouseChangedEvent(double x, double y)
    {
        XOffset = x;
        YOffset = y;
    }

    public MouseButtons ButtonEvent { get; set; }

    public short MouseWheel { get; set; }

    public ushort SpeedMultiplier { get; set; }

    public double XOffset { get; set; }

    public double YOffset { get; set; }


}
