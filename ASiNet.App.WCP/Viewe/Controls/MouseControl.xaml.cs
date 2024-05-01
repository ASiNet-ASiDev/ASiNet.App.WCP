using System.Windows.Input;

namespace ASiNet.App.WCP.Viewe.Controls;

public partial class MouseControl : Border
{
	public MouseControl()
	{
		InitializeComponent();
    }

    public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MouseControl));

    private bool _pointerPressed;

    private Point _lastPos;

    private CancellationTokenSource? _cts;

    public ICommand? Command
    {
        get { return (ICommand?)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    private void PointerGestureRecognizer_PointerMoved(object sender, PointerEventArgs e)
    {
        if(_pointerPressed)
        {
            var pos = e.GetPosition(this);
            if(!pos.HasValue)
                return;
            var newX = ((pos.Value.X - Width / 2) / Width) * 2;
            var newY = ((pos.Value.Y - Height / 2) / Height) * 2; 
            if(newX is >= -0.7 and <= 0.7)
                PointerHandler.TranslationX = newX * 150;
            if(newY is >= -0.7 and <= 0.7)
                PointerHandler.TranslationY = newY * 150;
            _lastPos = new(newX, newY);
        }
    }

    private void PointerGestureRecognizer_PointerPressed(object sender, PointerEventArgs e)
    {
        if(_cts is not null && !_cts.IsCancellationRequested)
        {
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
            }
            catch { }
        }
        _cts = new();
        _ = Updater(_cts.Token);
        _pointerPressed = true;
    }

    private void PointerGestureRecognizer_PointerReleased(object sender, PointerEventArgs e)
    {
        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
        catch {}
        _pointerPressed = false;
        PointerHandler.TranslateTo(0, 0, 100);
    }

    private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
    {
        try
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
        catch { }
        _pointerPressed = false;
        PointerHandler.TranslateTo(0, 0, 100);
    }

    private async Task Updater(CancellationToken token)
    {
        await Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                if(_lastPos.X != 0 || _lastPos.Y != 0)
                    Shell.Current.Dispatcher.Dispatch(() => Command?.Execute(_lastPos));
                Task.Delay(20).Wait();
            }
        });
    }
}