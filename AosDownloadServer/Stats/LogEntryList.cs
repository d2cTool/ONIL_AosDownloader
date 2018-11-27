using System.Collections.Generic;
using System.Linq;

namespace TestHttpSrv.Stats
{
    public class LogEntryList
    {
        private readonly List<LogEntry> logEntries;
        public List<LogEntry> sortedLogEntries;
        public LogEntryList()
        {
            logEntries = new List<LogEntry>();
            sortedLogEntries = new List<LogEntry>();
        }

        public void Add(LogEntry logEntry)
        {
            if(logEntries.Count > 20)
            {
                logEntries.RemoveAt(19);
            }

            logEntries.Insert(0,logEntry);
            sortedLogEntries = logEntries.OrderBy(o => o.Date).ToList();
        }
    }
}