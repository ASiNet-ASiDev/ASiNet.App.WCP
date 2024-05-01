using ASiNet.Data.Serialization.Attributes;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Core.Primitives;

[PreGenerate]
public class LanguageResponse : Package
{
    public bool Success { get; set; }

    public LanguageCode LanguageCode { get; set; }
}
