using System.Collections.Generic;

namespace TestHttpSrv.Logger
{
    public class GeneralErrorInfo
    {
        public string Ip { get; set; }
        public int Count { get; set; }
        public List<DetailedErrorInfo> DetailedInfos;

        public GeneralErrorInfo(LogEntry logEntry)
        {
            Count = 1;
            Ip = logEntry.Ip;
            DetailedInfos = new List<DetailedErrorInfo>();
            DetailedInfos.Add(new DetailedErrorInfo(logEntry));
        }

        public void Add(LogEntry logEntry)
        {
            if (logEntry.Ip.Equals(Ip))
            {
                DetailedInfos.Add(new DetailedErrorInfo(logEntry));
                Count++;
            }
        }
    }

    public class DetailedErrorInfo
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }

        public DetailedErrorInfo(LogEntry logEntry)
        {
            Date = logEntry.Date;
            Time = logEntry.Time;
            Level = logEntry.Level;
            Message = logEntry.Message;
            Callsite = logEntry.Callsite;
            Exception = logEntry.Exception;
        }
    }

    public class LogEntryManager
    {
        public Dictionary<string, GeneralErrorInfo> Errors;

        public LogEntryManager()
        {
            Errors = new Dictionary<string, GeneralErrorInfo>();
        }

        public void Add(LogEntry logEntry)
        {
            if (Errors.ContainsKey(logEntry.Ip))
            {
                Errors[logEntry.Ip].Add(logEntry);
            }
            else
            {
                Errors.Add(logEntry.Ip, new GeneralErrorInfo(logEntry));
            }
        }
    }
}