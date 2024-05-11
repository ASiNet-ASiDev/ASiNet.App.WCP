namespace ASiNet.Yandex.Ads;
public class Banner : View
{
    public static readonly BindableProperty AdUnitIdProperty =
         BindableProperty.Create(nameof(AdUnitId), typeof(string), typeof(Banner));

    public static readonly BindableProperty AdHeightProperty =
         BindableProperty.Create(nameof(AdHeight), typeof(int), typeof(Banner), -1);

    public string AdUnitId
    {
        get { return (string)GetValue(AdUnitIdProperty); }
        set { SetValue(AdUnitIdProperty, value); }
    }

    public int AdHeight
    {
        get { return (int)GetValue(AdHeightProperty); }
        set { SetValue(AdHeightProperty, value); }
    }
}
