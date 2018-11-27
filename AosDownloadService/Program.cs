using System.ServiceProcess;

namespace AosDownloader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AosDownloadService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
