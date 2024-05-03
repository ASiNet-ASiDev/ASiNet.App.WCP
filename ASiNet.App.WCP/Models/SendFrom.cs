using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.App.WCP.Models.Enums;

namespace ASiNet.App.WCP.Models;
public class SendFrom
{
    public string? Name { get; set; }

    public SendFromFlag Flag { get; set; }
}
