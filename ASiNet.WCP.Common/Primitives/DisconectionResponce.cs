using ASiNet.Data.Serialization.Attributes;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Core.Primitives;

[PreGenerate]
public class DisconectionResponce : Package
{
    public bool Success { get; set; }
}
