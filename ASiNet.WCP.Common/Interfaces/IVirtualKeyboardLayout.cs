using ASiNet.WCP.Common.Enums;

namespace ASiNet.WCP.Common.Interfaces;
public interface IVirtualKeyboardLayout
{
    public uint LoadLanguage(LanguageCode code);

    public bool SetLanguage(LanguageCode code);

    public LanguageCode GetCurrentLanguage();
}
