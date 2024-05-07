namespace ASiNet.Yandex.Ads;
public class Banner : View
{
    public static readonly BindableProperty AdUnitIdProperty =
         BindableProperty.Create(nameof(AdUnitId), typeof(string), typeof(Banner));

    public string AdUnitId
    {
        get { return (string)GetValue(AdUnitIdProperty); }
        set { SetValue(AdUnitIdProperty, value); }
    }
}
