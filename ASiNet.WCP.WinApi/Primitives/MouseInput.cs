using System.Runtime.InteropServices;
using ASiNet.WCP.WinApi.Enums;

namespace ASiNet.WCP.WinApi.Primitives;
[StructLayout(LayoutKind.Sequential)]
public struct MouseInput
{
    public int dx;
    public int dy;
    public int mouseData;
    public MouseEventFlag dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}