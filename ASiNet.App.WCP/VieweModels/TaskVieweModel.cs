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
                Status = Resources.Localization.AppResources.tm_waiting;
                break;
            case MediaTaskStatus.Created:
                FileName = task.FileName;
                Status = Resources.Localization.AppResources.tm_created;
                break;
            case MediaTaskStatus.Working:
                if(task.Progress.HasValue)
                    Progress = task.Progress.Value;
                if(task.ClientStatus.HasValue)
                {
                    switch (task.ClientStatus.Value)
                    {
                        case MediaClientStatus.StartOk:
                            Status = Resources.Localization.AppResources.tm_status_start_ok;
                            break;
                        case MediaClientStatus.StartFailed:
                            Status = Resources.Localization.AppResources.tm_status_start_failed;
                            break;
                        case MediaClientStatus.FinishOk:
                            Status = Resources.Localization.AppResources.tm_status_finish_ok;
                            break;
                        case MediaClientStatus.FinishFailed:
                            Status = Resources.Localization.AppResources.tm_status_finish_failed;
                            break;
                        case MediaClientStatus.Failed:
                            Status = Resources.Localization.AppResources.tm_status_failed;
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