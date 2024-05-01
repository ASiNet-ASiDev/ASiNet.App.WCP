using System.Linq.Expressions;
using ASiNet.Data.Serialization;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Interfaces;
using ASiNet.WCP.Core.Macroses;

namespace ASiNet.WCP.Macroses.Core.Actions;

public delegate Task MacrosCompilationLambda(CancellationToken token = default);

public class Macros
{
    public Macros(MacrosData data, IVirtualMouse? virtualMouse, IVirtualKeyboard? virtualKeyboard, IVirtualKeyboardLayout? virtualKeyboardLayout)
    {
        _virtualMouse = virtualMouse;
        _virtualKeyboardLayout = virtualKeyboardLayout;
        _virtualKeyboard = virtualKeyboard;
        _data = data;
    }

    public Macros(byte[] binaryData, IVirtualMouse? virtualMouse, IVirtualKeyboard? virtualKeyboard, IVirtualKeyboardLayout? virtualKeyboardLayout)
    {
        _virtualMouse = virtualMouse;
        _virtualKeyboardLayout = virtualKeyboardLayout;
        _virtualKeyboard = virtualKeyboard;
        _data = BinarySerializer.Deserialize<MacrosData>(binaryData) ??
            throw new NullReferenceException("Macros file read error!");
    }

    public Macros(string filePath, IVirtualMouse? virtualMouse, IVirtualKeyboard? virtualKeyboard, IVirtualKeyboardLayout? virtualKeyboardLayout)
    {
        _virtualMouse = virtualMouse;
        _virtualKeyboardLayout = virtualKeyboardLayout;
        _virtualKeyboard = virtualKeyboard;
        using var file = File.OpenRead(filePath);
        _data = BinarySerializer.Deserialize<MacrosData>(file) ??
            throw new NullReferenceException("Macros file read error!");
    }

    private readonly IVirtualMouse? _virtualMouse;
    private readonly IVirtualKeyboardLayout? _virtualKeyboardLayout;
    private readonly IVirtualKeyboard? _virtualKeyboard;
    private readonly MacrosData _data;

    
    public void SaveAsFile(string fileName)
    {
        using var file = File.Create(fileName);
        BinarySerializer.Serialize(_data, file);
    }

    public byte[] SaveAsBinaryData(bool saveCode = true)
    {
        if(saveCode)
        {
            var size = BinarySerializer.GetSize(_data);
            var data = new byte[size];
            BinarySerializer.Serialize(_data, data);
            return data;
        }
        else
        {
            var macrosData = new MacrosData()
            {
                Name = _data.Name,
                ShortName = _data.ShortName,
                Description = _data.Description,
                Author = _data.Author,
            };
            var size = BinarySerializer.GetSize(macrosData);
            var data = new byte[size];
            BinarySerializer.Serialize(macrosData, data);
            return data;
        }
    }

    public MacrosData GetData(bool getCode = false)
    {
        var macrosData = new MacrosData()
        {
            Name = _data.Name,
            ShortName = _data.ShortName,
            Description = _data.Description,
            Author = _data.Author,
            Actions = getCode ? _data.Actions : null,
        };
        return macrosData;
    }


    public async Task<MacrosCompilationLambda> CompileAsync(CancellationToken token = default) =>
        await Task.Run(() => BuildLambda(token));

    public MacrosCompilationLambda Compile(CancellationToken token = default) =>
        BuildLambda(token);

    private MacrosCompilationLambda BuildLambda(CancellationToken token)
    {
        if (_data.Actions?.FirstOrDefault(x => x is KeyboardAction) is not null && _virtualKeyboard is null)
            throw new NullReferenceException("Virtual keyboard not seted!");
        if (_data.Actions?.FirstOrDefault(x => x is MouseAction) is not null && _virtualMouse is null)
            throw new NullReferenceException("Virtual mouse not seted!");
        if (_data.Actions?.FirstOrDefault(x => x is LanguageAction) is not null && _virtualKeyboardLayout is null)
            throw new NullReferenceException("Virtual keyboard layout not seted!");

        var tokenParameter = Expression.Parameter(typeof(CancellationToken), "cancelationToken");
        var keyboardParameter = Expression.Parameter(typeof(IVirtualKeyboard), "virtualKeyboard");
        var mouseParameter = Expression.Parameter(typeof(IVirtualMouse), "virtualMouse");
        var keyboardLayoutParameter = Expression.Parameter(typeof(IVirtualKeyboardLayout), "virtualKeyboardLayout");

        var body = new List<Expression>()
        {
            Expression.Call(tokenParameter, nameof(CancellationToken.ThrowIfCancellationRequested), null),
            Expression.Assign(keyboardParameter, Expression.Constant(_virtualKeyboard)),
            Expression.Assign(mouseParameter, Expression.Constant(_virtualMouse)),
            Expression.Assign(keyboardLayoutParameter, Expression.Constant(_virtualKeyboardLayout)),
        };

        body.AddRange(BuildBody(keyboardParameter, mouseParameter, keyboardLayoutParameter, tokenParameter, token));


        var block = Expression.Block([keyboardParameter], body);

        var coreLambda = Expression.Lambda<Action>(block);

        var lambda = Expression.Lambda<MacrosCompilationLambda>(Expression.Call(typeof(Task), nameof(Task.Run), null, coreLambda), tokenParameter);
        return lambda.Compile();
    }

    private IEnumerable<Expression> BuildBody(
        Expression virtualKeyboard,
        Expression virtualMouse,
        Expression virtualKeyboardLayout,
        Expression cancelationToken,
        CancellationToken token)
    {
        var oldDelay = TimeSpan.Zero;
        foreach (var action in _data.Actions!)
        {
            token.ThrowIfCancellationRequested();
            var timeOffset = action.TimeOffset - oldDelay;
            oldDelay = timeOffset;
            if (timeOffset > TimeSpan.Zero)
                yield return MakeDelay(
                    Expression.Constant(timeOffset),
                    cancelationToken);
            if (action is KeyboardAction keyboardAction)
            {
                yield return InvokeKeyboardAPI(
                    keyboardAction.KeySendType,
                    virtualKeyboard,
                    Expression.Constant(keyboardAction.KeyCode),
                    Expression.Constant(keyboardAction.KeyState));
            }
            else if (action is MouseAction mouseAction)
            {
                yield return InvokeMouseAPI(mouseAction, virtualMouse);
            }
            else if (action is LanguageAction langAction)
            {
                yield return InvokeLanguageAPI(langAction.Language, virtualKeyboardLayout);
            }
            else
                throw new NotImplementedException();

        }
        yield break;
    }

    private Expression InvokeKeyboardAPI(KeySendType sendType, Expression virtualKeyboard, Expression KeyCode, Expression keyState) =>
        sendType switch
        {
            KeySendType.Default => Expression.Call(
                virtualKeyboard,
                nameof(IVirtualKeyboard.SendKeyEvent),
                null,
                KeyCode,
                keyState
                ),
            KeySendType.Direct => Expression.Call(
                virtualKeyboard,
                nameof(IVirtualKeyboard.SendKeyEventDirect),
                null,
                KeyCode,
                keyState
                ),
            _ => throw new NotImplementedException(),
        };

    private Expression InvokeMouseAPI(MouseAction action, Expression virtualMouse) =>
        Expression.Call(
            virtualMouse,
            nameof(IVirtualMouse.SendMouseEvent),
            null,
            Expression.Constant(action.XOffset),
            Expression.Constant(action.YOffset),
            Expression.Constant(action.SpeedMultiplier),
            Expression.Constant(action.MouseWheel),
            Expression.Constant(action.ButtonEvent)
            );

    private Expression InvokeLanguageAPI(LanguageCode langCode, Expression virtualLanguageLayout) =>
        Expression.Call(
            virtualLanguageLayout,
            nameof(IVirtualKeyboardLayout.SetLanguage),
            null,
            Expression.Constant(langCode)
            );


    private Expression MakeDelay(Expression timeSpan, Expression cancellationToken) =>
        Expression.Call(
            Expression.Call(typeof(Task), nameof(Task.Delay), null, timeSpan, cancellationToken),
            nameof(Task.Wait),
            null,
            cancellationToken
            );
}
