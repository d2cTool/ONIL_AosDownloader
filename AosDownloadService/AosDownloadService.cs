using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace AosDownloader
{
    public partial class AosDownloadService : ServiceBase
    {
        private EventLog eventLog;
        private Bootstrap bootstrap;

        public AosDownloadService()
        {
            InitializeComponent();
            eventLog = new EventLog();
            if (!EventLog.SourceExists("AosDownloader"))
            {
                EventLog.CreateEventSource("AosDownloader", "AosLog");
            }
            eventLog.Source = "AosDownloader";
            eventLog.Log = "AosLog";

            try
            {
                bootstrap = new Bootstrap();
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                if (ex.Data.Contains("LogMsg"))
                {
                    eventLog.WriteEntry(ex.Data["LogMsg"].ToString(), EventLogEntryType.Error);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("Start");
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("Stop");
        }
    }
}
