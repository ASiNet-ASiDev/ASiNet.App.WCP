using System.Text;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.Core;
public class TransportEndPoint
{
    public TransportEndPoint(WcpClient client, Stream fileStream, string endpointFile, TransportAction action, TransportDataEndPoint endPoint)
    {
        _client = client;
        Id = Guid.NewGuid();
        _dataStream = fileStream;
        _action = action;
        _endPoint = endPoint;
        _endPointFilePath = endpointFile;
        _datatype = TransportDataType.File;
        var data = ReadNextData();
        _client.SendTransportPackage(new()
        {
            OperationId = Id,
            Action = _action | TransportAction.Create,
            EndpointFilePath = endpointFile,
            DataType = _datatype,
            EndPoint = endPoint,
            FragmentsCount = _fragmentsCount,
            FragmentIndex = _lastFragmentIndex,
            TotalDataSize = fileStream.Length,
            Data = data
        });
    }

    public static TransportEndPoint PostFile(WcpClient client, FileStream file, string endpointFile)
    {
        return new(client, file, endpointFile, TransportAction.Post, TransportDataEndPoint.File);
    }

    public static TransportEndPoint PostTextAsFile(WcpClient client, string text, string endpointFile)
    {
        var utf = Encoding.UTF8.GetBytes(text);
        var mstr = new MemoryStream(utf);
        return new(client, mstr, endpointFile, TransportAction.Post, TransportDataEndPoint.File);
    }

    public Guid Id { get; }

    private WcpClient _client;

    private Stream _dataStream;

    private TransportDataEndPoint _endPoint;

    private TransportAction _action;
    private TransportDataType _datatype;

    private string _endPointFilePath;

    private int _fragmentsCount;

    private int _lastFragmentIndex = 0;

    private const ushort BUFFER_SIZE = 54512;

    private byte[]? _buffer;

    public TransportDataRequest? Chandge(TransportDataResponse request)
    {
        if (request.Status == TransportDataStatus.Ok)
        {
            if (_action.HasFlag(TransportAction.Get))
            {

            }
            else if (_action.HasFlag(TransportAction.Post))
            {
                var data = ReadNextData();
                var pack = new TransportDataRequest()
                {
                    Action = _action,
                    Data = data,
                    OperationId = Id,
                    EndPoint = _endPoint,
                    EndpointFilePath = _endPointFilePath,
                    FragmentIndex = _fragmentsCount,
                    FragmentsCount = _lastFragmentIndex,
                    TotalDataSize = _dataStream.Length,
                    DataType = _datatype,
                };

                return pack;
            }
        }
        Dispose();
        return null;
    }

    private byte[] ReadNextData()
    {
        if (_fragmentsCount == 0)
        {
            _fragmentsCount = (int)Math.Ceiling((double)_dataStream.Length / (double)BUFFER_SIZE);
        }
        _lastFragmentIndex++;
        _buffer ??= new byte[BUFFER_SIZE];
        var size = _dataStream.Read(_buffer);
        return _buffer[..size];
    }

    public void Dispose()
    {
        _dataStream?.Dispose();
    }
}
