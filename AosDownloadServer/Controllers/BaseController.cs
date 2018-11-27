using AosP2PClient.DBase;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TestHttpSrv.Common;
using TestHttpSrv.Stats;

namespace TestHttpSrv.Controllers
{
    public class BaseController : ApiController
    {
        private readonly BaseManager baseManager = MyMvcApplication.BaseManager;
        private readonly LogEntryList logEntryList = MyMvcApplication.LogEntryList;

        [HttpGet]
        public List<BaseFile> Get()
        {
            logEntryList.Add(new LogEntry(Global.GetIP4Address(HttpContext.Current.Request.UserHostAddress), "get all"));
            var str = Global.GetIP4Address(HttpContext.Current.Request.UserHostAddress);
            var files = baseManager.GetAllFiles();
            return (files.Count > 0) ? files : null;
        }

        [HttpGet]
        public BaseFile Get(string name)
        {
            logEntryList.Add(new LogEntry(Global.GetIP4Address(HttpContext.Current.Request.UserHostAddress), $"get {name}"));
            BaseFile file = baseManager.GetFile(name);
            return file ?? null;
        }

        [HttpPut]
        public HttpResponseMessage Put(BaseFile baseFile)
        {
            logEntryList.Add(new LogEntry(Global.GetIP4Address(HttpContext.Current.Request.UserHostAddress), $"put {baseFile.Name}"));
            try
            {
                bool result = false;
                if (baseFile?.GetClients() != null || baseFile.GetClients().Count > 0)
                {
                    var clientInfo = baseFile.GetClients()[0];
                    result = baseManager.AddClient(baseFile.Name, clientInfo);
                }

                var statusCode = (result) ? HttpStatusCode.Created : HttpStatusCode.NotModified;
                return new HttpResponseMessage(statusCode);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }
    }
}