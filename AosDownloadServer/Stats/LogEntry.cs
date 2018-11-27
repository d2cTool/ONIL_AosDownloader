using System;
using System.Net;

namespace TestHttpSrv.Stats
{
    public class LogEntry
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Ip { get; set; }
        public string Request { get; set; }
        public LogEntry(string ip, string request)
        {
            Date = DateTime.Now.ToShortDateString();
            Time = DateTime.Now.ToShortTimeString();
            Ip = ip;
            Request = request;
        }
    }
}