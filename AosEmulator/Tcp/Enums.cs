using System;

namespace AosEmulator
{
    [Flags]
    public enum MsgType : byte
    {
        None,
        FileRequest,
        FileResponse,
        AosRequest,
        AosResponse
    }

    [Flags]
    public enum DownloadStatus : byte
    {
        None,
        Complete,
        InProgress,
        NoClients
    }
}
