using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class GetRemoteDirectoryRequest : Package
{
    public string? RootPath { get; set; }
}
