using ASiNet.App.WCP.Models;

namespace ASiNet.App.WCP;

public partial class App : Application
{
    public App(IServiceProvider provider)
    {
        InitializeComponent();

        Services = provider;
        AlertSvc = Services.GetService<IAlertService>()!;

        MainPage = new AppShell();

    }

    public static IServiceProvider Services { get; private set; } = null!;
    public static IAlertService AlertSvc { get; private set; } = null!;

    public static AppConfig Config => _config.Value;

    private static Lazy<AppConfig> _config = new(() => AppConfig.ReadOrEmpty());
}
