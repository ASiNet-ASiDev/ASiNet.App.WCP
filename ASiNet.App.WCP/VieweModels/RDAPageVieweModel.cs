using System.Collections.ObjectModel;
using ASiNet.App.WCP.Models;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class RDAPageVieweModel : ObservableObject
{
    public RDAPageVieweModel()
    {
        _client = ((ShellVieweModel)Shell.Current.BindingContext).WcpClient;
        _ = Init();
    }

    public RDAPageVieweModel(WcpClient client)
    {
        _client = client;
        _ = Init();
    }

    public ObservableCollection<FileSystemEntry> Directories { get; } = [];

    [ObservableProperty]
    private string? _root;

    private WcpClient _client;

    [RelayCommand]
    private async Task UpDirectories()
    {
        if (string.IsNullOrEmpty(Root))
            return;
        Root = Path.GetDirectoryName(Root);
        var result = await _client.GetDirectories(Root);
        if (result.Status == GetDirectiryStatus.Success)
        {
            Directories.Clear();
            foreach (var item in result.Dirs!)
                Directories.Add(GetEntry(item));
        }
    }

    [RelayCommand]
    private async Task GetDirectories(string root)
    {
        var result = await _client.GetDirectories(root);
        Root = root;
        if (result.Status == GetDirectiryStatus.Success)
        {
            Directories.Clear();
            foreach (var item in result.Dirs!)
                Directories.Add(GetEntry(item));
        }
    }

    private async Task Init()
    {
        var result = await _client.GetDirectories(null);
        if (result.Status == GetDirectiryStatus.Success)
        {
            Directories.Clear();
            foreach (var item in result.Dirs!)
                Directories.Add(GetEntry(item));
        }
    }

    private FileSystemEntry GetEntry(string path)
    {
        var name = Path.GetFileName(path);
        if (string.IsNullOrEmpty(name))
            name = path;
        return new() { Name = name, Path = path };
    }
}
