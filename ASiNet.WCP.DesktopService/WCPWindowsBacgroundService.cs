using ASiNet.WCP.History;
using ASiNet.WCP.Server;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ASiNet.WCP.DesktopService;

public class WCPWindowsBacgroundService : BackgroundService
{
    private readonly ILogger<WCPWindowsBacgroundService> _logger;

    private WcpNetworkServer _wcpServer = null!;

    public WCPWindowsBacgroundService(ILogger<WCPWindowsBacgroundService> logger)
    {
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _wcpServer = new();
        _wcpServer.Start();
        using (var context = new HistoryContext())
            context.Database.EnsureCreated();

        new ToastContentBuilder().AddText("WCP Service started.")
            .Show();
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _wcpServer.Stop();
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _wcpServer.Updater(stoppingToken);
    }
}
