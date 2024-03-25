using System;
using System.IO;
using System.Security.Cryptography;

namespace APV.Common
{
    public static class IOUtility
    {
        /// <summary>
        /// Checks if the file exist and can be opened with an exclusive file read lock
        /// </summary>
        public static bool IsReady(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentOutOfRangeException(nameof(filename), "Filename is empty or whitespace.");

            var file = new FileInfo(filename);
            return IsReady(file);
        }

        /// <summary>
        /// Checks if the file exist and can be opened
        /// </summary>
        public static bool IsReady(FileInfo file, FileAccess access = FileAccess.Read, FileShare fileShare = FileShare.ReadWrite)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            try
            {
                bool write = ((access == FileAccess.ReadWrite) || ((access == FileAccess.Write)));
                bool directoryExists = ((file.Directory == null) || (file.Directory.Exists));
                bool fileExists = (file.Exists);
                bool deleteDirectoryAfterCheck = false;
                bool deleteFileAfterCheck = false;
                FileMode fileMode = FileMode.Open;

                try
                {
                    if ((!write) && ((!directoryExists) || (!fileExists)))
                    {
                        return false;
                    }

                    if (write)
                    {
                        if (!directoryExists)
                        {
                            file.Directory.Create();
                            deleteDirectoryAfterCheck = true;
                        }
                        if (!fileExists)
                        {
                            fileMode = FileMode.OpenOrCreate;
                            deleteFileAfterCheck = true;
                        }
                    }

                    using (File.Open(file.FullName, fileMode, access, fileShare))
                    {
                        return true;
                    }
                }
                finally
                {
                    if (deleteFileAfterCheck)
                    {
                        file.Delete();
                    }
                    if ((deleteDirectoryAfterCheck))
                    {
                        file.Directory?.Delete();
                    }
                }
            }
            catch (IOException)
            {
            }
            return false;
        }

        public static bool TryDelete(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            try
            {
                file.Delete();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CanRead(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentOutOfRangeException(nameof(filename), "Filename is empty or whitespace.");

            var file = new FileInfo(filename);
            return CanRead(file);
        }

        public static bool CanRead(FileInfo file)
        {
            return IsReady(file);
        }

        public static bool CanWrite(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentOutOfRangeException(nameof(filename), "Filename is empty or whitespace.");

            var file = new FileInfo(filename);
            return CanWrite(file);
        }

        public static bool CanWrite(FileInfo file)
        {
            return IsReady(file, FileAccess.Write, FileShare.Read);
        }

        public static string GetChecksum(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentOutOfRangeException(nameof(filename), "Filename is empty or whitespace.");

            byte[] rawData;
            var md5 = new MD5CryptoServiceProvider();
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                rawData = md5.ComputeHash(stream);
            }
            string checksum = rawData.ToHexString();
            return checksum;
        }

        #region Path

        public static string Combine(string path1, string path2)
        {
            path1 = (path1 ?? string.Empty).Trim().TrimEnd('/', '\\');
            path2 = (path2 ?? string.Empty).Trim().TrimStart('/', '\\');
            string path = Path.Combine(path1, path2);
            return path;
        }

        public static string Combine(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            if (paths.Length == 0)
            {
                return string.Empty;
            }

            string path = paths[0];
            int length = paths.Length;
            for (int i = 1; i < length; i++)
            {
                path = Combine(path, paths[i]);
            }
            return path;
        }

        public static string GetAbsolutePath(string path, string defaultFilename = null)
        {
            string folder;
            string filename = defaultFilename;

            if (string.IsNullOrWhiteSpace(path))
            {
                folder = SystemUtility.CurrentDirectory;
            }
            else
            {
                folder = Path.GetDirectoryName(path);
                string extension = Path.GetExtension(path);
                if (!string.IsNullOrWhiteSpace(extension))
                {
                    filename = Path.GetFileName(path);
                }
            }

            if (string.IsNullOrWhiteSpace(folder))
            {
                folder = SystemUtility.CurrentDirectory;
            }
            else if (!Path.IsPathRooted(folder))
            {
                folder = Combine(SystemUtility.CurrentDirectory, folder);
            }

            path = !string.IsNullOrWhiteSpace(filename) ? Combine(folder, filename) : folder;

            return path;
        }

        #endregion
    }
}