using Com.Yandex.Mobile.Ads.Banner;
using Com.Yandex.Mobile.Ads.Common;
using Microsoft.Maui.Handlers;

namespace ASiNet.Yandex.Ads;
public class BannerHandler : ViewHandler<Banner, BannerAdView>
{
    public BannerHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    public static IPropertyMapper<Banner, BannerHandler> PropertyMapper = new PropertyMapper<Banner, BannerHandler>(ViewHandler.ViewMapper)
    {
        [nameof(Banner.AdUnitId)] = MapAdUnitId,
    };

    public static CommandMapper<Banner, BannerHandler> CommandMapper = new(ViewCommandMapper) { };

    public static void MapAdUnitId(BannerHandler handler, Banner banner) { }

    protected override BannerAdView CreatePlatformView()
    {
        var bannerAd = new BannerAdView(Context);
        
        bannerAd.SetAdSize(AdSize.FlexibleSize((int)VirtualView.Width, VirtualView.AdHeight == -1 ? (int)VirtualView.Height : VirtualView.AdHeight));
        bannerAd.SetMinimumHeight(50);
        bannerAd.SetMinimumWidth(320);

        bannerAd.SetAdUnitId(this.VirtualView.AdUnitId);
        var adRequest = new AdRequest.Builder()
         .Build();

        bannerAd?.LoadAd(adRequest);
        return bannerAd!;
    }
}


