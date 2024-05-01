using System.Runtime.InteropServices;
using System.Text;
using ASiNet.WCP.WinApi.Enums;
using ASiNet.WCP.WinApi.Primitives;

namespace ASiNet.WCP.WinApi;
internal static class Functions
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    public static extern IntPtr GetMessageExtraInfo();

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point lpPoint);

    [DllImport("User32.dll")]
    public static extern bool SetCursorPos(int x, int y);
    [DllImport("User32.dll")]
    public static extern uint LoadKeyboardLayout(StringBuilder pwszKLID, uint flags);
    [DllImport("User32.dll")]
    public static extern uint GetKeyboardLayout(uint idThread);
    [DllImport("User32.dll")]
    public static extern uint ActivateKeyboardLayout(uint hkl, uint Flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("User32.dll")]
    public static extern ushort MapVirtualKeyA(ushort uCode, MapType Flags);
}