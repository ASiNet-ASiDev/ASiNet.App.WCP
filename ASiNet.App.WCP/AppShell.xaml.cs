﻿using ASiNet.App.WCP.VieweModels;

namespace ASiNet.App.WCP;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        BindingContext = new ShellVieweModel();
    }

}