using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class MouseVieweModel(WcpClient client) : ObservableObject
{
    private WcpClient _client = client;

    [RelayCommand]
    private void MouseInput(Point point)
    {
        _client.SendMouseMoveEvent(new(point.X, point.Y) { SpeedMultiplier = 3 });
    }

}
