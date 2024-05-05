using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class SendDataPageVieweModel : ObservableObject
{
    public SendDataPageVieweModel()
    {
        _client = ((ShellVieweModel)Shell.Current.BindingContext).WcpClient;
        Unlocked = true;
    }


    [ObservableProperty]
    private string? _localFilePath;

    [ObservableProperty]
    private string? _remoteFilePath;

    [ObservableProperty]
    private bool _fail;
    
    [ObservableProperty]
    private bool _unlocked;
    [ObservableProperty]
    private long _totalSize;
    [ObservableProperty]
    private long _proccessingSize;
    [ObservableProperty]
    private double _progress;
    [ObservableProperty]
    private string? _status;

    private WcpClient _client;

    [RelayCommand]
    private async Task Send()
    {
        Fail = false;
        if (LocalFilePath is null || RemoteFilePath is null)
        {
            Fail = true;
            return;
        }
        Unlocked = false;
        var result = await _client.OpenMediaStream(RemoteFilePath, LocalFilePath, ASiNet.WCP.Common.Enums.MediaAction.Post);
        if(result is null)
        {
            Fail = true;
            Unlocked = true;
            return;
        }
        result.Changed += OnChandged;
        result.StatusChanged += OnStatusChandged;
        Fail = false;
    }


    private void OnChandged(long total, long proccesing)
    {
        TotalSize = total;
        ProccessingSize = proccesing;
        Progress = (double)proccesing / (double)total;
    }

    private void OnStatusChandged(MediaClientStatus status)
    {
        switch (status)
        {
            case MediaClientStatus.StartOk:
                Status = "Sending a file...";
                break;
            case MediaClientStatus.StartFailed:
                Status = "The file could not be sent.";
                break;
            case MediaClientStatus.FinishOk:
                Status = "The file was sent successfully.";
                ProccessingSize = 0;
                TotalSize = 0;
                Progress = 0;
                Unlocked = true;
                break;
            case MediaClientStatus.FinishFailed:
                Status = "The file could not be sent.";
                ProccessingSize = 0;
                TotalSize = 0;
                Progress = 0;
                Unlocked = true;
                break;
            case MediaClientStatus.Failed:
                Status = "The file could not be sent.";
                ProccessingSize = 0;
                TotalSize = 0;
                Progress = 0;
                Unlocked = true;
                break;
        }
    }
}
