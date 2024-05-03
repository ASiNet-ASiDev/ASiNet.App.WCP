using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.DesktopService;
public class RemoteDirectoryAccess
{
    public RemoteDirectoryAccess()
    {
        try
        {
            _roots = DriveInfo.GetDrives().Select(x => x.Name).ToArray();
        }
        catch { }
    }

    private string[]? _roots;

    public GetRemoteDirectoryResponse GetDirectory(GetRemoteDirectoryRequest request)
    {
        try
        {
            if (request.RootPath is null)
            {
                return new()
                {
                    Status = _roots is null ? GetDirectiryStatus.AccessDenied : GetDirectiryStatus.Success, 
                    Directories = _roots,
                };
            }
            else
            {
                if(!Directory.Exists(request.RootPath))
                    return new() { Status = GetDirectiryStatus.DirectoryNotFound };
                var dirs = Directory.GetDirectories(request.RootPath);
                return new()
                { 
                    Status = GetDirectiryStatus.Success,
                    Directories =  dirs
                };
            }
        }
        catch 
        {
            return new()
            {
                Status = GetDirectiryStatus.Failed,
                Directories = null,
            };
        }
    }

}
