using System.Collections.ObjectModel;
using System.ComponentModel;
using ASiNet.App.WCP.Models;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class RDAPageVieweModel : ObservableObject
{
    public RDAPageVieweModel(WcpClient client, IRdaInvoker? invoker)
    {
        _invoker = invoker;
        _client = client;
        _ = Init();
    }

    public ObservableCollection<FileSystemEntry> Entrys { get; } = [];

    [ObservableProperty]
    private FileSystemEntry? _root;

    private WcpClient _client;

    private IRdaInvoker? _invoker;

    [RelayCommand]
    private async Task Select()
    {
        _invoker?.RdaResult(Root);
        await Shell.Current.Navigation.PopModalAsync();
    }

    [RelayCommand]
    private async Task Close()
    {
        _invoker?.RdaResult(null);
        await Shell.Current.Navigation.PopModalAsync();
    }

    [RelayCommand]
    private async Task UpDirectories()
    {
        if (!string.IsNullOrEmpty(Root?.Path) && Path.GetDirectoryName(Root.Path!) is string upPath)
        {
            Root = CreateEntry(upPath, false, true);
            var (dirs, files, status) = await _client.GetFileSystemEntris(Root.Path);
            if (status == GetDirectiryStatus.Success)
                UpdateEntrys(dirs, files);
        }
        else
            await Init();
    }

    [RelayCommand]
    private async Task GetDirectories(FileSystemEntry root)
    {
        if(root.IsFile)
        {
            Root = root;
            return;
        }
        var (dirs, files, status) = await _client.GetFileSystemEntris(root.Path);
        Root = root;
        if (status == GetDirectiryStatus.Success)
            UpdateEntrys(dirs, files);
    }

    private async Task Init()
    {
        var (dirs, status) = await _client.GetFileSystemRoots();
        if (status == GetDirectiryStatus.Success)
            UpdateEntrys(dirs, null);
        Root = Entrys.FirstOrDefault();
    }

    private void UpdateEntrys(string[]? directories, string[]? files)
    {
        Entrys.Clear();
        if(directories is not null)
            foreach (var item in directories)
                Entrys.Add(CreateEntry(item, false, true));
        if (files is not null)
            foreach (var item in files)
                Entrys.Add(CreateEntry(item, true, false));
    }

    private FileSystemEntry CreateEntry(string path, bool isFile, bool isDirectory)
    {
        return new(path, isFile, isDirectory);
    }

}
