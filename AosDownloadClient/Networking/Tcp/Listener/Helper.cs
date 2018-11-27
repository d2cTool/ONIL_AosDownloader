using System.Net.Sockets;

namespace AosP2PClient.Networking.Tcp.Listener
{
    public class SocketStateObject
    {
        public Socket socket = null;
        public readonly int bufferSize;
        public readonly byte[] buffer;
        public SocketStateObject(int bufSize)
        {
            bufferSize = bufSize;
            buffer = new byte[bufferSize];
        }
    }
}
