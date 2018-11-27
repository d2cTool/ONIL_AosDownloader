using System;

namespace AosP2PClient.Networking.Tcp.Protocol
{
    public struct TcpHeader
    {
        public MsgType Type { get; private set; }
        public int DataSize { get; private set; }
        public const int Length = sizeof(int) + sizeof(int);

        public TcpHeader(MsgType type, int length)
        {
            DataSize = length;
            Type = type;
        }

        public byte[] Serialize()
        {
            byte[] type = BitConverter.GetBytes((int)Type);
            byte[] length = BitConverter.GetBytes(DataSize);

            byte[] msg = new byte[Length];
            Array.Copy(type, 0, msg, 0, type.Length);
            Array.Copy(length, 0, msg, sizeof(int), length.Length);
            return msg;
        }

        public static TcpHeader Deserialize(byte[] data)
        {
            int type = BitConverter.ToInt32(data, 0);
            int length = BitConverter.ToInt32(data, sizeof(int));
            return new TcpHeader((MsgType)type, length);
        }
    }
}
