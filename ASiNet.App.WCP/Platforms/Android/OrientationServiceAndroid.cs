using Android.Content.PM;

namespace ASiNet.App.WCP;
public partial class OrientationService
{
    private static readonly IReadOnlyDictionary<DisplayOrientation, ScreenOrientation> _androidDisplayOrientationMap =
            new Dictionary<DisplayOrientation, ScreenOrientation>
            {
                [DisplayOrientation.Landscape] = ScreenOrientation.Landscape,
                [DisplayOrientation.Portrait] = ScreenOrientation.Portrait,
            };

    private static readonly IReadOnlyDictionary<DisplayOrientation, ScreenOrientation> _androidDisplayOrientationMapReverse =
            new Dictionary<DisplayOrientation, ScreenOrientation>
            {
                [DisplayOrientation.Landscape] = ScreenOrientation.ReverseLandscape,
                [DisplayOrientation.Portrait] = ScreenOrientation.Portrait,
            };

    public partial void SetOrientation(DisplayOrientation orientation, bool reverse)
    {
        var currentActivity = ActivityStateManager.Default.GetCurrentActivity();
        if (currentActivity is not null)
        {
            if (!reverse)
            {
                if (_androidDisplayOrientationMapReverse.TryGetValue(orientation, out ScreenOrientation screenOrientation))
                    currentActivity.RequestedOrientation = screenOrientation;
            }
            else
            {
                if (_androidDisplayOrientationMap.TryGetValue(orientation, out ScreenOrientation screenOrientation))
                    currentActivity.RequestedOrientation = screenOrientation;
            }
        }
    }
}
