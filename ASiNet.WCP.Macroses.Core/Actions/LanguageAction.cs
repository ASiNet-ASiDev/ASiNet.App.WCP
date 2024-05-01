using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Core.Macroses;

namespace ASiNet.WCP.Macroses.Core.Actions;

public class LanguageAction : IUserAction
{
    public TimeSpan TimeOffset { get; set; }

    public LanguageCode Language { get; set; }
}
