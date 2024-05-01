using System.Runtime.CompilerServices;
using System.Windows.Input;
using ASiNet.App.WCP.Models;
using ASiNet.App.WCP.Models.Enums;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;
using CommunityToolkit.Maui.Extensions;

namespace ASiNet.App.WCP.Viewe.Controls;

public partial class Key : Border
{
    public Key()
    {
        //FontFamily = "OpenSansRegular";
        InitializeComponent();
        SetValue(CornerRadiusProperty, CornerRadius);
        SetValue(BackgroundColorProperty, ((SolidColorBrush)UpBakgroundColor).Color);
        SetValue(StrokeShapeProperty, CornerRadius);
        SetValue(Label.FontFamilyProperty, FontFamily);
    }

    public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Key));

    public static readonly BindableProperty KeyResultProperty =
            BindableProperty.Create(nameof(KeyResult), typeof(KeyboardKeyResult), typeof(Key));

    public static readonly BindableProperty KeyVisualStatusProperty =
            BindableProperty.Create(nameof(KeyVisualStatus), typeof(KeyCapVisualStatus), typeof(Key));

    public static readonly BindableProperty UpBakgroundColorProperty =
            BindableProperty.Create(nameof(UpBakgroundColor), typeof(Brush), typeof(Key));

    public static readonly BindableProperty PressedBakgroundColorProperty =
            BindableProperty.Create(nameof(PressedBakgroundColor), typeof(Brush), typeof(Key));

    public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Brush), typeof(Key));

    public static readonly BindableProperty PressedTextColorProperty =
            BindableProperty.Create(nameof(PressedTextColor), typeof(Brush), typeof(Key));

    public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(Key), 14.0D);

    public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(IShape), typeof(Key));

    public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(Key));



    private KeySupportedMode _mode;
    private KeyCode _code;


    private bool _isPressed;

    public ICommand? Command
    {
        get { return (ICommand?)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public string? FontFamily
    {
        get { return (string?)GetValue(FontFamilyProperty); }
        set { SetValue(FontFamilyProperty, value); }
    }

    public Brush UpBakgroundColor
    {
        get { return (Brush)GetValue(UpBakgroundColorProperty); }
        set { SetValue(UpBakgroundColorProperty, value); }
    }

    public Brush PressedBakgroundColor
    {
        get { return (Brush)GetValue(PressedBakgroundColorProperty); }
        set { SetValue(PressedBakgroundColorProperty, value); }
    }

    public Brush TextColor
    {
        get { return (Brush)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public IShape? CornerRadius
    {
        get { return (IShape?)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    public Brush PressedTextColor
    {
        get { return (Brush)GetValue(PressedTextColorProperty); }
        set { SetValue(PressedTextColorProperty, value); }
    }

    public KeyboardKeyResult? KeyResult
    {
        get { return (KeyboardKeyResult?)GetValue(KeyResultProperty); }
        set { SetValue(KeyResultProperty, value); }
    }

    public KeyCapVisualStatus? KeyVisualStatus
    {
        get { return (KeyCapVisualStatus?)GetValue(KeyVisualStatusProperty); }
        set { SetValue(KeyVisualStatusProperty, value); }
    }


    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        switch (_mode)
        {
            case KeySupportedMode.Default:

                _ = RunClickAnimation();
                Command?.Execute(new KeyChandgeEvent() { Code = _code, State = KeyState.Click });
                break;
            case KeySupportedMode.Hold:
                if (_isPressed)
                {
                    _isPressed = false;
                    _ = RunUpAnimation();
                    Command?.Execute(new KeyChandgeEvent() { Code = _code, State = KeyState.Up });
                }
                else
                {
                    _isPressed = true;
                    _ = RunDownAnimation();
                    Command?.Execute(new KeyChandgeEvent() { Code = _code, State = KeyState.Down });
                }
                break;
        }
    }

    private async Task RunClickAnimation()
    {
        await this.BackgroundColorTo(((SolidColorBrush)PressedBakgroundColor).Color, length: 100);
        await this.BackgroundColorTo(((SolidColorBrush)UpBakgroundColor).Color, length: 100);
    }

    private async Task RunDownAnimation()
    {
        await this.BackgroundColorTo(((SolidColorBrush)PressedBakgroundColor).Color, length: 100);
    }

    private async Task RunUpAnimation()
    {
        await this.BackgroundColorTo(((SolidColorBrush)UpBakgroundColor).Color, length: 100);
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(KeyVisualStatus) && KeyVisualStatus is not null)
        {
            Text.Text = KeyVisualStatus.Value switch
            {
                KeyCapVisualStatus.Default => KeyResult?.Name,
                KeyCapVisualStatus.ShiftDefault => KeyResult?.ShiftName,
                KeyCapVisualStatus.Alt => KeyResult?.AltName,
                KeyCapVisualStatus.ShiftAlt => KeyResult?.AltShiftName,
                _ => string.Empty
            };
        }
        else if (propertyName == nameof(KeyResult))
        {
            Text.Text = KeyResult?.Name;
            _mode = KeyResult?.Mode ?? KeySupportedMode.Default;
            _code = KeyResult?.KeyCode ?? KeyCode.None;
        }
        else if (propertyName == nameof(FontFamily))
        {
            Text.FontFamily = FontFamily;
        }
    }
}