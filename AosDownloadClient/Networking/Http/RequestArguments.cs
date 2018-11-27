using System.Net;

namespace AosP2PClient.Networking.Http
{
    public class RequestArguments
    {
        public string Content { get; private set; }
        public HttpWebRequest Request { get; private set; }
        public RequestArguments(HttpWebRequest request, string content)
        {
            Request = request;
            Content = content;
        }
    }
}
