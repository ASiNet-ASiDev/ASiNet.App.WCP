using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Core.Macroses;

namespace ASiNet.WCP.Macroses.Core.Actions;

public class KeyboardAction : IUserAction
{
    public TimeSpan TimeOffset { get; set; }

    public KeySendType KeySendType { get; set; }

    public KeyCode KeyCode { get; set; }

    public KeyState KeyState { get; set; }
}
