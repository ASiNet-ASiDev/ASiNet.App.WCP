using System.Text.Json;
using System.Text.Json.Serialization;

namespace ASiNet.App.WCP.Models;
public class AppConfig
{
    static AppConfig()
    {
        if (!Directory.Exists(CnfDir))
            Directory.CreateDirectory(CnfDir);
    }
    [JsonPropertyName("interface_reverse_horizontal_orientation")]
    public bool ReverseOrientation { get; set; }
    [JsonPropertyName("connection_auto_connect")]
    public bool AutoConnect { get; set; }
    [JsonPropertyName("connection_address")]
    public string? Address { get; set; } = null!;
    [JsonPropertyName("connection_port")]
    public int Port { get; set; }


    private static string CnfDir = Path.Join(FileSystem.AppDataDirectory, "configs");
    private static string CnfPath = Path.Join(CnfDir, "app_config.cnf");

    public static AppConfig ReadOrEmpty()
    {
        if(!File.Exists(CnfPath))
        {
            var cnf = new AppConfig() { Port = 44544 };
            cnf.SaveOrUpdate();
            return cnf;
        }

        using var file = File.OpenRead(CnfPath);
        var data = JsonSerializer.Deserialize<AppConfig>(file);
        return data ?? new() { Port = 44544 };
    }

    public void SaveOrUpdate()
    {
        using FileStream file = File.Create(CnfPath);
        JsonSerializer.Serialize(file, this);
    }

}
