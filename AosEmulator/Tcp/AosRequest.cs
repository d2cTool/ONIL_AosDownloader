using System;
using System.Text;

namespace AosEmulator
{
    public class AosRequest
    {
        public int Priority { get; private set; }
        public string FileName { get; private set; }
        public AosRequest(string fileName, int priority)
        {
            FileName = fileName;
            Priority = priority;
        }
        public byte[] Serialize()
        {
            byte[] fileName = Encoding.Unicode.GetBytes(FileName);
            byte[] priority = BitConverter.GetBytes(Priority);
            byte[] msg = new byte[priority.Length + fileName.Length];

            Array.Copy(priority, 0, msg, 0, priority.Length);
            Array.Copy(fileName, 0, msg, priority.Length, fileName.Length);

            return msg;
        }
        public static AosRequest Deserialize(byte[] data)
        {
            int priority = BitConverter.ToInt32(data, 0);
            string fileName = Encoding.Unicode.GetString(data, sizeof(int), data.Length - sizeof(int));
            return new AosRequest(fileName, priority);
        }
    }
}
