using AosP2PClient.Networking.Tcp.Protocol;
using System.Net;

namespace AosP2PClient.Networking.Tcp.P2PTcpClient
{
    public class DownloadArgs
    {
        public FileRequest Request { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public DownloadArgs(FileRequest request, IPEndPoint endPoint)
        {
            Request = request;
            EndPoint = endPoint;
        }
    }
}
