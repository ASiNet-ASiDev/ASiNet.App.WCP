using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        _client  = client;
        _ = Init();
    }

    public ObservableCollection<string> Directories { get; } = [];

    [ObservableProperty]
    private string? _root;

    private WcpClient _client;

    [RelayCommand]
    private async Task GetDirectories(string root)
    {
        var result = await _client.GetDirectories(root);
        Root = root;
        if(result.Status == GetDirectiryStatus.Success)
        {
            Directories.Clear();
            foreach (var item in result.Dirs!)
                Directories.Add(item);
        }
    }

    private async Task Init()
    {
        var result = await _client.GetDirectories(null);
        if (result.Status == GetDirectiryStatus.Success)
        {
            Directories.Clear();
            foreach (var item in result.Dirs!)
                Directories.Add(item);
        }
    }

}
