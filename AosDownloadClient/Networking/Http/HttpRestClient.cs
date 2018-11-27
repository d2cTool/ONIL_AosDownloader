using NLog;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace AosP2PClient.Networking.Http
{
    public static class HttpRestClient
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static ResponseResult SendRequest(RequestArguments requestArguments)
        {
            try
            {
                if (string.IsNullOrEmpty(requestArguments.Content))
                {
                    using (HttpWebResponse response = requestArguments.Request.GetResponse() as HttpWebResponse)
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                            return new ResponseResult(result);
                        }
                    }
                }
                else
                {
                    requestArguments.Request.ContentType = "application/json";
                    byte[] sData = Encoding.UTF8.GetBytes(requestArguments.Content);
                    requestArguments.Request.ContentLength = sData.Length;

                    using (var stream = requestArguments.Request.GetRequestStream())
                    {
                        stream.Write(sData, 0, sData.Length);
                    }

                    using (var response = requestArguments.Request.GetResponse() as HttpWebResponse)
                    {
                        string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        return new ResponseResult(result);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"request: {requestArguments.Request.RequestUri}, content: {requestArguments.Content}");
                return null;
            }
        }
    }
}
