using AosP2PClient.Common;
using System;
using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace AosP2PClient.DBase
{
    public sealed class BaseWatcher
    {
        private static readonly object padlock = new object();
        private static volatile BaseWatcher instance;

        private readonly SettingsModel settingsModel;
        private FileSystemWatcher fileSystemWatcher;

        public event EventHandler<EventArgs> FolderChanged;

        public static BaseWatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new BaseWatcher();
                        }
                    }
                }
                return instance;
            }
        }

        [PermissionSet(SecurityAction.Demand)]
        private BaseWatcher()
        {
            settingsModel = SettingsManager.GetSettingsModel();
            fileSystemWatcher = new FileSystemWatcher(settingsModel.BaseSettingsModel.Path)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName,
                EnableRaisingEvents = true
            };
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Created += FileSystemWatcher_Changed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            while (IsFileLocked(e.FullPath))
            {
                Thread.Sleep(1);
            }
            FolderChanged?.Invoke(this, new EventArgs());
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            while (IsFileLocked(e.FullPath))
            {
                Thread.Sleep(1);
            }
            FolderChanged?.Invoke(this, new EventArgs());
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            throw new Exception($"Base modification. Deleted file: {e.FullPath}");
        }

        [PermissionSet(SecurityAction.Demand)]
        private bool IsFileLocked(string file)
        {
            FileStream stream = null;
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
            return false;
        }
    }
}
