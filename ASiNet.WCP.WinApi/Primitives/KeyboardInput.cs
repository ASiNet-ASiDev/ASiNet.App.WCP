using System.Runtime.InteropServices;
using ASiNet.WCP.WinApi.Enums;

namespace ASiNet.WCP.WinApi.Primitives;
[StructLayout(LayoutKind.Sequential)]
public struct KeyboardInput
{
    public ushort wVk;
    public ushort wScan;
    public KeyEventFlag dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}
