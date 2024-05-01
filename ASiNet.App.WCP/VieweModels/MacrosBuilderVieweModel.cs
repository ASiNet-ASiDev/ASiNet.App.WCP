using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.App.WCP.Models.Enums;
using ASiNet.App.WCP.Viewe;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ASiNet.App.WCP.VieweModels;
public partial class MacrosBuilderVieweModel(MacrosesVieweModel parent) : ObservableObject
{
    [ObservableProperty]
    private string _name = $"My macros {Random.Shared.Next(999, 10000)}";
    [ObservableProperty]
    private string? _shortName;
    [ObservableProperty]
    private string? _description;
    [ObservableProperty]
    private string? _author;

    private ASiNet.WCP.Macroses.Core.MacrosBuilder _builder = new();

    private MacrosesVieweModel _parent = parent;

    [RelayCommand]
    private async Task AddAction(MacrosAction action)
    {

    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }

    [RelayCommand]
    private async Task Create()
    {
        _parent.AddMacros(_builder.BuildAsMacrosData());
        await Shell.Current.Navigation.PopModalAsync();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Name) : _builder.SetName(Name); break;
            case nameof(ShortName): _builder.SetShortName(ShortName); break;
            case nameof(Description): _builder.SetDescription(Description); break;
            case nameof(Author): _builder.SetAuthor(Author); break;
        }

        base.OnPropertyChanged(e);
    }
}
