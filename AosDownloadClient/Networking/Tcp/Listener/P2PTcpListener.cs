using AosP2PClient.Common;
using AosP2PClient.DBase;
using AosP2PClient.Networking.Tcp.ConnectionListener;
using AosP2PClient.Networking.Tcp.Protocol;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AosP2PClient.Networking.Tcp.Listener
{
    public class P2PTcpListener
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private TcpListener tcpListener;
        private readonly BaseManager BaseManager;
        private readonly IPEndPoint ipEndPoint;
        private readonly SettingsModel settingsModel;

        public event EventHandler<StatusChangedEventArgs> StatusChangedEvent;

        public ListenerStatus Status { get; private set; }

        public P2PTcpListener(BaseManager baseManager)
        {
            try
            {
                BaseManager = baseManager;
                Status = ListenerStatus.NotListening;
                settingsModel = SettingsManager.GetSettingsModel();
                ipEndPoint = new IPEndPoint(IPAddress.Parse(Globals.GetLocalIP()), settingsModel.TcpSettingsModel.P2PListenerPort);
                tcpListener = new TcpListener(ipEndPoint);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Can't create P2P TcpListener {Globals.GetLocalIP()}:{settingsModel.TcpSettingsModel.P2PListenerPort}");
            }
        }

        public void Start()
        {
            if (Status == ListenerStatus.Listening)
            {
                return;
            }

            try
            {
                tcpListener.Start();
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnected), null);
                RaiseStatusChanged(ListenerStatus.Listening);
            }
            catch (SocketException ex)
            {
                RaiseStatusChanged(ListenerStatus.PortNotFree);
                logger.Error(ex, $"Can't start P2P TcpListener {ipEndPoint?.Address}:{ipEndPoint?.Port}");
            }
        }

        public void Stop()
        {
            RaiseStatusChanged(ListenerStatus.NotListening);
            try
            {
                tcpListener?.Stop();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Can't stop P2P TcpListener {ipEndPoint?.Address}:{ipEndPoint?.Port}");
            }
        }

        private void OnClientConnected(IAsyncResult ar)
        {
            TcpClient tcpClient = null;
            IPEndPoint endPoint = null;
            try
            {
                tcpClient = tcpListener.EndAcceptTcpClient(ar);
                endPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;

                ThreadPool.QueueUserWorkItem(delegate
                {
                    ConnectionReceived(tcpClient);
                });
            }
            catch (SocketException ex)
            {
                tcpClient?.Close();
                logger.Error(ex, $"Can't recieve connection from {ipEndPoint?.Address}:{ipEndPoint?.Port}");
            }
            finally
            {
                try
                {
                    if (Status == ListenerStatus.Listening)
                    {
                        tcpListener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnected), null);
                    }
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        private void RaiseStatusChanged(ListenerStatus status)
        {
            if (Status != status)
            {
                Status = status;
                StatusChangedEvent?.Invoke(this, new StatusChangedEventArgs(Status));
            }
        }

        private void ConnectionReceived(TcpClient client)
        {
            IPEndPoint endPoint = null;
            try
            {
                endPoint = client.Client.RemoteEndPoint as IPEndPoint;
                NetworkStream networkStream = client.GetStream();

                int bodySize = ReadHeader(networkStream);
                FileRequest fileRequest = ReadBody(networkStream, bodySize);

                byte[] data = BaseManager.ReadFile(fileRequest.FileName);

                if (data == null)
                {
                    throw new Exception($"Cant get file: {fileRequest.FileName} as byte array");
                }
                Write(networkStream, fileRequest, data);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Tcp connection from {ipEndPoint?.Address}:{ipEndPoint?.Port}");
            }
            finally
            {
                client.GetStream()?.Close();
                client.Close();
            }
        }

        private int ReadHeader(NetworkStream stream)
        {
            byte[] header = new byte[TcpHeader.Length];
            ReadWholeArray(stream, header);

            TcpHeader tcpHeader = TcpHeader.Deserialize(header);

            if (tcpHeader.Type != MsgType.FileRequest)
            {
                throw new Exception($"Invalid request type {tcpHeader.Type}");
            }

            if (tcpHeader.DataSize <= 0 || tcpHeader.DataSize > Globals.MaxFileSize)
            {
                throw new Exception($"Invalid request data size {tcpHeader.DataSize}");
            }

            return tcpHeader.DataSize;
        }

        private FileRequest ReadBody(NetworkStream stream, int size)
        {
            byte[] body = new byte[size];
            ReadWholeArray(stream, body);
            return FileRequest.Deserialize(body);
        }

        private void Write(NetworkStream stream, FileRequest fileRequest, byte[] data)
        {
            FileResponse fileResponse = new FileResponse(fileRequest.FileName, data);
            byte[] body = fileResponse.Serialize();

            TcpHeader tcpHeader = new TcpHeader(MsgType.FileResponse, body.Length);
            byte[] header = tcpHeader.Serialize();

            stream.Write(header, 0, header.Length);
            stream.Write(body, 0, body.Length);
        }

        public static void ReadWholeArray(NetworkStream stream, byte[] data)
        {
            var offset = 0;
            var remaining = data.Length;
            while (remaining > 0)
            {
                var read = stream.Read(data, offset, remaining);
                if (read <= 0)
                {
                    throw new EndOfStreamException($"End of stream reached with {remaining} bytes left to read");
                }

                remaining -= read;
                offset += read;
            }
        }
    }
}
