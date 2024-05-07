using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.Core;
public class MediaManager(WcpClient client) : IDisposable
{

    internal class MediaTaskQueueItem(int id, string? remoteDir, string remoteFileName, string localPath, MediaAction action)
    {
        public int Id { get; set; } = id;

        public string? RemoteDirectory { get; set; } = remoteDir;

        public string RemoteFileName { get; set; } = remoteFileName;

        public string LocalFilePath { get; set; } = localPath;

        public MediaAction Action { get; set; } = action;
    }

    public event Action<MediaTask>? TaskChanged;

    private List<MediaClient> _mediaStreams = [];

    private WcpClient _client = client;

    private Queue<MediaTaskQueueItem> _taskQueue = [];

    private int _lastId;

    private readonly object _idLocker = new();
    private readonly object _autorunerLocker = new();

    private bool _autorunerWork = false;

    public bool RunNew(string? remoteDirectoryPath, string remoteFileName, string localPath, MediaAction action)
    {
        try
        {
            var taskId = 0;
            lock (_idLocker)
            {
                _lastId++;
                taskId = _lastId;
            }
            TaskChanged?.Invoke(new()
            {
                Id = taskId,
                TaskAction = MediaTaskStatus.Waiting,
                FileName = Path.GetFileName(localPath),
                Action = action
            });
            lock (_autorunerLocker)
            {
                _taskQueue.Enqueue(new(taskId, remoteDirectoryPath, remoteFileName, localPath, action));
                if (!_autorunerWork)
                    _ = QuequAutoruner();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task QuequAutoruner()
    {
        lock (_autorunerLocker)
            _autorunerWork = true;
        try
        {
            while (_taskQueue.Count > 0)
            {
                var task = _taskQueue.Peek();
                var request = new MediaRequest()
                {
                    Action = task.Action switch
                    {
                        MediaAction.Get => MediaAction.Post,
                        MediaAction.Post => MediaAction.Get,
                        _ => throw new NotImplementedException(),
                    },
                    DirectoryPath = task.RemoteDirectory,
                    FileName = task.RemoteFileName,
                };
                var response = await _client.SendAndAccept<MediaRequest, MediaResponse>(request);
                if (response is null)
                    continue;
                if (response.Status == MediaStatus.Ok)
                {
                    CreateTask(task.Id, response.Address!, response.Port, task.LocalFilePath, task.Action);
                    lock (_autorunerLocker)
                    {
                        _taskQueue.Dequeue();
                    }
                }
                else if (response.Status == MediaStatus.WorkingAll)
                    await Task.Delay(500);
                else
                {
                    lock (_autorunerLocker)
                    {
                        _taskQueue.Dequeue();
                    }
                }
            }
        }
        catch { }
        finally
        {
            lock (_autorunerLocker)
                _autorunerWork = false;
        }
    }

    private MediaClient CreateTask(int taskId, string address, int port, string localpath, MediaAction action)
    {
        var mediaClient = new MediaClient(taskId, this, address, port, localpath, action);
        _mediaStreams.Add(mediaClient);
        TaskChanged?.Invoke(new()
        {
            Id = mediaClient.Id,
            TaskAction = MediaTaskStatus.Created,
            FileName = Path.GetFileName(localpath),
            Action = action
        });
        mediaClient.StatusChanged += OnClientStatusChanged;
        mediaClient.Changed += OnClientChanged;
        return mediaClient;
    }

    internal void ClosedClient(MediaClient client)
    {
        client.StatusChanged -= OnClientStatusChanged;
        client.Changed -= OnClientChanged;
        TaskChanged?.Invoke(new()
        {
            Id = client.Id,
            FileName = Path.GetFileName(client.FilePath),
            ClientStatus = client.Failed ? MediaClientStatus.FinishFailed : MediaClientStatus.FinishOk,
            TaskAction = MediaTaskStatus.Removed
        });
        _mediaStreams.Remove(client);
    }

    private void OnClientChanged(MediaClient client, long total, long proc)
    {
        TaskChanged?.Invoke(new()
        {
            Id = client.Id,
            TaskAction = MediaTaskStatus.Working, 
            ClientStatus = MediaClientStatus.StartOk,
            FileName = Path.GetFileName(client.FilePath),
            Progress = (double)proc / (double)total,
        });
    }

    private void OnClientStatusChanged(MediaClient client, MediaClientStatus status)
    {
        TaskChanged?.Invoke(new()
        {
            Id = client.Id,
            ClientStatus = status,
            FileName = Path.GetFileName(client.FilePath),
            TaskAction = MediaTaskStatus.Working
        });
    }

    public void Dispose()
    {
        foreach (var item in _mediaStreams)
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

public class MediaTask
{
    public int Id { get; set; }

    public string? FileName { get; set; }


    public MediaAction Action { get; set; }

    public MediaTaskStatus TaskAction { get; set; }

    public MediaClientStatus? ClientStatus { get; set; }

    public double? Progress { get; set; }
}

public enum MediaTaskStatus
{
    Waiting,
    Created,
    Working,
    Removed,
    Failed,
}
