using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class TransportDataResponse : Package
{
    public Guid OperationId { get; set; }

    public TransportDataStatus Status { get; set; }


    public int FragmentsCount { get; set; }

    public int FragmentIndex { get; set; }

    public long TotalDataSize { get; set; }

    public byte[]? HashSum { get; set; }

    public byte[]? Data { get; set; }
}
