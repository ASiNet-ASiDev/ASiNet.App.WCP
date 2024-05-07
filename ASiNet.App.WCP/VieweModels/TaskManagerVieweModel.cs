using System.Collections.ObjectModel;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class TaskManagerVieweModel : ObservableObject
{
    public TaskManagerVieweModel(WcpClient client)
    {
        _client = client;
        _mediaManager = _client.MediaManager;
        _mediaManager.TaskChanged += OnTaskChanged;
    }

    public ObservableCollection<TaskVieweModel> Tasks { get; } = [];

    private WcpClient _client;
    private MediaManager _mediaManager;

    private void OnTaskChanged(MediaTask task)
    {
        switch (task.TaskAction)
        {
            case MediaTaskStatus.Waiting:
                var tvm = new TaskVieweModel(task.FileName, task.Id, task.Action, _mediaManager);
                tvm.OnTaskChanged(task);
                Tasks.Add(tvm);
                break;
            default:
                if(Tasks.FirstOrDefault(x => task.Id == x.Id) is null)
                {
                    tvm = new TaskVieweModel(task.FileName, task.Id, task.Action, _mediaManager);
                    tvm.OnTaskChanged(task);
                    Tasks.Add(tvm);
                }
                break;
        }
    }

    [RelayCommand]
    private void ClearRecords()
    {
        foreach (var item in Tasks)
            item.Dispose();
        Tasks.Clear();
    }
}
