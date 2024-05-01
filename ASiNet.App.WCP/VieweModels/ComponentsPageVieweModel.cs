﻿using System.Collections.ObjectModel;
using ASiNet.App.WCP.Viewe.Controls;
using ASiNet.WCP.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ASiNet.App.WCP.VieweModels;
public partial class ComponentsPageVieweModel : ObservableObject
{
    public ComponentsPageVieweModel()
    {
        _client = ((ShellVieweModel)Shell.Current.BindingContext).WcpClient;

        KeyboardVM = new(_client);
        MouseVM = new(_client);

        var enruKeyboard = new KeyboardItemVieweModel<RuEnKeyboard>(_keyboardVM)
        {
            Title = Resources.Localization.AppResources.cmp_default_ruen_keyboard_title,
            Description = Resources.Localization.AppResources.cmp_default_ruen_keyboard_description,
            Author = Resources.Localization.AppResources.cmp_default_ruen_keyboard_author,
        };
        Components.Add(enruKeyboard);
        var joymouseKeyboard = new MouseItemVieweModel<Joystick>(_mouseVM)
        {
            Title = Resources.Localization.AppResources.cmp_default_mouse_title,
            Description = Resources.Localization.AppResources.cmp_default_mouse_description,
            Author = Resources.Localization.AppResources.cmp_default_mouse_author,
        };
        Components.Add(joymouseKeyboard);
    }

    public ObservableCollection<ComponentVieweModel> Components { get; } = [];

    [ObservableProperty]
    private KeyboardVieweModel _keyboardVM;
    [ObservableProperty]
    private MouseVieweModel _mouseVM;

    private WcpClient _client;
}