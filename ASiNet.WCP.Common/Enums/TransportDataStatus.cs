namespace ASiNet.WCP.Common.Enums;
public enum TransportDataStatus : byte
{
    Ok,
    Failed,
    HashSumMismath,
    FileNotFound,
    OperationClosed,
    OperationNotFound,
    OperationNotSupported,
}
