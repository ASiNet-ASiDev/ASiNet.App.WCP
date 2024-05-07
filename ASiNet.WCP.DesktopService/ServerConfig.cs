using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ASiNet.WCP.DesktopService;
public class ServerConfig
{
    static ServerConfig()
    {
        if (!Directory.Exists(ConfigsPath))
            Directory.CreateDirectory(ConfigsPath);
    }

    public static string AppDirectory = Environment.CurrentDirectory;

    [JsonPropertyName("server_port")]
    public int Port { get; set; }

    [JsonPropertyName("files_directory")]
    public string FilesDirectory { get; set; } = null!;

    [JsonPropertyName("media_streams_count")]
    public int MediaClientCount { get; set; }

    [JsonPropertyName("media_port")]
    public int MediaPort { get; set; }

    [JsonPropertyName("media_connection_timeout")]
    public int MediaConnectionTimeout { get; set; }

    [JsonPropertyName("remote_access_roots")]
    public RemoteAccessRoot[] RemoteAccessRoots { get; set; } = null!;

    [JsonPropertyName("remote_access_ignore")]
    public string[] IgnoreFiles { get; set; } = null!;

    private static string ConfigsPath = Path.Join(AppDirectory, "configs");

    private static string ConfigPath = Path.Join(ConfigsPath, "config.cnf");

    public static ServerConfig LoadOrEmpty()
    {
        try
        {
            using var stream = File.Exists(ConfigPath) ? File.OpenRead(ConfigPath) : null;
            return (stream is not null ? JsonSerializer.Deserialize<ServerConfig>(stream) : NewDefault()) ?? NewDefault();
        }
        catch (Exception)
        {
            File.Delete(ConfigPath);
            var cnf = new ServerConfig();
            cnf.SaveOrUpdate();
            return cnf;
        }
    }

    public void SaveOrUpdate()
    {
        using var file = File.Create(ConfigPath);
        JsonSerializer.Serialize(file, this);
    }

    private static ServerConfig NewDefault()
    {
        var cnf = new ServerConfig()
        { 
            Port = 44544,
            FilesDirectory = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "WCP"),
            MediaPort = 44600,
            MediaConnectionTimeout = 10_000,
            MediaClientCount = 4,
            RemoteAccessRoots = DefaultRoots(), 
            IgnoreFiles = [".ini", ".ink"]
        };
        cnf.SaveOrUpdate();
        return cnf;
    }


    private static RemoteAccessRoot[] DefaultRoots()
    {
        return [
            new RemoteAccessRoot() { Name = "Downloads", Path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads") },
            new RemoteAccessRoot() { Name = "Desktop", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) },
            new RemoteAccessRoot() { Name = "Videos", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) },
            new RemoteAccessRoot() { Name = "Pictures", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) },
            new RemoteAccessRoot() { Name = "Documents", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
            new RemoteAccessRoot() { Name = "Music", Path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) },
            ];
    }

}
