using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Primitives;

namespace ASiNet.WCP.DesktopService;
public class RemoteDirectoryAccess
{
    public RemoteDirectoryAccess(ServerConfig config)
    {
        _roots = config.RemoteAccessRoots;
        _ignores = config.IgnoreFiles;
    }

    private RemoteAccessRoot[] _roots;

    private string[] _ignores;

    public GetRemoteDirectoryResponse Change(GetRemoteDirectoryRequest request)
    {
        try
        {
            if (request.GetRoots)
            {
                return new()
                {
                    Directories = _roots.Select(x => x.Path).ToArray(),
                    Files = _roots.Select(x => x.Name).ToArray(),
                    Status = GetDirectiryStatus.Success,
                };
            }
            if (request.Root is null)
                return new() { Status = GetDirectiryStatus.DirectoryNotFound };
            else if (_roots.FirstOrDefault(x => x.Name == request.Root) is RemoteAccessRoot root)
            {
                var directories = Directory.GetDirectories(root.Path);
                string[]? files = request.GetFiles ? Directory.GetFiles(root.Path).Where(x => !_ignores.Any(y => x.EndsWith(y))).ToArray() : null;
                return new()
                {
                    Directories = directories,
                    Files = files,
                    Root = root.Path,
                    Status = GetDirectiryStatus.Success
                };
            }
            else
            {
                var directories = Directory.GetDirectories(request.Root);
                string[]? files = request.GetFiles ? Directory.GetFiles(request.Root).Where(x => !_ignores.Any(y => x.EndsWith(y))).ToArray() : null;
                return new()
                {
                    Directories = directories,
                    Files = files,
                    Root = request.Root,
                    Status = GetDirectiryStatus.Success
                };
            }
        }
        catch (Exception)
        {
            return new()
            {
                Status = GetDirectiryStatus.Failed
            };
        }
    }
}
