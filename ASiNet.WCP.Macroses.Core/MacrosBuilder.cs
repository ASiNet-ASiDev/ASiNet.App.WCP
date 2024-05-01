using ASiNet.WCP.Common.Interfaces;
using ASiNet.WCP.Core.Macroses;
using ASiNet.WCP.Macroses.Core.Actions;

namespace ASiNet.WCP.Macroses.Core;
public class MacrosBuilder
{

    private readonly List<IUserAction> _actions = [];

    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? Description { get; set; }

    public string? Author { get; set; }

    public MacrosBuilder SetName(string name)
    {
        Name = name;
        return this;
    }

    public MacrosBuilder SetShortName(string? shortName)
    {
        ShortName = shortName;
        return this;
    }

    public MacrosBuilder SetDescription(string? description)
    {
        Description = description;
        return this;
    }

    public MacrosBuilder SetAuthor(string? author)
    {
        Author = author;
        return this;
    }

    public MacrosBuilder AddAction(IUserAction action)
    {
        var lastTimeOffset = _actions.LastOrDefault()?.TimeOffset ?? TimeSpan.Zero;

        if (action.TimeOffset - lastTimeOffset < TimeSpan.Zero)
            throw new IndexOutOfRangeException($"Negative time offset: [{action.TimeOffset - lastTimeOffset}], new time offset [{action.TimeOffset.Milliseconds}] less than expected [{lastTimeOffset.Milliseconds}].");

        _actions.Add(action);
        return this;
    }


    public Macros Build(IVirtualMouse? virtualMouse, IVirtualKeyboard? virtualKeyboard, IVirtualKeyboardLayout? virtualKeyboardLayout)
    {
        if (Name is null)
            throw new NullReferenceException(nameof(Name));
        var data = new MacrosData()
        {
            Name = Name,
            ShortName = ShortName,
            Description = Description,
            Author = Author,
            Actions = [.. _actions]
        };
        return new(data, virtualMouse, virtualKeyboard, virtualKeyboardLayout);
    }

    public MacrosData BuildAsMacrosData()
    {
        if (Name is null)
            throw new NullReferenceException(nameof(Name));
        var data = new MacrosData()
        {
            Name = Name,
            ShortName = ShortName,
            Description = Description,
            Author = Author,
            Actions = [.. _actions]
        };
        return data;
    }

}
