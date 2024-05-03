using System.Windows.Input;

namespace ASiNet.App.WCP.Viewe;

public partial class RDAPage : ContentPage
{
	public RDAPage()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty ChildCommandProperty =
            BindableProperty.Create(nameof(ChildCommand), typeof(ICommand), typeof(RDAPage));

    public ICommand ChildCommand
    {
        get => (ICommand)GetValue(ChildCommandProperty);
        set => SetValue(ChildCommandProperty, value);
    }
}