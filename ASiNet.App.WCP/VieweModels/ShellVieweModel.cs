using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ASiNet.App.WCP.VieweModels;
public partial class ShellVieweModel : ObservableObject
{
    public ShellVieweModel()
    {
        WcpClient = new();
        TaskManager = new(WcpClient);
    }

    public TaskManagerVieweModel TaskManager;

    public WcpClient WcpClient { get; } = new();
}
