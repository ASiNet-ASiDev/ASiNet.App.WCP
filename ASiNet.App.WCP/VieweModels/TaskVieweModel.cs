using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ASiNet.App.WCP.VieweModels;

public partial class TaskVieweModel : ObservableObject, IDisposable
{
    public TaskVieweModel(string? fileName, int id, MediaAction action, MediaManager manager)
    {
        Action = action;
        FileName = fileName;
        Id = id;
        _mediaManager = manager;
        _mediaManager.TaskChanged += OnTaskChanged;
    }

    public int Id { get; set; }

    private MediaManager _mediaManager;

    [ObservableProperty]
    private double _progress;
    [ObservableProperty]
    private string? _FileName;
    [ObservableProperty]
    private MediaAction _action;

    [ObservableProperty]
    private string? _status;

    public void OnTaskChanged(MediaTask task)
    {
        if(task.Id != Id)
            return;
        switch (task.TaskAction)
        {
            case MediaTaskStatus.Waiting:
                FileName = task.FileName;
                Status = "Waiting...";
                break;
            case MediaTaskStatus.Created:
                FileName = task.FileName;
                Status = "Loading...";
                break;
            case MediaTaskStatus.Working:
                if(task.Progress.HasValue)
                    Progress = task.Progress.Value;
                if(task.ClientStatus.HasValue)
                {
                    switch (task.ClientStatus.Value)
                    {
                        case MediaClientStatus.StartOk:
                            Status = "Working...";
                            break;
                        case MediaClientStatus.StartFailed:
                            Status = "Operation failed.";
                            break;
                        case MediaClientStatus.FinishOk:
                            Status = "Operation Ok.";
                            break;
                        case MediaClientStatus.FinishFailed:
                            Status = "Operation failed.";
                            break;
                        case MediaClientStatus.Failed:
                            Status = "Operation failed.";
                            break;
                    }
                }
                break;
            case MediaTaskStatus.Removed:

                _mediaManager.TaskChanged -= OnTaskChanged;
                break;
        }
    }

    public void Dispose()
    {
        _mediaManager.TaskChanged -= OnTaskChanged;
    }
}