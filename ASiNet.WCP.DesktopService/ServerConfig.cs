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
            return new();
        }
    }

    private static ServerConfig NewDefault()
    {
        var cnf = new ServerConfig() { Port = 44544 };
        cnf.SaveOrUpdate();
        return cnf;
    }

    public void SaveOrUpdate()
    {
        using var file = File.Create(ConfigPath);
        JsonSerializer.Serialize(file, this);
    }

}
