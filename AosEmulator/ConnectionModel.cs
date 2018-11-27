using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Windows.Input;

namespace AosEmulator
{
    public class ConnectionModel : INotifyPropertyChanged, IDisposable
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string FileName { get; set; }
        public string Protocol { get; set; }
        public int Priority { get; set; }

        private AosTcpClient AosTcpClient;
        private Thread thrd; 
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ConnectBtnClicked { get; set; }
        public ICommand SendBtnClicked { get; set; }

        public ConnectionModel()
        {
            AosTcpClient = new AosTcpClient();
            AosTcpClient.GetDataEvent += AosTcpClient_GetDataEvent;
            ConnectBtnClicked = new SimpleCommand(o => Connect(o));
            SendBtnClicked = new SimpleCommand(o => Send(o));
            Ip = "192.168.1.78";
            Port = "6767";
            FileName = "tmp.zip";
            Protocol = "";
            Priority = 1;
        }

        private void AosTcpClient_GetDataEvent(object sender, AosResponse e)
        {
            AddToProtocol($"name: {e.FileName}, %: {e.Percentage}, qSize: {e.QueueSize}, status: {e.Status}");
        }

        private void AddToProtocol(string msg)
        {
            DateTime dateTime = DateTime.Now;
            string time = dateTime.ToLongTimeString();

            string result = $"{time}: {msg}";
            Protocol += result + "\r\n";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Protocol"));
        }

        private IPEndPoint GetIPAddress()
        {
            IPAddress iPAddress = IPAddress.Parse(Ip);
            int port = int.Parse(Port);
            return new IPEndPoint(iPAddress, port);
        }

        public void Connect(object o)
        {
            AddToProtocol($"Try connect to {Ip}:{Port}");
            try
            {
                if (thrd != null)
                {
                    thrd.Abort();
                    thrd = null;
                }

                AosTcpClient.ChangeEndPoint(GetIPAddress());
                thrd = new Thread(AosTcpClient.Connect);
                thrd.Start();
            }
            catch (Exception ex)
            {
                AddToProtocol($" -- {ex.Message}");
            }
        }

        public void Send(object o)
        {
            AddToProtocol($"Try send request: file: {FileName}, priority: {Priority}");
            try
            {
                AosTcpClient.Send(FileName, Priority);
            }
            catch(Exception ex)
            {
                AddToProtocol($" -- {ex.Message}");
            }
        }

        public void Dispose()
        {
            AosTcpClient?.Dispose();
            thrd?.Abort();
        }
    }
}
