using ASiNet.App.WCP.Models.Enums;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class KeyboardVieweModel : ObservableObject
{
    public KeyboardVieweModel(WcpClient client)
    {
        _client = client;
        _ = GetLanguage();
    }

    [ObservableProperty]
    private KeyCapVisualStatus _kcStatus = KeyCapVisualStatus.Default;

    private WcpClient _client;

    [RelayCommand]
    private void PressedKey(KeyChandgeEvent keyResult)
    {
        ChangeStatus(keyResult);
        _ = Task.Run(() =>
        {
            _client.SendKeyEvent(keyResult);
        });
    }

    [RelayCommand]
    private void Reverse()
    {
        try
        {
            var ro = !App.Config.ReverseOrientation;
            OrientationService.Current.SetOrientation(DisplayOrientation.Landscape, ro);
            App.Config.ReverseOrientation = ro;
            App.Config.SaveOrUpdate();
        }
        catch (Exception)
        {

            throw;
        }
    }

    [RelayCommand]
    private async Task SwitchLanguage(KeyChandgeEvent keyResult)
    {
        switch (KcStatus)
        {
            case KeyCapVisualStatus.Default:
                if (await _client.SetLanguage(LanguageCode.RussianRU))
                {
                    Shell.Current.Dispatcher.Dispatch(() =>
                    {
                        KcStatus = KeyCapVisualStatus.Alt;
                    });
                }
                break;
            case KeyCapVisualStatus.ShiftDefault:
                if (await _client.SetLanguage(LanguageCode.RussianRU))
                {
                    Shell.Current.Dispatcher.Dispatch(() =>
                    {
                        KcStatus = KeyCapVisualStatus.ShiftAlt;
                    });
                }
                break;
            case KeyCapVisualStatus.Alt:
                if (await _client.SetLanguage(LanguageCode.EnglishUS))
                {
                    Shell.Current.Dispatcher.Dispatch(() =>
                    {
                        KcStatus = KeyCapVisualStatus.Default;
                    });
                }
                break;
            case KeyCapVisualStatus.ShiftAlt:
                if (await _client.SetLanguage(LanguageCode.EnglishUS))
                {
                    Shell.Current.Dispatcher.Dispatch(() =>
                    {
                        KcStatus = KeyCapVisualStatus.ShiftDefault;
                    });
                }
                break;
        }
    }

    [RelayCommand]
    private async Task ClosePage(KeyChandgeEvent keyResult)
    {
        await Shell.Current.Navigation.PopModalAsync();
    }

    private async Task GetLanguage()
    {
        switch (await _client.GetLanguage())
        {
            case LanguageCode.EnglishUS:
                Shell.Current.Dispatcher.Dispatch(() =>
                {
                    KcStatus = KeyCapVisualStatus.Default;
                });
                break;
            case LanguageCode.RussianRU:
                Shell.Current.Dispatcher.Dispatch(() =>
                {
                    KcStatus = KeyCapVisualStatus.Alt;
                });
                break;
        }
    }

    private void ChangeStatus(KeyChandgeEvent keyResult)
    {
        if (keyResult.Code != KeyCode.Shift)
            return;
        switch (KcStatus)
        {
            case KeyCapVisualStatus.Default or KeyCapVisualStatus.ShiftDefault:
                if (keyResult.State == KeyState.Down)
                    KcStatus = KeyCapVisualStatus.ShiftDefault;
                else if (keyResult.State == KeyState.Up)
                    KcStatus = KeyCapVisualStatus.Default;
                break;
            case KeyCapVisualStatus.Alt or KeyCapVisualStatus.ShiftAlt:
                if (keyResult.State == KeyState.Down)
                    KcStatus = KeyCapVisualStatus.ShiftAlt;
                else if (keyResult.State == KeyState.Up)
                    KcStatus = KeyCapVisualStatus.Alt;
                break;
        }
    }
}
