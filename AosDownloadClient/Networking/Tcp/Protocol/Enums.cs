using System;

namespace AosP2PClient.Networking.Tcp.Protocol
{
    [Flags]
    public enum MsgType
    {
        None,
        FileRequest,
        FileResponse,
        AosRequest,
        AosResponse
    }

    [Flags]
    public enum DownloadStatus
    {
        None,
        Complete,
        InProgress,
        NoClients
    }
}
