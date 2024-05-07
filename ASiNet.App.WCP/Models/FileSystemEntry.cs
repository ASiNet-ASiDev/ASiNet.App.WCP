using System.Text.RegularExpressions;

namespace ASiNet.App.WCP.Models;
public class FileSystemEntry
{
    public FileSystemEntry(string path, bool isFile, bool isDirectory)
    {
        var name = path.TrimEnd('\\', '/').Split('/', '\\').LastOrDefault();
        Name = string.IsNullOrEmpty(name) ? path : name;
        Path = path;
        IsDirectory = isDirectory;
        IsFile = isFile;
    }
    public string? Name { get; set; }

    public string? Path { get; set; }

    public string? Directory => IsFile ? GetRoot : Path;


    public bool IsFile { get; set; }

    public bool IsDirectory { get; set; }

    public string? GetRoot => Path?.Replace(Name!, string.Empty);
}
