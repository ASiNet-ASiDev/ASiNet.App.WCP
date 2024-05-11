using System.Runtime.InteropServices;
using ASiNet.WCP.WinApi.Enums;

namespace ASiNet.WCP.WinApi;
public static class WindowsClipboard
{
    public static bool SetTextToClipboard(string text)
    {
        try
        {
            if (Functions.OpenClipboard(IntPtr.Zero))
            {
                if (!Functions.OpenClipboard(IntPtr.Zero))
                {

                    return false;
                }
                Functions.EmptyClipboard();
                Functions.SetClipboardData((int)ClipboardDataType.UnicodeText, Marshal.StringToHGlobalUni(text));
                Functions.CloseClipboard();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
