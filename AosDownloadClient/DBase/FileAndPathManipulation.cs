using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace AosP2PClient.DBase
{
    public static class FileAndPathManipulation
    {
        static ReaderWriterLock locker = new ReaderWriterLock();

        public static void WriteFile(string fullName, byte[] data)
        {
            try
            {
                locker.AcquireWriterLock(int.MaxValue);
                File.WriteAllBytes(fullName, data);
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }

        public static byte[] ReadFile(string fullName)
        {
            try
            {
                locker.AcquireReaderLock(int.MaxValue);
                return File.ReadAllBytes(fullName);
            }
            finally
            {
                locker.ReleaseReaderLock();
            }
        }

        public static void DirectoryCopy(string oldPath, string newPath, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(oldPath);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + oldPath);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(newPath, file.Name);
                file.CopyTo(tempPath, true);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string tempPath = Path.Combine(newPath, subDir.Name);
                    DirectoryCopy(subDir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static byte[] GetMD5(string fullFileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fullFileName))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentNullException("fromPath");
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentNullException("toPath");
            }

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
    }

    class FileCompare : IEqualityComparer<FileInfo>
    {
        public FileCompare() { }

        public bool Equals(FileInfo f1, FileInfo f2)
        {
            return (f1.Name == f2.Name &&
                    f1.Length == f2.Length);
        }

        public int GetHashCode(FileInfo fi)
        {
            string s = String.Format("{0}{1}", fi.Name, fi.Length);
            return s.GetHashCode();
        }
    }
}
