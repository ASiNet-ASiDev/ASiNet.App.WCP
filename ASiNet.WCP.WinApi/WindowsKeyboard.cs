using System.Runtime.InteropServices;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Interfaces;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.WinApi.Enums;
using ASiNet.WCP.WinApi.Primitives;

namespace ASiNet.WCP.WinApi;
public class WindowsKeyboard : IVirtualKeyboard
{
    private static int _inputSize = Marshal.SizeOf<Input>();



    public bool SendKeyEventDirect(KeyChandgeEvent keyChandge)
    {
        try
        {
            Input[] inputs = keyChandge.State switch
            {
                KeyState.Click => [
                    NewDirectInput(keyChandge.Code, KeyEventFlag.KeyDown),
                    NewDirectInput(keyChandge.Code, KeyEventFlag.KeyUp),
                ],
                KeyState.Down => [
                    NewDirectInput(keyChandge.Code, KeyEventFlag.KeyDown),
                ],
                KeyState.Up => [
                    NewDirectInput(keyChandge.Code, KeyEventFlag.KeyUp),
                ],
                _ => throw new NotImplementedException()
            };

            _ = Functions.SendInput((uint)inputs.Length, inputs, _inputSize);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool SendKeyEventDirect(KeyCode keyCode, KeyState keyState)
    {
        try
        {
            Input[] inputs = keyState switch
            {
                KeyState.Click => [
                    NewDirectInput(keyCode, KeyEventFlag.KeyDown),
                    NewDirectInput(keyCode, KeyEventFlag.KeyUp),
                ],
                KeyState.Down => [
                    NewDirectInput(keyCode, KeyEventFlag.KeyDown),
                ],
                KeyState.Up => [
                    NewDirectInput(keyCode, KeyEventFlag.KeyUp),
                ],
                _ => throw new NotImplementedException()
            };

            _ = Functions.SendInput((uint)inputs.Length, inputs, _inputSize);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool SendKeyEvent(KeyCode keyCode, KeyState keyState)
    {
        try
        {
            Input[] inputs = keyState switch
            {
                KeyState.Click => [
                    NewInput(keyCode, KeyEventFlag.KeyDown),
                    NewInput(keyCode, KeyEventFlag.KeyUp),
                ],
                KeyState.Down => [
                    NewInput(keyCode, KeyEventFlag.KeyDown),
                ],
                KeyState.Up => [
                    NewInput(keyCode, KeyEventFlag.KeyUp),
                ],
                _ => throw new NotImplementedException()
            };

            _ = Functions.SendInput((uint)inputs.Length, inputs, _inputSize);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool SendKeyEvent(KeyChandgeEvent keyChandge)
    {
        try
        {
            Input[] inputs = keyChandge.State switch
            {
                KeyState.Click => [
                    NewInput(keyChandge.Code, KeyEventFlag.KeyDown),
                    NewInput(keyChandge.Code, KeyEventFlag.KeyUp),
                ],
                KeyState.Down => [
                    NewInput(keyChandge.Code, KeyEventFlag.KeyDown),
                ],
                KeyState.Up => [
                    NewInput(keyChandge.Code, KeyEventFlag.KeyUp),
                ],
                _ => throw new NotImplementedException()
            };

            _ = Functions.SendInput((uint)inputs.Length, inputs, _inputSize);

            return true;
        }
        catch
        {
            return false;
        }
    }


    private static Input NewInput(KeyCode code, KeyEventFlag flag) =>
        new()
        {
            type = InputType.Keyboard,
            u = new InputUnion()
            {
                ki = new()
                {
                    wVk = (ushort)code,
                    wScan = 0,
                    dwFlags = flag,
                    dwExtraInfo = Functions.GetMessageExtraInfo(),
                }
            }
        };

    private static Input NewDirectInput(KeyCode code, KeyEventFlag flag) =>
        new()
        {
            type = InputType.Keyboard,
            u = new InputUnion()
            {
                ki = new()
                {
                    wVk = 0,
                    wScan = Functions.MapVirtualKeyA((ushort)code, MapType.VirtualCodeToScanCode),
                    dwFlags = flag | KeyEventFlag.Scancode,
                    dwExtraInfo = Functions.GetMessageExtraInfo(),
                }
            }
        };
}
