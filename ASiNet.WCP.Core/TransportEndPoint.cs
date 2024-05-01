using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Common.DataTransport;
using ASiNet.WCP.Common;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Common.Enums;

namespace ASiNet.WCP.Core;
public class TransportEndPoint
{
    public TransportEndPoint(WcpClient client, Stream fileStream, string endpointFile, TransportAction action, Common.Enums.TransportEndPoint endPoint)
    {
        _client = client;
        Id = Guid.NewGuid();
        _fileStream = fileStream;
        _action = action;
        _endPoint = endPoint;
        _endPointFilePath = endpointFile;
        _client.SendTransportPackage(new() { OperationId = Id, Action = _action, EndpointFilePath = endpointFile,  });
    }

    public TransportEndPoint PostFile(WcpClient client, FileStream file, string endpointFile)
    {
        return new(client, file, endpointFile, TransportAction.Post, Common.Enums.TransportEndPoint.File);
    }

    public TransportEndPoint PostTextAsFile(WcpClient client, string text, string endpointFile)
    {
        var utf = Encoding.UTF8.GetBytes(text);
        var mstr = new MemoryStream(utf);
        return new(client, mstr, endpointFile, TransportAction.Post, Common.Enums.TransportEndPoint.File);
    }

    public Guid Id { get; }

    private WcpClient _client;

    private Stream? _fileStream;

    private Common.Enums.TransportEndPoint _endPoint;

    private TransportAction _action;

    private string _endPointFilePath;

    private int _fragmentsCount;

    private int _lastFragmentIndex = 0;

    private const ushort BUFFER_SIZE = 54512;

    public TransportDataRequest Chandge(TransportDataResponse request)
    {
        if(request.Status == Common.Enums.TransportDataStatus.Ok)
        {
            
        }
        return null;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
