using System.Runtime.InteropServices;

namespace ASiNet.WCP.WinApi.Primitives;
[StructLayout(LayoutKind.Sequential)]
public struct HardwareInput
{
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
}