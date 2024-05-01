using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Common.DataTransport;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.DesktopService;
public class TransportEndPointsManadger
{

    private Dictionary<Guid, ITransportEndPoint> _transportEndpoints = [];

    public TransportDataResponse Chandge(TransportDataRequest request)
    {
        if (request.Action.HasFlag(Common.Enums.TransportAction.Create))
        {
            var endPoint = new TransportEndPoint(request.OperationId);
            return endPoint.Chandge(request);
        }
        else if(_transportEndpoints.TryGetValue(request.OperationId, out var endPoint))
        {
            return endPoint.Chandge(request);
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
