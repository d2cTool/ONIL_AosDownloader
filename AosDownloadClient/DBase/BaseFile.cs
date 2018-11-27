using AosP2PClient.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace AosP2PClient.DBase
{
    [DataContract]
    public class BaseFile : EventArgs, IEquatable<BaseFile>
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Md5 { get; private set; }
        [DataMember]
        public long Size { get; private set; }
        public int ClientsCount => clients.Count;

        private readonly FileInfo file;
        [DataMember]
        private List<ClientInfo> clients = new List<ClientInfo>();

        public BaseFile(FileInfo file, DirectoryInfo pathToBase)
        {
            MD5 md5Hasher = MD5.Create();
            Size = file.Length;
            Name = FileAndPathManipulation.MakeRelativePath(pathToBase.FullName, file.FullName);
            byte[] Md5Hash = FileAndPathManipulation.GetMD5(file.FullName);
            Md5 = Globals.ToHex(Md5Hash, false);
            this.file = file;
        }

        public BaseFile() { }

        public void AddClient(ClientInfo clientInfo)
        {
            if (clients.Contains(clientInfo))
            {
                var c = clients.Find(cl => cl.Equals(clientInfo));
                c.DateTimeUpdate();
            }
            else
            {
                clients.Add(clientInfo);
            }
        }

        public void UpdateClients(List<ClientInfo> clientInfoList)
        {
            clients = clientInfoList;
        }

        public List<ClientInfo> GetClients()
        {
            return clients;
        }

        public bool RemoveClient(ClientInfo clientInfo)
        {
            if (clients.Contains(clientInfo))
            {
                clients.Remove(clientInfo);
                return true;
            }
            return false;
        }

        public bool Equals(BaseFile other)
        {
            return Name.Equals(other.Name) &&
                Md5 == other.Md5 &&
                Size == other.Size;
        }

        public byte[] GetFile()
        {
            byte[] data = new byte[Size];
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                fs.Read(data, 0, data.Length);
            }
            return data;
        }
    }
}
