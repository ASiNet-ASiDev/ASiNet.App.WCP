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

    [DllImport("User32")]
    internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("User32")]
    internal static extern bool CloseClipboard();

    [DllImport("User32")]
    internal static extern bool EmptyClipboard();

    [DllImport("User32")]
    internal static extern bool IsClipboardFormatAvailable(int format);

    [DllImport("User32")]
    internal static extern IntPtr GetClipboardData(int uFormat);

    [DllImport("User32", CharSet = CharSet.Unicode)]
    internal static extern IntPtr SetClipboardData(int uFormat, IntPtr hMem);
}