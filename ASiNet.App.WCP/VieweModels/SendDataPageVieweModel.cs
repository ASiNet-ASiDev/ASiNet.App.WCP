using ASiNet.App.WCP.Models;
using ASiNet.App.WCP.Viewe;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class SendDataPageVieweModel : ObservableObject
{
    public SendDataPageVieweModel(WcpClient client)
    {
        _client = client;
        _manager = ((ShellVieweModel)Shell.Current.BindingContext).TaskManager;
        _mediaManager = _client.MediaManager;
    }

    [ObservableProperty]
    private string? _localDirectory;

    [ObservableProperty]
    private string? _localFileName;

    [ObservableProperty]
    private string? _remoteFileName;

    [ObservableProperty]
    private bool _fail;

    [ObservableProperty]
    private TaskManagerVieweModel _manager;

    private WcpClient _client;
    private MediaManager _mediaManager;

    [RelayCommand]
    private void SetLocalFilePath(FileSystemEntry? entry)
    {
        if (entry is null)
        {
            LocalDirectory = null;
            LocalFileName = null;
        }
        else
        {
            if (entry.IsFile)
            {
                LocalDirectory = entry.Directory;
                LocalFileName = entry.Name;
                RemoteFileName = entry.Name;
            }
        }
    }

    [RelayCommand]
    private async Task Send()
    {
        Fail = false;
        if (LocalFileName is null || LocalDirectory is null || RemoteFileName is null)
        {
            Fail = true;
            return;
        }
        var localFilePath = Path.Join(LocalDirectory, LocalFileName);
        var result = _mediaManager.RunNew(null, RemoteFileName, localFilePath, ASiNet.WCP.Common.Enums.MediaAction.Post);
        if(!result)
        {
            Fail = true;
            return;
        }
        Fail = false;
    }

    [RelayCommand]
    private async Task SendFile(IEnumerable<FileSystemEntry> entrys)
    {
        await Task.Run(() =>
        {
            foreach (var entry in entrys)
            {
                if (entry.Name is null || entry.Path is null)
                    return;
                _mediaManager.RunNew(null, entry.Name!, entry.Path!, ASiNet.WCP.Common.Enums.MediaAction.Post);
            }
        });
    }
}
