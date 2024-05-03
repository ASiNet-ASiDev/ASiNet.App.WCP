using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Common.DataTransport;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.DesktopService;
public class TransportEndPointsManadgerServer
{

    private Dictionary<Guid, ITransportEndPoint> _transportEndpoints = [];

    public TransportDataResponse? Chandge(TransportDataRequest request)
    {
        if(request.Status == Common.Enums.TransportDataStatus.Ok)
        {
            if (request.Action.HasFlag(Common.Enums.TransportAction.Create))
            {
                var endPoint = new TransportEndPointServer(request.OperationId);
                var response = endPoint.Chandge(request);
                if (response is null)
                {
                    _transportEndpoints.Remove(request.OperationId);
                    return new()
                    {
                        OperationId = request.OperationId,
                        Status = Common.Enums.TransportDataStatus.OperationClosed
                    };
                }
                return response;
            }
            else if (_transportEndpoints.TryGetValue(request.OperationId, out var endPoint))
            {
                if (request.Status == Common.Enums.TransportDataStatus.OperationClosed)
                {
                    endPoint.Dispose();
                    _transportEndpoints.Remove(request.OperationId);
                }
                if (endPoint.IsDisposed)
                {
                    _transportEndpoints?.Remove(endPoint.Id);
                    return new()
                    {
                        OperationId = request.OperationId,
                        Status = Common.Enums.TransportDataStatus.OperationClosed
                    };
                }
                var response = endPoint.Chandge(request);
                if (response is null)
                {
                    _transportEndpoints.Remove(request.OperationId);
                    return new()
                    {
                        OperationId = request.OperationId,
                        Status = Common.Enums.TransportDataStatus.OperationClosed
                    };
                }
                return response;
            }
            else
            {
                return new()
                {
                    OperationId = request.OperationId,
                    Status = Common.Enums.TransportDataStatus.OperationNotFound
                };
            }
        }
        else if(request.Status == Common.Enums.TransportDataStatus.OperationClosed)
        {
            if (_transportEndpoints.TryGetValue(request.OperationId, out var endPoint))
                endPoint?.Dispose();
            _transportEndpoints.Remove(request.OperationId);
            return null;
        }
        return null;
    }

    public void Dispose()
    {
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
