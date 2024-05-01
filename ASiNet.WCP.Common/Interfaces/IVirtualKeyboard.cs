using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.Common.Interfaces;
public interface IVirtualKeyboard
{

    public bool SendKeyEvent(KeyChandgeEvent keyChandge);

    public bool SendKeyEvent(KeyCode keyCode, KeyState keyState);

    public bool SendKeyEventDirect(KeyChandgeEvent keyChandge);

    public bool SendKeyEventDirect(KeyCode keyCode, KeyState keyState);

}
