using System.Collections.ObjectModel;
using ASiNet.App.WCP.Viewe;
using ASiNet.App.WCP.Viewe.Controls;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ASiNet.App.WCP.VieweModels;
public partial class ToolsPageVieweModel : ObservableObject
{
    public ToolsPageVieweModel()
    {
        _client = ((ShellVieweModel)Shell.Current.BindingContext).WcpClient;

        KeyboardVM = new(_client);
        YouTubeKeyboardVM = new(_client);
        MouseVM = new(_client);

        var youTubeKeyboard = new ToolVieweModel<YouTubeKeyboard, YouTubeKeyboardViewModel>(_youTubeKeyboardVM)
        {
            Title = Resources.Localization.AppResources.cmp_default_youtube_keyboard_title,
            Description = Resources.Localization.AppResources.cmp_default_youtube_keyboard_description,
            Author = Resources.Localization.AppResources.cmp_default_youtube_keyboard_author,
        };
        Tools.Add(youTubeKeyboard);

        var d = new ToolVieweModel<SendDataPage, SendDataPageVieweModel>(_sdpvm ??= new(_client))
        {
            Title = Resources.Localization.AppResources.cmp_send_file_title,
            Description = Resources.Localization.AppResources.cmp_send_file_description,
            Author = Resources.Localization.AppResources.cmp_send_file_author,
        };
        Tools.Add(d);

        var r = new ToolVieweModel<SendTextPage, SendTextPageVieweModel>(new(_client))
        {
            Title = Resources.Localization.AppResources.cmp_default_send_clipboard_Title,
            Description = Resources.Localization.AppResources.cmp_default_send_clipboard_description,
            Author = Resources.Localization.AppResources.cmp_default_send_clipboard_author,
        };

        var enruKeyboard = new ToolVieweModel<RuEnKeyboard, KeyboardVieweModel>(_keyboardVM)
        {
            Title = Resources.Localization.AppResources.cmp_default_ruen_keyboard_title,
            Description = Resources.Localization.AppResources.cmp_default_ruen_keyboard_description,
            Author = Resources.Localization.AppResources.cmp_default_ruen_keyboard_author,
        };
        Tools.Add(enruKeyboard);

        Tools.Add(r);



        //var joymouseKeyboard = new MouseItemVieweModel<MouseControl>(_mouseVM)
        //{
        //    Title = Resources.Localization.AppResources.cmp_default_mouse_title,
        //    Description = Resources.Localization.AppResources.cmp_default_mouse_description,
        //    Author = Resources.Localization.AppResources.cmp_default_mouse_author,
        //};
        //Components.Add(joymouseKeyboard);
    }

    public ObservableCollection<ToolVieweModel> Tools { get; } = [];

    [ObservableProperty]
    private KeyboardVieweModel _keyboardVM;
    [ObservableProperty]
    private YouTubeKeyboardViewModel _youTubeKeyboardVM;
    [ObservableProperty]
    private MouseVieweModel _mouseVM;

    private SendDataPageVieweModel _sdpvm;

    private WcpClient _client;
}
