using AosP2PClient.Common;
using AosP2PClient.DBase;
using AosP2PClient.Networking.Http;
using AosP2PClient.Networking.Tcp.Manager.DownloadManager;
using AosP2PClient.Networking.Tcp.P2PTcpClient;
using AosP2PClient.Networking.Tcp.Listener;
using System.Collections.Generic;
using System.Threading;
using System;
using AosP2PClient.Networking.Tcp.Protocol;
using System.Net;
using NLog;

namespace AosP2PClient.Networking.Tcp.Manager.DowloadManager
{
    public class DownloadManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly DownloadQueue DownloadQueue;
        private readonly BaseManager BaseManager;
        private readonly AosTcpListener AosTcpListener;
        private readonly HttpRestClientManager HttpRestClientManager;
        private readonly TaskConcurrentQueue<DownloadResult, DownloadArgs> TaskQueue;
        private readonly Dictionary<string, BaseFile> BaseFileDictionary;
        private readonly Timer timer;
        private readonly Random rnd;

        public DownloadManager(BaseManager baseManager, HttpRestClientManager httpRestClientManager)
        {
            DownloadQueue = new DownloadQueue();
            TaskQueue = new TaskConcurrentQueue<DownloadResult, DownloadArgs>();
            TaskQueue.TaskExecuted += TaskQueue_TaskExecuted;
            BaseFileDictionary = new Dictionary<string, BaseFile>();
            BaseManager = baseManager;
            AosTcpListener = new AosTcpListener();
            HttpRestClientManager = httpRestClientManager;
            AosTcpListener.AosRequestReceivedEvent += AosTcpListener_AosRequestReceivedEvent;
            httpRestClientManager.FileInfoReceived += HttpRestClientManager_FileInfoReceived;

            timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            rnd = new Random();
            AosTcpListener.Start();
        }

        private void TaskQueue_TaskExecuted(object sender, DownloadResult e)
        {
            logger.Info(""); // send downdload speed
            if (BaseFileDictionary.ContainsKey(e.Response.FileName))
            {
                var bFile = BaseFileDictionary[e.Response.FileName];
                BaseManager.WriteFile(bFile, e.Response.Data);
                BaseFileDictionary.Remove(e.Response.FileName);

                AosResponse response = new AosResponse(100, TaskQueue.GetQueueCount(), DownloadStatus.Complete, e.Response.FileName);
                AosTcpListener.Send(response);
            }
        }

        private void OnTimerElapsed(object state)
        {
            if(TaskQueue.GetQueueCount() > 0 || DownloadQueue.Count() == 0)
            {
                return;
            }

            List<QueueNode> unfinished = new List<QueueNode>();

            while (DownloadQueue.Count() > 0)
            {
                var el = DownloadQueue.Dequeue().Value;
                if (BaseFileDictionary.ContainsKey(el.FileName))
                {
                    if(BaseFileDictionary[el.FileName].ClientsCount > 0)
                    {
                        int clientId = rnd.Next(0, BaseFileDictionary[el.FileName].ClientsCount);
                        var clients = BaseFileDictionary[el.FileName].GetClients();
                        ClientInfo clientInfo = clients[clientId];

                        FileRequest fileRequest = new FileRequest(el.FileName);
                        IPAddress iPAddress = IPAddress.Parse(clientInfo.Ip);
                        IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, clientInfo.Port);
                        DownloadArgs args = new DownloadArgs(fileRequest, iPEndPoint);

                        TaskQueue.Queue(AosTcpClient.GetFile, args);
                        
                        AosResponse response = new AosResponse(0, TaskQueue.GetQueueCount(), DownloadStatus.InProgress, el.FileName);
                        AosTcpListener.Send(response);
                    }
                    else
                    {
                        AosResponse response = new AosResponse(0, TaskQueue.GetQueueCount(), DownloadStatus.NoClients, el.FileName);
                        AosTcpListener.Send(response);

                        HttpRestClientManager.GetFileInfo(el.FileName);

                        unfinished.Add(el);
                    }
                }
                else
                {
                    AosResponse response = new AosResponse(0, TaskQueue.GetQueueCount(), DownloadStatus.None, el.FileName);
                    AosTcpListener.Send(response);

                    HttpRestClientManager.GetFileInfo(el.FileName);

                    unfinished.Add(el);
                }
            }

            foreach (var item in unfinished)
            {
                DownloadQueue.Enqueue(item.Priority, item.FileName);
            }

            TaskQueue.StartTasks();
        }

        private void HttpRestClientManager_FileInfoReceived(object sender, BaseFile e)
        {
            if (BaseFileDictionary.ContainsKey(e.Name))
            {
                BaseFileDictionary[e.Name].UpdateClients(e.GetClients());
            }
            else
            {
                BaseFileDictionary.Add(e.Name, e);
            }
        }

        private void AosTcpListener_AosRequestReceivedEvent(object sender, AosRequestEventArgs e)
        {
            var req = e.Request;
            if (!BaseManager.HasFile(req.FileName))
            {
                DownloadQueue.Enqueue(req.Priority, req.FileName);
                HttpRestClientManager.GetFileInfo(req.FileName);
                //AosTcpListener.
            }
        }
    }
}
