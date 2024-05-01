using System.Runtime.InteropServices;

namespace ASiNet.WCP.WinApi.Primitives;
[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int X;
    public int Y;
}