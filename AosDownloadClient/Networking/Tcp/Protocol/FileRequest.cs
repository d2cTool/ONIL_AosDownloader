using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AosP2PClient.Networking.Tcp.Protocol
{
    [Serializable]
    public class FileRequest
    {
        public string FileName { get; private set; }

        public FileRequest(string fileName)
        {
            FileName = fileName;
        }

        public byte[] GetHeader()
        {
            TcpHeader tcpHeader = new TcpHeader(MsgType.FileRequest, Serialize().Length);
            return tcpHeader.Serialize();
        }

        public byte[] Serialize()
        {
            using (var memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, this);
                return memoryStream.ToArray();
            }
        }

        public static FileRequest Deserialize(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                return new BinaryFormatter().Deserialize(memoryStream) as FileRequest;
            }
        }
    }
}
