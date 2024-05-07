using System.Windows.Input;
using ASiNet.App.WCP.Models;

namespace ASiNet.App.WCP.Viewe;

public partial class SendDataPage : ContentPage
{
	public SendDataPage()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SendDataPage));

    public static readonly BindableProperty SendFilesProperty =
        BindableProperty.Create(nameof(SendFiles), typeof(ICommand), typeof(SendDataPage));

    public ICommand? Command
    {
        get { return (ICommand?)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public ICommand? SendFiles
    {
        get { return (ICommand?)GetValue(SendFilesProperty); }
        set { SetValue(SendFilesProperty, value); }
    }


    private async void Button_Pressed(object sender, EventArgs e)
    {
        var options = new PickOptions() { PickerTitle = "Select file", };
        var result = await FilePicker.Default.PickAsync(options);
        if (result is not null)
        {
            Command?.Execute(new FileSystemEntry(result.FullPath, true, false));
        }
    }
    private async void Button_Pressed_files(object sender, EventArgs e)
    {
        var options = new PickOptions() { PickerTitle = "Select files", };
        var result = await FilePicker.Default.PickMultipleAsync(options);
        if (result is not null)
        {
            SendFiles?.Execute(result.Select(x => new FileSystemEntry(x.FullPath, true, false)));
        }
    }
}