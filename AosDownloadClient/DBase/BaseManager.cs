using AosP2PClient.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AosP2PClient.DBase
{
    public sealed class BaseManager : IDisposable
    {
        private static readonly object padlock = new object();
        private static volatile BaseManager instance;

        private static SettingsModel SettingsModel;
        private DirectoryInfo basePath;
        private Dictionary<string, BaseFile> baseDictionary = new Dictionary<string, BaseFile>();
        private readonly Timer timer;
        private BaseWatcher BaseWatcher;

        private BaseManager()
        {
            SettingsModel = SettingsManager.GetSettingsModel();
            basePath = new DirectoryInfo(SettingsModel.BaseSettingsModel.Path);

            if (!basePath.Exists)
            {
                throw new DirectoryNotFoundException("Base directory does not exist or could not be found: " + SettingsModel.BaseSettingsModel.Path);
            }

            Refresh();

            if (SettingsModel.BaseSettingsModel.IsServer)
            {
                BaseWatcher = BaseWatcher.Instance;
                BaseWatcher.FolderChanged += BaseWatcher_FolderChanged;
            }

            timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero, TimeSpan.FromMinutes(SettingsModel.BaseSettingsModel.RefreshTimeout));
        }

        public static BaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new BaseManager();
                        }
                    }
                }
                return instance;
            }
        }

        private void BaseWatcher_FolderChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void OnTimerElapsed(object state)
        {
            lock (baseDictionary)
            {
                foreach (var item in baseDictionary.Values)
                {
                    var clients = item.GetClients();
                    if (clients.Count > 0)
                    {
                        var now = DateTime.Now;
                        int removed = clients.RemoveAll(cl =>
                        (now - cl.RegistrationDataTime).Minutes > SettingsModel.BaseSettingsModel.CleanTimeout);
                        //logger.Debug("base: clean: name: {0} removed: {1}", item.Name, removed);
                    }
                }
            }
        }

        public void Refresh()
        {
            lock (baseDictionary)
            {
                FileInfo[] files = basePath.GetFiles("*.zip*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    var RelativePath = FileAndPathManipulation.MakeRelativePath(basePath.FullName, file.FullName);
                    if (!baseDictionary.ContainsKey(RelativePath))
                    {
                        var baseFile = new BaseFile(file, basePath);
                        baseDictionary.Add(RelativePath, baseFile);
                    }
                }
            }
            //logger.Debug("base: refresh: count: {0}", baseDictionary.Count);
        }

        public bool HasFile(string relPath)
        {
            return baseDictionary.ContainsKey(relPath);
        }

        public BaseFile GetFile(string relPath)
        {
            return baseDictionary.ContainsKey(relPath) ? baseDictionary[relPath] : null;
        }

        public List<BaseFile> GetAllFiles()
        {
            List<BaseFile> files = new List<BaseFile>(baseDictionary.Count);
            foreach (var item in baseDictionary)
            {
                files.Add(item.Value);
            }
            return files;
        }

        public byte[] ReadFile(string name)
        {
            try
            {
                return baseDictionary[name].GetFile();
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "@GetFilePart");
                return null;
            }
        }

        public bool WriteFile(BaseFile file, byte[] data)
        {
            try
            {
                string path = basePath.FullName + file.Name;
                File.WriteAllBytes(path, data);
                Refresh();
                return true;
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "@GetFilePart");
                return false;
            }
        }

        public bool AddClient(string name, ClientInfo clientInfo)
        {
            if (baseDictionary.ContainsKey(name))
            {
                baseDictionary[name].AddClient(clientInfo);
                return true;
            }
            return false;
        }

        public string GetPathToBase()
        {
            return basePath.FullName;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
