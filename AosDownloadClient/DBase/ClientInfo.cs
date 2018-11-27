using System;
using System.Net;
using System.Runtime.Serialization;

namespace AosP2PClient.DBase
{
    [DataContract]
    public class ClientInfo : IEquatable<ClientInfo>
    {
        [DataMember]
        public int Port { get; private set; }
        [DataMember]
        public string Ip { get; private set; }
        [DataMember]
        public string RegistrationDate { get; private set; }
        [DataMember]
        public string RegistrationTime { get; private set; }
        public DateTime RegistrationDataTime { get; private set; }

        public ClientInfo(string ip, int port)
        {
            if (string.IsNullOrEmpty(ip))
            {
                throw new ArgumentException("Invalid ip address", "ip");
            }

            if (!IPAddress.TryParse(ip, out IPAddress ipAddr))
            {
                throw new ArgumentException("Invalid ip address", "ip");
            }

            if (port < 1024 || port > 49151)
            {
                throw new ArgumentOutOfRangeException("port", "Invalid port number");
            }

            Ip = ip;
            Port = port;
            DateTimeUpdate();
        }

        public void DateTimeUpdate()
        {
            RegistrationDataTime = DateTime.Now;
            RegistrationDate = RegistrationDataTime.ToShortDateString();
            RegistrationTime = RegistrationDataTime.ToShortTimeString();
        }

        public bool Equals(ClientInfo other)
        {
            return Ip.Equals(other.Ip) &&
                Port == other.Port;
        }

        public ClientInfo()
        {

        }
    }
}
