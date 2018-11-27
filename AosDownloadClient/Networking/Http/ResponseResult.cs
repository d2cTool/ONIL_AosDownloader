using System;

namespace AosP2PClient.Networking.Http
{
    public class ResponseResult : EventArgs
    {
        public string Result { get; private set; }
        public ResponseResult(string result)
        {
            Result = result;
        }
    }
}
