using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.Core;
public class TransportEndPointsManadger(WcpClient client)
{

    private Dictionary<Guid, TransportEndPoint> _transportEndpoints = [];

    private WcpClient _client = client;

    private CancellationTokenSource? _cts;

    public bool CreatePostFile(FileStream file, string endpointFile)
    {
        try
        {
            StartUpdater();
            var endPoint = TransportEndPoint.PostFile(_client, file, endpointFile);
            _transportEndpoints.Add(endPoint.Id, endPoint);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreatePostTextAsFile(string text, string endpointFile)
    {
        try
        {
            StartUpdater();
            var endPoint = TransportEndPoint.PostTextAsFile(_client, text, endpointFile);
            _transportEndpoints.Add(endPoint.Id, endPoint);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public TransportDataRequest Chandge(TransportDataResponse request)
    {
        if (_transportEndpoints.TryGetValue(request.OperationId, out var endPoint))
        {
            var pack = endPoint.Chandge(request);
            if (pack is null)
            {
                endPoint.Dispose();
                _transportEndpoints?.Remove(request.OperationId);
                StopUpdater();
                return new() { Status = Common.Enums.TransportDataStatus.OperationClosed };
            }
            return pack;
        }
        else
        {
            StopUpdater();
            return new()
            {
                OperationId = request.OperationId,
                Status = Common.Enums.TransportDataStatus.OperationNotFound
            };
        }
    }


    private void StartUpdater()
    {
        if (!(_cts?.IsCancellationRequested) ?? false)
        {
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
            }
            catch { }
            _cts = new CancellationTokenSource();
            _client?.TransportUpdater(_cts.Token);
        }
    }

    private void StopUpdater()
    {
        if (_transportEndpoints.Count > 0)
            return;
        if (!(_cts?.IsCancellationRequested) ?? false)
        {
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
            }
            catch { }
        }
    }

    public void Dispose()
    {
        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
        catch { }
        foreach (var item in _transportEndpoints.Values)
        {
            try
            {
                item.Dispose();
            }
            catch (Exception)
            {

            }
        }
    }
}
