using AosP2PClient.Common;
using AosP2PClient.DBase;
using System;
using System.Net;
using System.Threading;

namespace AosP2PClient.Networking.Http
{
    public class HttpRestClientManager
    {
        private readonly BaseManager BaseManager;
        private readonly SettingsModel SettingsModel;
        private readonly TaskConcurrentQueue<ResponseResult, RequestArguments> taskConcurrentQueue;
        private readonly Timer timer;

        public event EventHandler<BaseFile> FileInfoReceived;

        public HttpRestClientManager(BaseManager baseManager)
        {
            BaseManager = baseManager;
            SettingsModel = SettingsManager.GetSettingsModel();
            taskConcurrentQueue = new TaskConcurrentQueue<ResponseResult, RequestArguments>(SettingsModel.HttpSettingsModel.RequestQueueSize);
            taskConcurrentQueue.TaskExecuted += TaskConcurrentQueue_TaskExecuted;

            timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero, TimeSpan.FromMinutes(SettingsModel.HttpSettingsModel.FilesUpdateTimeout));
        }

        private void TaskConcurrentQueue_TaskExecuted(object sender, ResponseResult e)
        {
            //logger.Debug("http: get response: {0}", e?.Result);
            string response = e?.Result;
            if (!string.IsNullOrEmpty(response) && response != "null")
            {
                var baseFile = JSON.Deserialize<BaseFile>(response);
                FileInfoReceived?.Invoke(this, baseFile);
            }
        }

        private void OnTimerElapsed(object state)
        {
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = SettingsModel.HttpSettingsModel.Ip,
                Port = SettingsModel.HttpSettingsModel.Port,
                Path = SettingsModel.HttpSettingsModel.UriPath + "put"
            };

            foreach (var item in BaseManager.GetAllFiles())
            {
                //item.AddClient(Globals.GetLocalIP(), SettingsModel.TcpSettingsModel.ListenerPort);
                ClientInfo clientInfo = new ClientInfo(Globals.GetLocalIP(), SettingsModel.TcpSettingsModel.P2PListenerPort);
                item.AddClient(clientInfo);
                HttpWebRequest request = WebRequest.Create(uriBuilder.Uri) as HttpWebRequest;
                request.Method = "PUT";

                string content = JSON.Serialize(item);
                Func<RequestArguments, ResponseResult> sendRequestDelegate = HttpRestClient.SendRequest;
                RequestArguments requestArgument = new RequestArguments(request, content);
                taskConcurrentQueue.Queue(sendRequestDelegate, requestArgument);
                //logger.Debug("http: update: create request: {0} {1}:{2}", item.Name, SettingsModel.HttpSettingsModel.Ip, 
                //    SettingsModel.HttpSettingsModel.Port);
            }
            taskConcurrentQueue.StartTasks();
        }

        public void GetFileInfo(string fileName)
        {
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = SettingsModel.HttpSettingsModel.Ip,
                Port = SettingsModel.HttpSettingsModel.Port,
                Path = SettingsModel.HttpSettingsModel.UriPath + "get",
                Query = $"name={fileName}"
            };

            HttpWebRequest request = WebRequest.Create(uriBuilder.Uri) as HttpWebRequest;
            request.Method = "GET";

            Func<RequestArguments, ResponseResult> sendRequestDelegate = HttpRestClient.SendRequest;
            RequestArguments requestArgument = new RequestArguments(request, null);
            taskConcurrentQueue.Queue(sendRequestDelegate, requestArgument);
            taskConcurrentQueue.StartTasks();
            //logger.Debug("http: get: create request: {0} {1}:{2}", fileName, SettingsModel.HttpSettingsModel.Ip, 
            //    SettingsModel.HttpSettingsModel.Port);
        }
    }
}
