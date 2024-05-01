using ASiNet.App.WCP.Models;

namespace ASiNet.App.WCP;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
        
    }

    public static AppConfig Config => _config.Value;

    private static Lazy<AppConfig> _config = new(() => AppConfig.ReadOrEmpty());
}
