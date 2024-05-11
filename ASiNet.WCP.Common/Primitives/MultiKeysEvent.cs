using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class MultiKeysEvent : Package
{

    public static MultiKeysEvent FromString(string text)
    {
        var isPressedShift = false;
        var keys = new List<KeyEvent>();
        foreach (var key in text)
        {
            if (char.IsLetter(key))
            {
                if (char.IsUpper(key))
                {
                    if (!isPressedShift)
                    {
                        keys.Add(new() { Mod = KeyCode.Shift, State = KeyState.Down });
                        isPressedShift = true;
                    }
                }
                else
                {
                    if (isPressedShift)
                    {
                        keys.Add(new() { Mod = KeyCode.Shift, State = KeyState.Up });
                        isPressedShift = false;
                    }
                }
                keys.Add(new() { Code = key, State = KeyState.Click });
            }
        }

        if(isPressedShift)
            keys.Add(new() { Mod = KeyCode.Shift, State = KeyState.Up });
        return new() { Events = [.. keys] };
    }

    public KeyEvent[] Events { get; set; } = null!;
}

public struct KeyEvent
{
    public char Code { get; set; }

    public KeyCode Mod {  get; set; }
    public KeyState State { get; set; }
}