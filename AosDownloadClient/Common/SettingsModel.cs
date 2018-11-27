using System;
using System.Xml;
using System.Xml.Serialization;

namespace AosP2PClient.Common
{
    [XmlRoot("settings")]
    public class SettingsModel : IEquatable<SettingsModel>
    {
        [XmlElement("base")]
        public BaseSettingsModel BaseSettingsModel { get; set; }
        [XmlElement("http")]
        public HttpSettingsModel HttpSettingsModel { get; set; }
        [XmlElement("tcp")]
        public TcpSettingsModel TcpSettingsModel { get; set; }

        public bool Equals(SettingsModel other)
        {
            return BaseSettingsModel.Equals(other.BaseSettingsModel) &&
                HttpSettingsModel.Equals(other.HttpSettingsModel) &&
                TcpSettingsModel.Equals(other.TcpSettingsModel);
        }
    }

    public class BaseSettingsModel : IEquatable<BaseSettingsModel>
    {
        [XmlElement("path")]
        public string Path { get; set; }
        [XmlElement("refresh-timeout")]
        public int RefreshTimeout { get; set; }
        [XmlElement("clean-timeout")]
        public int CleanTimeout { get; set; }
        [XmlElement("is-server")]
        public bool IsServer { get; set; }

        public bool Equals(BaseSettingsModel other)
        {
            return Path.Equals(other.Path) &&
                RefreshTimeout == other.RefreshTimeout &&
                CleanTimeout == other.CleanTimeout &&
                IsServer == other.IsServer;
        }
    }

    public class HttpSettingsModel : IEquatable<HttpSettingsModel>
    {
        [XmlElement("ip")]
        public string Ip { get; set; }
        [XmlElement("port")]
        public int Port { get; set; }
        [XmlElement("uri-path")]
        public string UriPath { get; set; }
        [XmlElement("files-update-timeout")]
        public int FilesUpdateTimeout { get; set; }
        [XmlElement("request-queue-size")]
        public int RequestQueueSize { get; set; }

        public bool Equals(HttpSettingsModel other)
        {
            return Ip.Equals(other.Ip) &&
                Port == other.Port &&
                UriPath.Equals(other.UriPath) &&
                FilesUpdateTimeout == other.FilesUpdateTimeout &&
                RequestQueueSize == other.RequestQueueSize;
        }
    }

    public class TcpSettingsModel : IEquatable<TcpSettingsModel>
    {
        [XmlElement("client-port")]
        public int ClientPort { get; set; }
        [XmlElement("p2p-listener-port")]
        public int P2PListenerPort { get; set; }
        [XmlElement("aos-listener-port")]
        public int AosListenerPort { get; set; }
        [XmlElement("max-parallelization-count")]
        public int MaxParallelizationCount { get; set; }

        public bool Equals(TcpSettingsModel other)
        {
            return ClientPort == other.ClientPort &&
                P2PListenerPort == other.P2PListenerPort &&
                AosListenerPort == other.AosListenerPort &&
                MaxParallelizationCount == other.MaxParallelizationCount;
        }
    }

    public static class SettingsManager
    {
        public static SettingsModel GetSettingsModel()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsModel));
            using (XmlReader reader = XmlReader.Create(currentDir + Globals.SettingsFileName))
            {
                return (SettingsModel)serializer.Deserialize(reader);
            }
        }

        public static SettingsModel GetSettingsModel(string pathToSettings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsModel));
            using (XmlReader reader = XmlReader.Create(pathToSettings + Globals.SettingsFileName))
            {
                return (SettingsModel)serializer.Deserialize(reader);
            }
        }

        public static bool SetSettingsModel(SettingsModel model)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsModel));
            using (XmlWriter writer = XmlWriter.Create(currentDir + Globals.SettingsFileName))
            {
                serializer.Serialize(writer, model);
                return true;
            }
        }

        public static bool SetSettingsModel(string pathToSettings, SettingsModel model)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsModel));
            using (XmlWriter writer = XmlWriter.Create(pathToSettings + Globals.SettingsFileName))
            {
                serializer.Serialize(writer, model);
                return true;
            }
        }
    }
}
