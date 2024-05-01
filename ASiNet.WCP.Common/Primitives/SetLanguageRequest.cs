using ASiNet.Data.Serialization.Attributes;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Core.Primitives;
[PreGenerate]
public class SetLanguageRequest : Package
{
    public LanguageCode LanguageCode { get; set; }
}
