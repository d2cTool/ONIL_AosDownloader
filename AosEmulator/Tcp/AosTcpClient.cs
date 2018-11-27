using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AosEmulator
{
    public class AosTcpClient : IDisposable
    {
        private TcpClient tcpClient;
        private IPEndPoint endPoint;
        private NetworkStream networkStream;
        //StateObject 

        public event EventHandler<AosResponse> GetDataEvent;

        public AosTcpClient()
        {
            tcpClient = new TcpClient();
        } 

        public void ChangeEndPoint(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
        }

        public void Connect()
        {
            tcpClient.Connect(endPoint);
            networkStream = tcpClient.GetStream();
            //tcpClient.Client.BeginReceive();


            while (true)
            {
                if (networkStream.DataAvailable)
                {
                    var response = GetResponse();
                    GetDataEvent?.Invoke(this, response);
                }
                Thread.Sleep(0);
            }
        }

        public void Send(string fileName, int priority)
        {
            SendRequest(MakeRequest(fileName, priority));
        }

        private AosRequest MakeRequest(string fileName, int priority)
        {
            return new AosRequest(fileName, priority);
        }

        private void SendRequest(AosRequest request)
        {
            byte[] data = request.Serialize();
            TcpHeader tcpHeader = new TcpHeader(MsgType.AosRequest, data.Length);
            byte[] header = tcpHeader.Serialize();

            networkStream.Write(header, 0, header.Length);
            networkStream.Write(data, 0, data.Length);
        }

        private AosResponse GetResponse()
        {
            byte[] header = new byte[TcpHeader.Length];
            ReadWholeArray(networkStream, header);
            TcpHeader tcpHeader = TcpHeader.Deserialize(header);

            byte[] body = new byte[tcpHeader.DataSize];
            ReadWholeArray(networkStream, body);

            return AosResponse.Deserialize(body);
        }

        private static void ReadWholeArray(NetworkStream stream, byte[] data)
        {
            var offset = 0;
            var remaining = data.Length;
            while (remaining > 0)
            {
                var read = stream.Read(data, offset, remaining);
                if (read <= 0)
                {
                    throw new EndOfStreamException($"Can't read all data: {remaining} bytes left to read");
                }

                remaining -= read;
                offset += read;
            }
        }

        public void Dispose()
        {
            networkStream?.Dispose();
            tcpClient?.Close();
        }
    }
}
