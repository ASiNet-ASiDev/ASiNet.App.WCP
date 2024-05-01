using Microsoft.Extensions.Hosting;

namespace ASiNet.WCP.DesktopService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<WCPWindowsBacgroundService>();
        var host = builder.Build();
        host.Run();
    }
}