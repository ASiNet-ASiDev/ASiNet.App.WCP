using ASiNet.WCP.Common.DataTransport;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.DesktopService;
internal class TransportEndPoint(Guid id) : ITransportEndPoint
{
    public Guid Id { get; } = id;


    private FileStream? _endPointFile;

    private int _fragmentsCount;

    private int _lastFragmentIndex = 0;

    private const ushort BUFFER_SIZE = 54512;

    private byte[]? _buffer;

    public TransportDataResponse Chandge(TransportDataRequest request)
    {
        if (request.Action.HasFlag(TransportAction.Post))
        {
            switch (request.EndPoint)
            {
                case Common.Enums.TransportEndPoint.Clipboard:
                    return new() { OperationId = request.OperationId, Status = TransportDataStatus.OperationNotSupported };
                    break;
                case Common.Enums.TransportEndPoint.File:
                    return FileDataPost(request);
            }
        }
        else if (request.Action.HasFlag(TransportAction.Get))
        {
            switch (request.EndPoint)
            {
                case Common.Enums.TransportEndPoint.Clipboard:
                    return new() { OperationId = request.OperationId, Status = TransportDataStatus.OperationNotSupported };
                break;
                case Common.Enums.TransportEndPoint.File:
                    return FileDataGet(request);
            }
        }
        return new() { OperationId = request.OperationId, Status = TransportDataStatus.OperationNotSupported };
    }

    private TransportDataResponse FileDataPost(TransportDataRequest request)
    {
        try
        {
            if (request.EndpointFilePath is null)
                return new() { OperationId = request.OperationId, Status = TransportDataStatus.FileNotFound };
            if (request.FragmentsCount == 1)
            {
                using(var file = File.Create(request.EndpointFilePath))
                    file.Write(request.Data);
                return new() { OperationId = request.OperationId, Status = TransportDataStatus.Ok };
            }

            _endPointFile ??= new(request.EndpointFilePath, FileMode.Create, FileAccess.Write);

            _endPointFile.Write(request.Data);
            _lastFragmentIndex = request.FragmentIndex;

            if(_lastFragmentIndex == request.FragmentsCount)
                Dispose();
            return new() { OperationId = request.OperationId, Status = TransportDataStatus.Ok };
        }
        catch (Exception)
        {
            return new() { OperationId = request.OperationId, Status = TransportDataStatus.Failed };
        }
    }

    private TransportDataResponse FileDataGet(TransportDataRequest request)
    {
        try
        {
            if (request.EndpointFilePath is null || !File.Exists(request.EndpointFilePath))
                return new() { OperationId = request.OperationId, Status = TransportDataStatus.FileNotFound };

            if(_endPointFile == null)
            {
                _endPointFile = new(request.EndpointFilePath, FileMode.Open, FileAccess.Read);
                _buffer = new byte[BUFFER_SIZE];
                _fragmentsCount = (int)Math.Ceiling((double)_endPointFile.Length / (double)BUFFER_SIZE);
            }

            var readedBytes = _endPointFile?.Read(_buffer);
            _lastFragmentIndex++;

            if(readedBytes is null)
            {

            }

            var pack = new TransportDataResponse()
            {
                OperationId = request.OperationId,
                Status = TransportDataStatus.Ok,
                FragmentsCount = _fragmentsCount,
                FragmentIndex = _lastFragmentIndex,
                Data = _buffer![0..readedBytes!.Value],
                TotalDataSize = _endPointFile!.Length
            };

            if (_lastFragmentIndex == _fragmentsCount)
                Dispose();
            return pack;
        }
        catch (Exception)
        {
            return new() { OperationId = request.OperationId, Status = TransportDataStatus.Failed };
        }
    }

    public void Dispose()
    {
        _endPointFile?.Dispose();
    }
}
