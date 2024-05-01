using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Core.Macroses;

namespace ASiNet.WCP.Macroses.Core.Actions;

public class MouseAction : IUserAction
{
    public TimeSpan TimeOffset { get; set; }

    public MouseButtons ButtonEvent { get; set; }

    public short MouseWheel { get; set; }

    public ushort SpeedMultiplier { get; set; }

    public double XOffset { get; set; }

    public double YOffset { get; set; }

}
