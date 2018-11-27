using System;

namespace AosP2PClient.Networking.Tcp.ConnectionListener
{
    [Flags]
    public enum ListenerStatus
    {
        Listening,
        PortNotFree,
        NotListening
    }
}
