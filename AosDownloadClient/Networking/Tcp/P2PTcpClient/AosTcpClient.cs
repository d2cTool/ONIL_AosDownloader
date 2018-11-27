using AosP2PClient.Common;
using AosP2PClient.Networking.Tcp.Protocol;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace AosP2PClient.Networking.Tcp.P2PTcpClient
{
    public static class AosTcpClient
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Stopwatch stopWatch = new Stopwatch();

        public static DownloadResult GetFile(DownloadArgs args)
        {
            stopWatch.Reset();
            stopWatch.Start();
            FileResponse response = GetResponse(args.EndPoint, args.Request);
            stopWatch.Stop();

            int speed = (int)(response.Data.Length * 8 / (stopWatch.ElapsedMilliseconds + 1)); // kbps
            DownloadResult result = new DownloadResult(args.Request, args.EndPoint, speed, response);

            return result;
        }

        private static FileResponse GetResponse(IPEndPoint endPoint, FileRequest request)
        {
            TcpClient tcpClient = new TcpClient()
            {
                ReceiveTimeout = Globals.TcpReceiveTimeout,
                SendTimeout = Globals.TcpSendTimeout
            };

            NetworkStream networkStream = null;
            try
            {
                tcpClient.Connect(endPoint);
                networkStream = tcpClient.GetStream();
                SendRequest(networkStream, request);
                return ReadResponse(networkStream);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Can't get response {endPoint.Address}:{endPoint.Port}");
                return null;
            }
            finally
            {
                networkStream?.Close();
                tcpClient.Close();
            }
        }

        private static void SendRequest(NetworkStream networkStream, FileRequest request)
        {
            byte[] header = request.GetHeader();
            byte[] data = request.Serialize();

            networkStream.Write(header, 0, header.Length);
            networkStream.Write(data, 0, data.Length);
        }

        private static FileResponse ReadResponse(NetworkStream networkStream)
        {
            byte[] header = new byte[TcpHeader.Length];
            ReadWholeArray(networkStream, header);
            TcpHeader tcpHeader = ParseHeader(header);

            byte[] body = new byte[tcpHeader.DataSize];
            ReadWholeArray(networkStream, body);

            return FileResponse.Deserialize(body);
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

        private static TcpHeader ParseHeader(byte[] buffer)
        {
            TcpHeader tcpHeader = TcpHeader.Deserialize(buffer);

            if (tcpHeader.Type != MsgType.AosResponse)
            {
                //throw new Exception($"Invalid request type {tcpHeader.Type}");
            }

            if (tcpHeader.DataSize <= 0 || tcpHeader.DataSize > Globals.MaxFileSize)
            {
                throw new Exception($"Invalid body size {tcpHeader.DataSize}");
            }
            return tcpHeader;
        }
    }
}
