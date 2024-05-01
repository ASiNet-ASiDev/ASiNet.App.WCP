using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class YouTubeKeyboardViewModel : ObservableObject
{
    private WcpClient _client;
    public YouTubeKeyboardViewModel(WcpClient client)
    {
        _client = client;
    }

    [RelayCommand]
    private void PressedKey(KeyChandgeEvent keyResult)
    {
        _ = Task.Run(() =>
        {
            switch(keyResult.Code)
            {
                case KeyCode.YouTubePrevEpisode:
                    SendWithPresssed(KeyCode.LeftCtrl, KeyCode.ArrowLeft);
                    break;
                case KeyCode.YouTubeNextEpisode:
                    SendWithPresssed(KeyCode.LeftCtrl, KeyCode.ArrowRight);
                    break;

                case KeyCode.YouTubePrevVideo:
                    SendWithPresssed(KeyCode.LeftShift, KeyCode.P);
                    break;
                case KeyCode.YouTubeNextVideo:
                    SendWithPresssed(KeyCode.LeftShift, KeyCode.N);
                    break;

                case KeyCode.YouTubeSlowDown:
                    SendWithPresssed(KeyCode.LeftShift, KeyCode.OemComma);
                    break;
                case KeyCode.YouTubeSpeedUp:
                    SendWithPresssed(KeyCode.LeftShift, KeyCode.Separator);
                    break;

                default:
                    _client.SendKeyEvent(keyResult);
                    break;
            }
        });
    }

    private void SendWithPresssed(KeyCode pressedKey, KeyCode key)
    {
        _client.SendKeyEvent(new() { Code = pressedKey, State = KeyState.Down });
        _client.SendKeyEvent(new() { Code = key, State = KeyState.Click });
        _client.SendKeyEvent(new() { Code = pressedKey, State = KeyState.Up });
    }
}
