using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Core;

namespace ASiNet.WCP.DesktopService;
public class MediaManager(ServerClient client) : IDisposable
{
    private ServerClient _client = client;
    private int[] _ports = [44545];

    private List<MediaClient> _mediaClients = [];

    public MediaStreamResponse Change(MediaStreamRequest request)
    {
        FreeClients();
        var freePort = GetFreePort();
        if (!freePort.HasValue)
            return new() { Status = MediaStreamStatus.Failed };

        var mediaClient = new MediaClient(freePort.Value, request.Path!, request.Action);
        _mediaClients.Add(mediaClient);

        return new()
        {
            Port = freePort.Value,
            Address = _client.Address,
            Status = MediaStreamStatus.Ok
        };
    }


    private int? GetFreePort()
    {
        if (_mediaClients.Count == _ports.Length)
            return null;
        var freePort = _ports.FirstOrDefault(x => _mediaClients.FirstOrDefault(y => y.Port == x) is null);
        return freePort;
    }

    private void FreeClients()
    {
        try
        {
            var clients = _mediaClients.Where(x => x.Failed || !x.Connected && !x.Waiting && !x.Working).ToList();
            foreach (var item in clients)
            {
                item.Dispose();
                _mediaClients.Remove(item);
            }
        }
        catch { }
    }

    public void Dispose()
    {
        foreach (var item in _mediaClients)
        {
            try
            {
                item.Dispose();
            }
            catch
            {
                continue;
            }
        }
    }
}
