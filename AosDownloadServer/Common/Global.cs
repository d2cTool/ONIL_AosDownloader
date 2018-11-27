using System.Net;

namespace TestHttpSrv.Common
{
    public static class Global
    {
        public static string GetIP4Address(string userHostAddr)
        {
            string IP4Address = string.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(userHostAddr))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != string.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }
    }
}