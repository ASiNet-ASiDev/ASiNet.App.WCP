using System.Net;
using System.Net.Sockets;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Core;

namespace ASiNet.WCP.DesktopService;
public class MediaManager : IDisposable
{
    public MediaManager(ServerClient client)
    {
        _client = client;
        _port = client.Config.MediaPort;
        _mediaClientsCount = client.Config.MediaClientCount;
        _filesDirectory = client.Config.FilesDirectory;
        _connectionTimeout = client.Config.MediaConnectionTimeout;
        _mediaListener = new(IPAddress.Any, _port);
        _mediaListener.Start();
    }

    private ServerClient _client;
    private int _port;

    private int _mediaClientsCount;

    private int _connectionTimeout;

    private string _filesDirectory;

    private List<MediaClient> _mediaClients = [];

    private readonly object _locker = new();

    private TcpListener _mediaListener;

    public MediaResponse Change(MediaRequest request)
    {
        FreeClients();

        if (_mediaClients.Count >= _mediaClientsCount)
        {
            return new() { Status = MediaStatus.WorkingAll };
        }

        var filePath = string.Empty;
        if (request.DirectoryPath is null)
        {
            if (!Directory.Exists(_client.Config.FilesDirectory))
                Directory.CreateDirectory(_client.Config.FilesDirectory);
            filePath = Path.Join(_filesDirectory, request.FileName!);
        }
        else
            filePath = Path.Join(request.DirectoryPath, request.FileName);


        _ = WaitClient(filePath, request.Action);

        return new()
        {
            Port = _port,
            Address = _client.Address,
            Status = MediaStatus.Ok
        };
    }

    private async Task WaitClient(string path, MediaAction media)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(_connectionTimeout);
        try
        {
            var client = await _mediaListener.AcceptTcpClientAsync(cts.Token);
            lock (_locker)
            {
                var mediaClient = new MediaClient(client, _port, path, media);
                _mediaClients.Add(mediaClient);
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {

        }

    }

    private void FreeClients()
    {
        try
        {
            lock (_locker)
            {
                var clients = _mediaClients.Where(x => x.Failed || x.Disposed).ToList();
                foreach (var item in clients)
                {
                    if (!item.Disposed)
                        item.Dispose();
                    _mediaClients.Remove(item);
                }
            }
        }
        catch { }
    }

    public void Dispose()
    {
        _mediaListener.Stop();
        _mediaListener.Dispose();
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
