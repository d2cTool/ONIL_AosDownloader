using System;

namespace TestHttpSrv.Logger
{
    public class LogEntry
    {
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Ip { get; set; }
        public LogEntry(string ip, ErrorData errorData)
        {
            Date = DateTime.Now.ToShortDateString();
            Time = DateTime.Now.ToShortTimeString();
            Logger = errorData.logger;
            Message = errorData.message;
            Level = errorData.level;
            Callsite = errorData.callsite;
            Exception = errorData.exception;
            Ip = ip;
        }
    }
}