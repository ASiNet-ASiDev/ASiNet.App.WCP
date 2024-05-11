using ASiNet.App.WCP.Resources.Localization;
using CommunityToolkit.Maui;
using LocalizationResourceManager.Maui;
using ASiNet.Yandex.Ads;

namespace ASiNet.App.WCP;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.RegisterYandexAds();
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Awesome6-Solid.otf", "A6_S");
            })
            .UseLocalizationResourceManager(settings =>
            {
                settings.AddResource(AppResources.ResourceManager);
                settings.RestoreLatestCulture(true);
            });
        builder.Services.AddSingleton<IAlertService, AlertService>();
        //#if DEBUG
        //		builder.Logging.AddDebug();
        //#endif

        return builder.Build();
    }
}
