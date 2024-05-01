using System.Windows.Input;
using ASiNet.App.WCP.Models.Enums;

namespace ASiNet.App.WCP.Viewe;

public partial class MacrosBuilder : ContentPage
{
	public MacrosBuilder()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty AddCommandProperty =
            BindableProperty.Create(nameof(AddCommand), typeof(ICommand), typeof(MacrosBuilder));

    public ICommand? AddCommand
    {
        get { return (ICommand?)GetValue(AddCommandProperty); }
        set { SetValue(AddCommandProperty, value); }
    }

    private async void Button_Pressed(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("Action type:", "Cancel", null, "Keyboard", "Mouse", "Language");
        AddCommand?.Execute(action switch
        {
            "Keyboard" => MacrosAction.Keyboard,
            "Mouse" => MacrosAction.Mouse,
            "Language" => MacrosAction.Language,
            _ => MacrosAction.None,
        });
    }
}