using ASiNet.WCP.Common.Network;
using ASiNet.WCP.Core.Macroses;

namespace ASiNet.WCP.Common.Primitives;
public class AddMacrosRequest : Package
{
    public MacrosData[] MakrosData { get; set; } = null!;
}
