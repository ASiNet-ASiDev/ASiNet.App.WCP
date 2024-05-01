using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.App.WCP.Models;
public class KeyboardKeyResult
{

    public string Name { get; set; } = null!;

    public string AltName { get; set; } = null!;

    public string ShiftName { get; set; } = null!;

    public string AltShiftName { get; set; } = null!;

    public KeyCode KeyCode { get; set; }

    public KeySupportedMode Mode { get; set; }


    public KeyChandgeEvent ToKeyboardEvent(bool isPressed)
    {
        switch (Mode)
        {
            case KeySupportedMode.Default:
                return new()
                {
                    Code = KeyCode,
                    State = KeyState.Click
                };
            case KeySupportedMode.Hold:
                return new()
                {
                    Code = KeyCode,
                    State = isPressed ? KeyState.Down : KeyState.Up
                };
            default:
                break;
        }
        throw new NotImplementedException();
    }
}
