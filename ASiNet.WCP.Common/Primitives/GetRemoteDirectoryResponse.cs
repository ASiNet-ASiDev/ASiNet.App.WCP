using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class GetRemoteDirectoryResponse : Package
{

    public GetDirectiryStatus Status { get; set; }

    public string[]? Directories { get; set; }

}
