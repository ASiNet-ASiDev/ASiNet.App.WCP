using ASiNet.App.WCP.Models;
using ASiNet.App.WCP.Viewe;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class AcceptDataPageVieweModel : ObservableObject, IRdaInvoker
{

    public AcceptDataPageVieweModel()
    {
        _client = ((ShellVieweModel)Shell.Current.BindingContext).WcpClient;
        _mediaManager = _client.MediaManager;
    }


    [ObservableProperty]
    private string? _localFilePath;

    [ObservableProperty]
    private string? _remoteFilePath;

    [ObservableProperty]
    private bool _fail;

    private WcpClient _client;
    private MediaManager _mediaManager;

    [RelayCommand]
    private async Task RemoteDirectoryAcces()
    {
        await Shell.Current.Navigation.PushModalAsync(new RDAPage() { BindingContext = new RDAPageVieweModel(_client, this) });
    }

    public void RdaResult(FileSystemEntry? entry)
    {
        if (entry is null)
        {
            RemoteFilePath = null;
        }
        else
        {
            if (entry.IsFile)
            {
                RemoteFilePath = entry.Path;
            }
        }
    }


    [RelayCommand]
    private async Task Accept()
    {
        //Fail = false;
        //if (LocalFilePath is null || RemoteFilePath is null)
        //{
        //    Fail = true;
        //    return;
        //}
        //var result = await _mediaManager.RunNew(RemoteFilePath, LocalFilePath, ASiNet.WCP.Common.Enums.MediaAction.Get);
        //if (!result)
        //{
        //    Fail = true;
        //    return;
        //}
        //Fail = false;
    }
}
