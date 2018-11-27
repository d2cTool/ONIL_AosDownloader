using AosP2PClient.Networking.Tcp.Protocol;
using System;
using System.Net;

namespace AosP2PClient.Networking.Tcp.P2PTcpClient
{
    public class DownloadResult : EventArgs
    {
        public FileRequest Request { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public int DownloadSpeed { get; private set; }
        public FileResponse Response { get; private set; }
        public DownloadResult(FileRequest request, IPEndPoint endPoint, int downloadSpeed, FileResponse response)
        {
            Request = request;
            EndPoint = endPoint;
            DownloadSpeed = downloadSpeed;
            Response = response;
        }
    }
}
