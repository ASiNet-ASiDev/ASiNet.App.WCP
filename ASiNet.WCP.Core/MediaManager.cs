using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Common.Enums;

namespace ASiNet.WCP.Core;
public class MediaManager : IDisposable
{

    private List<MediaClient> _mediaStreams = [];

    public MediaClient? RunNew(string address, int port, string path, MediaAction action)
    {
        try
        {
            var mediaClient = new MediaClient(address, port, path, action);
            _mediaStreams.Add(mediaClient);
            return mediaClient;
        }
        catch
        {
            return null;
        }
    }

    public IEnumerable<MediaTask> GetMediaTasks()
    {
        return _mediaStreams.Select(x => new MediaTask() 
        { 
            Id = x.Port, 
            TaskStatus = 
                x.Working ? MediaTaskStatus.Working : 
                x.Waiting ? MediaTaskStatus.Connecting : 
                x.Failed ? MediaTaskStatus.Failed : MediaTaskStatus.Stopped,
        });
    }

    public IEnumerable<MediaTask> Refresh()
    {
        var clients = _mediaStreams.Where(x => x.Failed || !x.Connected && !x.Waiting && !x.Working).ToList();
        foreach (var item in clients)
        {
            item.Dispose();
            _mediaStreams.Remove(item);
        }

        return _mediaStreams.Select(x => new MediaTask() { Id = x.Port, TaskStatus = x.Working ? MediaTaskStatus.Working : MediaTaskStatus.Stopped });
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
    public MediaTaskStatus TaskStatus { get; set; }
}

public enum MediaTaskStatus
{
    Connecting,
    Working,
    Stopped,
    Failed,
}
