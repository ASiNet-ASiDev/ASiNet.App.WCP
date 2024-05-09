namespace ASiNet.WCP.Common;

public class RemoteServerInfo(string? name, int port, string addres, int version)
{
    public string? Name { get; set; } = name;

    public int Port { get; set; } = port;

    public string Address { get; set; } = addres;

    public int ProtocolVersion { get; set; } = version;

    public bool IncorrectProtocolVersion { get; set; }
}
