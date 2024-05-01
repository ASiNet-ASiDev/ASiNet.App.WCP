namespace ASiNet.WCP.Core.Macroses;
public class MacrosData
{
    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? Description { get; set; }

    public string? Author { get; set; }

    public IUserAction[]? Actions { get; set; } 
}
