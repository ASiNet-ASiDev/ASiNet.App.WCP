using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASiNet.App.WCP;
public partial class OrientationService
{

    public static OrientationService Current => _instance.Value;

    private static Lazy<OrientationService> _instance = new(() => new());

    public partial void SetOrientation(DisplayOrientation orientation, bool reverse);

}
