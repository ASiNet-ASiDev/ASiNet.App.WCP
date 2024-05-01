using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class TransportDataRequest : Package
{
    public Guid OperationId { get; set; }

    public TransportDataType DataType { get; set; }

    public TransportEndPoint EndPoint { get; set; }

    public TransportAction Action { get; set; }


    public string? EndpointFilePath { get; set; }

    public int FragmentsCount { get; set; }

    public int FragmentIndex { get; set; }

    public long TotalDataSize { get; set; }

    public byte[]? HashSum { get; set; }

    public byte[]? Data { get; set; }
}
