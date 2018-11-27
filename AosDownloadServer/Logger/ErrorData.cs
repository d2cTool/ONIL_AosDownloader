using System.Runtime.Serialization;

namespace TestHttpSrv.Logger
{
    [DataContract]
    public class ErrorData
    {
        [DataMember]
        public string logger;
        [DataMember]
        public string level;
        [DataMember]
        public string message;
        [DataMember]
        public string callsite;
        [DataMember]
        public string exception;
    }
}