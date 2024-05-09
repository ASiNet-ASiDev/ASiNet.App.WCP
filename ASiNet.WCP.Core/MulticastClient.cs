using System.Net;
using System.Net.Sockets;
using ASiNet.Data.Serialization;
using ASiNet.Data.Serialization.Attributes;
using ASiNet.WCP.Common;

namespace ASiNet.WCP.Core;
public class MulticastClient : IDisposable
{
    public MulticastClient(int port, string address, int serverPort, int protocolVersion)
    {
        _serverPort = serverPort;
        _protocolVersion = protocolVersion;
        _address = IPAddress.Parse(address);
        _udp = new UdpClient(port)
        {
            MulticastLoopback = false
        };
    }

    public MulticastClient(int port, string address)
    {
        _port = port;
        _address = IPAddress.Parse(address);
        _udp = new UdpClient(port)
        {
            MulticastLoopback = false
        };
    }

    private IPAddress _address;
    private UdpClient _udp;
    private int _serverPort;
    private int _protocolVersion;
    private int _port;

    public async IAsyncEnumerable<RemoteServerInfo> FindServer(int timeout = 5_000)
    {
        var request = new MulticastRequest() { GetInfo = true };
        var data = new byte[BinarySerializer.GetSize(request)];
        BinarySerializer.Serialize(request, data);
        await _udp.SendAsync(data, data.Length, new IPEndPoint(_address, _port));
        var cts = new CancellationTokenSource();
        cts.CancelAfter(timeout);
        while (!cts.Token.IsCancellationRequested)
        {
            MulticastResponse? package = null;
            var address = string.Empty;
            try
            {
                var res = await _udp.ReceiveAsync(cts.Token);
                address = res.RemoteEndPoint.Address.ToString();
                package = BinarySerializer.Deserialize<MulticastResponse>(res.Buffer);
            }
            catch { }
            if (package is not null)
            {
                yield return new(package.ServerName, package.ServerPort, address, package.ProtocolVersion)
                { 
                    IncorrectProtocolVersion = WCPProtocolVersion.VERSION != package.ProtocolVersion,
                };
            }
        }
        yield break;
    }


    public async Task WaitUser(CancellationToken token)
    {
        try
        {
            _udp.JoinMulticastGroup(_address);
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var request = await _udp.ReceiveAsync(token);
                    var package = BinarySerializer.Deserialize<MulticastRequest>(request.Buffer);
                    if (package is not null && package.GetInfo)
                    {
                        var response = new MulticastResponse()
                        {
                            ServerName = Environment.MachineName,
                            ServerPort = _serverPort,
                            ProtocolVersion = _protocolVersion,
                        };
                        var data = new byte[BinarySerializer.GetSize(response)];
                        BinarySerializer.Serialize(response, data);
                        _udp.Send(data, data.Length, request.RemoteEndPoint);
                    }
                }
                catch { }
            }
        }
        catch { }
        finally
        {
            _udp.DropMulticastGroup(_address);
        }
    }

    public void Dispose()
    {
        _udp.Dispose();
    }
}

[PreGenerate]
internal class MulticastRequest()
{
    public bool GetInfo { get; set; }
}

[PreGenerate]
internal class MulticastResponse()
{
    public string? ServerName { get; set; }
    public int ServerPort { get; set; }

    public int ProtocolVersion { get; set; }
}