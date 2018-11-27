using AosP2PClient.Common;
using AosP2PClient.DBase;
using AosP2PClient.Networking.Http;
using AosP2PClient.Networking.Tcp.Listener;
using AosP2PClient.Networking.Tcp.Manager.DowloadManager;
using NLog;
using System;

namespace AosDownloader
{
    public class Bootstrap : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly SettingsModel SettingsModel;
        private static BaseManager baseManager;
        private static HttpRestClientManager httpRestClientManager;
        private static P2PTcpListener p2pTcpListener;
        private static DownloadManager downloadManager;
        public Bootstrap()
        {
            SettingsModel = SettingsManager.GetSettingsModel();
            baseManager = BaseManager.Instance;
            httpRestClientManager = new HttpRestClientManager(baseManager);
            p2pTcpListener = new P2PTcpListener(baseManager);
            downloadManager = new DownloadManager(baseManager, httpRestClientManager);
            p2pTcpListener.Start();
        }

        public void Dispose()
        {
            p2pTcpListener?.Stop();
            //downloadManager?.
        }
    }
}
