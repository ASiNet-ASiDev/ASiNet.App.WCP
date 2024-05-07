using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASiNet.App.WCP.Models;
public interface IRdaInvoker
{

    public void RdaResult(FileSystemEntry? entry);

}
