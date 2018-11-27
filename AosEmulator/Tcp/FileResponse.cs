using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AosEmulator
{
    [Serializable]
    public class FileResponse : EventArgs
    {
        public string FileName { get; private set; }
        public byte[] Data { get; private set; }
        public FileResponse(string name, byte[] data)
        {
            FileName = name;
            Data = data;
        }
        public byte[] Serialize()
        {
            using (var memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, this);
                return memoryStream.ToArray();
            }
        }
        public static FileResponse Deserialize(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                return new BinaryFormatter().Deserialize(memoryStream) as FileResponse;
            }
        }
    }
}

