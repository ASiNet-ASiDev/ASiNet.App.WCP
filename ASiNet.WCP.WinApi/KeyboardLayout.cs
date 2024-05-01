using System.Text;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.WinApi;

namespace ASiNet.WCP.Core;
public class KeyboardLayout
{
    private const uint KLF_ACTIVATE = 0x00000001;
    private const int WM_INPUTLANGCHANGEREQUEST = 0x50;
    private const nint HWND_BROADCAST = 0xffff;

    private Dictionary<LanguageCode, uint> _loadedLanguages = [];
    private Dictionary<uint, LanguageCode> _hclLanguagePairs = [];

    public uint LoadLanguage(LanguageCode code)
    {
        try
        {
            var lc = ((int)code).ToString("x8");
            var pwsz = new StringBuilder(lc);
            var result = Functions.LoadKeyboardLayout(pwsz, 0);
            _loadedLanguages.Add(code, result);
            _hclLanguagePairs.Add(result, code);
            return result;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public bool SetLanguage(LanguageCode code)
    {
        if (_loadedLanguages.TryGetValue(code, out var hkl))
        {
            Functions.ActivateKeyboardLayout(hkl, 0);
            return Functions.PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, (int)KLF_ACTIVATE, (int)hkl);
        }
        return false;
    }

    public LanguageCode GetCurrentLanguage()
    {
        var result = Functions.GetKeyboardLayout(0);
        if (_hclLanguagePairs.TryGetValue(result, out var value))
            return value;
        return LanguageCode.None;
    }
}
