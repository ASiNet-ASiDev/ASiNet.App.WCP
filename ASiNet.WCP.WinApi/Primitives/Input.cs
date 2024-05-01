using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.WinApi.Enums;

namespace ASiNet.WCP.WinApi.Primitives;
public struct Input
{
    public InputType type;
    public InputUnion u;
}