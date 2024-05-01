using System.Runtime.InteropServices;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Interfaces;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.WinApi.Enums;
using ASiNet.WCP.WinApi.Primitives;

namespace ASiNet.WCP.WinApi;
public class WindowsMouse : IVirtualMouse
{
    private static int _inputSize = Marshal.SizeOf<Input>();

    public bool SendMouseEvent(double xOffset, double yOffset, ushort speedMultiplier, short mouseWheel, MouseButtons button)
    {
        try
        {
            var (firstPackage, isGeherateSecond) = GenerateFirstPackage(xOffset, yOffset, speedMultiplier, mouseWheel, button);

            if (!isGeherateSecond)
            {
                _ = Functions.SendInput(1, [firstPackage], _inputSize);
                return true;
            }

            var seconPackage = GenerateSecondPackage(button);

            _ = Functions.SendInput(2, [firstPackage, seconPackage], _inputSize);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool SendMouseEvent(MouseChangedEvent change)
    {
        try
        {
            var (firstPackage, isGeherateSecond) = GenerateFirstPackage(change.XOffset, change.YOffset, change.SpeedMultiplier, change.MouseWheel, change.ButtonEvent);

            if (!isGeherateSecond)
            {
                _ = Functions.SendInput(1, [firstPackage], _inputSize);
                return true;
            }

            var seconPackage = GenerateSecondPackage(change.ButtonEvent);

            _ = Functions.SendInput(2, [firstPackage, seconPackage], _inputSize);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private (Input, bool GenerateSecond) GenerateFirstPackage(double xOffset, double yOffset, ushort speedMultiplier, short mouseWheel, MouseButtons buttons)
    {
        var input = new Input
        {
            type = InputType.Mouse
        };
        var mouseInput = new MouseInput();
        var generateSecond = false;

        if (speedMultiplier > 0)
        {
            mouseInput.dwFlags |= MouseEventFlag.Move;
            mouseInput.dx = (int)Math.Round(xOffset * speedMultiplier);
            mouseInput.dy = (int)Math.Round(yOffset * speedMultiplier);
        }
        if (mouseWheel is not 0)
        {
            mouseInput.dwFlags |= MouseEventFlag.Wheel;
            mouseInput.mouseData = mouseWheel;
        }
        if (buttons != MouseButtons.None)
        {
            generateSecond = buttons is MouseButtons.LeftClick or MouseButtons.RightClick or MouseButtons.MiddleClick or MouseButtons.XButtonClick;
            mouseInput.dwFlags |= MouseButtonEventConvert(buttons);
        }
        mouseInput.dwExtraInfo = Functions.GetMessageExtraInfo();
        input.u = new() { mi = mouseInput };
        return (input, generateSecond);
    }

    private Input GenerateSecondPackage(MouseButtons buttons)
    {
        var input = new Input
        {
            type = InputType.Mouse
        };
        var mouseInput = new MouseInput();

        mouseInput.dwFlags = buttons switch
        {
            MouseButtons.LeftClick => MouseEventFlag.LeftUp,
            MouseButtons.RightClick => MouseEventFlag.RightUp,
            MouseButtons.MiddleClick => MouseEventFlag.MiddleUp,
            MouseButtons.XButtonClick => MouseEventFlag.XUp,
            _ => 0,
        };

        mouseInput.dwExtraInfo = Functions.GetMessageExtraInfo();
        input.u = new() { mi = mouseInput };
        return input;
    }


    private MouseEventFlag MouseButtonEventConvert(MouseButtons mouseButtons) => mouseButtons switch
    {
        MouseButtons.LeftClick => MouseEventFlag.LeftDown,
        MouseButtons.RightClick => MouseEventFlag.RightDown,
        MouseButtons.MiddleClick => MouseEventFlag.MiddleDown,
        MouseButtons.XButtonClick => MouseEventFlag.XDown,
        MouseButtons.LeftDown => MouseEventFlag.LeftDown,
        MouseButtons.RightDown => MouseEventFlag.RightDown,
        MouseButtons.MiddleDown => MouseEventFlag.MiddleDown,
        MouseButtons.XButtonDown => MouseEventFlag.XDown,
        MouseButtons.LeftUp => MouseEventFlag.LeftUp,
        MouseButtons.RightUp => MouseEventFlag.RightUp,
        MouseButtons.MiddleUp => MouseEventFlag.MiddleUp,
        MouseButtons.XButtonUp => MouseEventFlag.XUp,
        _ => 0
    };
}
