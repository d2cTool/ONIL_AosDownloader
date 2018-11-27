using System;
using System.Text;

namespace AosP2PClient.Networking.Tcp.Protocol
{
    public class AosResponse
    {
        public int Percentage { get; private set; }
        public int QueueSize { get; private set; }
        public DownloadStatus Status { get; private set; }
        public string FileName { get; private set; }
        public AosResponse(int percentage, int queueSize, DownloadStatus status, string fileName)
        {
            FileName = fileName;
            Percentage = percentage;
            Status = status;
            QueueSize = queueSize;
        }
        public byte[] Serialize()
        {
            byte[] percentage = BitConverter.GetBytes(Percentage);
            byte[] queueSize = BitConverter.GetBytes(QueueSize);
            byte[] status = BitConverter.GetBytes((int)Status);
            byte[] file = Encoding.Unicode.GetBytes(FileName);

            byte[] msg = new byte[file.Length + percentage.Length + status.Length + queueSize.Length];

            Array.Copy(percentage, 0, msg, 0, percentage.Length);
            Array.Copy(queueSize, 0, msg, percentage.Length, queueSize.Length);
            Array.Copy(status, 0, msg, percentage.Length + queueSize.Length, status.Length);
            Array.Copy(file, 0, msg, status.Length + queueSize.Length + percentage.Length, file.Length);

            return msg;
        }
        public static AosResponse Deserialize(byte[] data)
        {
            int percentage = BitConverter.ToInt32(data, 0);
            int queueSize = BitConverter.ToInt32(data, sizeof(int));
            DownloadStatus status = (DownloadStatus)data[sizeof(int) + sizeof(int)];
            string file = Encoding.Unicode.GetString(data, sizeof(int) + sizeof(int) + sizeof(int), data.Length - sizeof(int) - sizeof(int) - sizeof(int));
            return new AosResponse(percentage, queueSize, status, file);
        }
    }
}
