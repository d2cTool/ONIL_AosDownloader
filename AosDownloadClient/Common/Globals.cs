using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AosP2PClient.Common
{
    public static class Globals
    {
        public const int MaxFileSize = 20 * 1024 * 1024;
        public const int MaxTrunkSize = 1024 * 1024;
        public const int BufferSize = 8 * 1024 * 1024;
        public const int TcpReceiveTimeout = 10 * 1000; // 10 sec
        public const int TcpSendTimeout = 10 * 1000; // 10 sec
        public const string SettingsFileName = @"/AosP2PFileSharingSettings.xml";

        public static string GetLocalIP()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                return (socket.LocalEndPoint as IPEndPoint).Address.ToString();
            }
        }

        public static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            }

            return result.ToString();
        }
    }
}
