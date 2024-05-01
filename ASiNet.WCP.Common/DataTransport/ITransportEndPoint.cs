using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.Common.DataTransport;
public interface ITransportEndPoint : IDisposable
{
    public Guid Id { get; }

    public TransportDataResponse Chandge(TransportDataRequest request);
}
