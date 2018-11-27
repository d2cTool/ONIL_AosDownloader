using AosP2PClient.Common;
using AosP2PClient.Networking.Tcp.ConnectionListener;
using AosP2PClient.Networking.Tcp.Protocol;
using NLog;
using System;
using System.Net;
using System.Net.Sockets;

namespace AosP2PClient.Networking.Tcp.Listener
{
    public class AosRequestEventArgs : EventArgs
    {
        public AosRequest Request { get; private set; }

        public AosRequestEventArgs(AosRequest request)
        {
            Request = request;
        }
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public ListenerStatus TcpListenerStatus { get; private set; }
        public StatusChangedEventArgs(ListenerStatus tcpListenerStatus)
        {
            TcpListenerStatus = tcpListenerStatus;
        }
    }

    public class AosTcpListener
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private TcpListener tcpListener;
        private readonly SettingsModel settingsModel;
        private readonly IPEndPoint ipEndPoint;

        private TcpClient tcpClient = null;
        
        public ListenerStatus Status { get; private set; }
        public event EventHandler<StatusChangedEventArgs> StatusChangedEvent;
        public event EventHandler<AosRequestEventArgs> AosRequestReceivedEvent;

        public AosTcpListener()
        {
            try
            {
                settingsModel = SettingsManager.GetSettingsModel();
                ipEndPoint = new IPEndPoint(IPAddress.Parse(Globals.GetLocalIP()), settingsModel.TcpSettingsModel.AosListenerPort);
                tcpListener = new TcpListener(ipEndPoint);
                Status = ListenerStatus.NotListening;
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Can't create Aos TcpListener {Globals.GetLocalIP()}:{settingsModel.TcpSettingsModel.AosListenerPort}");
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
                logger.Error(ex, $"Can't start AosListener");
            }
        }

        private void OnClientConnected(IAsyncResult ar)
        {
            IPEndPoint endPoint = null;

            try
            {
                tcpClient = tcpListener.EndAcceptTcpClient(ar);
                endPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                try
                {
                    ConnectionReceived(tcpClient);
                }
                catch(Exception ex)
                {
                    tcpClient?.Close();
                    logger.Error(ex, $"Can't get data from {endPoint?.Address}:{endPoint?.Port}");
                }
            }
            catch (SocketException ex)
            {
                tcpClient?.Close();
                logger.Error(ex, $"Can't recieve connection from {endPoint?.Address}:{endPoint?.Port}");
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

        private void ConnectionReceived(TcpClient tcpClient)
        {
            try
            {
                SocketStateObject headerState = new SocketStateObject(TcpHeader.Length);
                Socket client = tcpClient.Client;
                headerState.socket = client;

                var sync = client.BeginReceive(headerState.buffer, 0, headerState.bufferSize, 0, new AsyncCallback(ReceiveHeaderCallback), headerState);
                sync.AsyncWaitHandle.WaitOne();
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Can't recieve data");
            }
        }

        private void ReceiveHeaderCallback(IAsyncResult ar)
        {
            try
            {
                SocketStateObject headerState = (SocketStateObject)ar.AsyncState;
                Socket client = headerState.socket;

                int bytesRead = client.EndReceive(ar);
                TcpHeader tcpHeader = ParseHeader(headerState.buffer);

                SocketStateObject bodyState = new SocketStateObject(tcpHeader.DataSize);
                bodyState.socket = client;

                var sync = client.BeginReceive(bodyState.buffer, 0, bodyState.bufferSize, 0, new AsyncCallback(ReceiveBodyCallback), bodyState);
                sync.AsyncWaitHandle.WaitOne();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Can't recieve data");
            }
        }

        private void ReceiveBodyCallback(IAsyncResult ar)
        {
            try
            {
                SocketStateObject bodyState = (SocketStateObject)ar.AsyncState;
                Socket client = bodyState.socket;

                int bytesRead = client.EndReceive(ar);
                AosRequest tcpRequest = AosRequest.Deserialize(bodyState.buffer);

                AosRequestReceivedEvent?.Invoke(this, new AosRequestEventArgs(tcpRequest));

                SocketStateObject headerState = new SocketStateObject(TcpHeader.Length);
                headerState.socket = client;

                var sync = client.BeginReceive(headerState.buffer, 0, headerState.bufferSize, 0, new AsyncCallback(ReceiveHeaderCallback), headerState);
                sync.AsyncWaitHandle.WaitOne();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Can't recieve data");
            }
        }

        private TcpHeader ParseHeader(byte[] buffer)
        {
            TcpHeader tcpHeader = TcpHeader.Deserialize(buffer);
            if (tcpHeader.Type != MsgType.AosRequest)
            {
                throw new Exception($"Invalid request type {tcpHeader.Type}");
            }
            if (tcpHeader.DataSize <= 0 || tcpHeader.DataSize > Globals.MaxFileSize)
            {
                throw new Exception($"Invalid request data size {tcpHeader.DataSize}");
            }
            return tcpHeader;
        }

        public bool Send(AosResponse response)
        {
            IPEndPoint endPoint = null;
            try
            {
                endPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint; 

                byte[] data = response.Serialize();
                TcpHeader tcpHeader = new TcpHeader(MsgType.AosResponse, data.Length);
                byte[] header = tcpHeader.Serialize();

                tcpClient.Client.Send(header);
                tcpClient.Client.Send(data);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Can't send AosResponse: {response.FileName} to {endPoint?.Address}:{endPoint?.Port}");
                return false;
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

        public void Stop()
        {
            //AosTcpClient = null;
            Status = ListenerStatus.NotListening;
            tcpListener?.Stop();
        }
    }
}
