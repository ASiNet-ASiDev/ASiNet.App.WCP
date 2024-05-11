using System.Collections.ObjectModel;
using System.Windows.Input;
using ASiNet.App.WCP.VieweModels;

namespace ASiNet.App.WCP.Viewe;

public partial class ChatPage : ContentPage
{
	public ChatPage()
	{
		InitializeComponent();
        TextInputEditor.SizeChanged += OnTISzChanged;
	}

    public static readonly BindableProperty SendMessageCommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ChatPage));

    public static readonly BindableProperty LoadMessagesCommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ChatPage));

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Messages.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
        LoadMessagesCommand?.Execute(null);
    }

    public ICommand? SendMessageCommand
    {
        get { return (ICommand?)GetValue(SendMessageCommandProperty); }
        set { SetValue(SendMessageCommandProperty, value); }
    }

    public ICommand? LoadMessagesCommand
    {
        get { return (ICommand?)GetValue(LoadMessagesCommandProperty); }
        set { SetValue(LoadMessagesCommandProperty, value); }
    }

    private void LoadNew(object sender, EventArgs e)
    {
        if((LoadMessagesCommand?.CanExecute(null) ?? false))
        {
            Messages.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset;
            LoadMessagesCommand?.Execute(null);
        }
    }

    private void OnTISzChanged(object? sender, EventArgs e)
    {
        Messages.Margin = new(0, 0, 0, TextInputEditor.Height + 5);
    }

    private void SentTextPressed(object sender, EventArgs e)
    {
        if(TextInputEditor.Text != null)
        {
            Messages.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
            SendMessageCommand?.Execute(new ChatMessageVieweModel(TextInputEditor.Text, DateTime.Now));
            TextInputEditor.Text = null;

            //Messages.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset;
        }
    }
}