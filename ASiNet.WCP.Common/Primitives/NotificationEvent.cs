using ASiNet.WCP.Common.Enums;
using ASiNet.WCP.Common.Network;

namespace ASiNet.WCP.Common.Primitives;
public class NotificationEvent : Package
{
    public NotificationType Type { get; set; }

    public long? Id { get; set; }
    public byte[]? Data { get; set; }
}
