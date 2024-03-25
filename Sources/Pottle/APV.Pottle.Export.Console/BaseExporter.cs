using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Export.ConsoleApplication
{
    public abstract class BaseExporter
    {
        private int _successCount = 0;
        private int _failedCount = 0;

        public static readonly Encoding RussianEncoding = Encoding.GetEncoding("windows-1251");

        public const string IconImage = "Icon.ico";

        public const string LogoImage = "Logo.png";

        public abstract string[] GetTags();

        public abstract string GetTitle();

        public abstract bool ExportEntity(string fodler);

        public SortedList<string, string> ParseTags(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (!File.Exists(path))
                throw new InvalidOperationException(string.Format("File \"{0}\" does not exist.", path));

            string[] tags = GetTags();

            var result = new SortedList<string, string>();
            string[] data = File.ReadLines(path, RussianEncoding).ToArray();
            foreach (string line in data)
            {
                string[] items = (line ?? string.Empty).Trim().Split(new[] {": ", ":\t"}, StringSplitOptions.RemoveEmptyEntries);
                if ((items.Length == 2) && (!string.IsNullOrWhiteSpace(items[0])))
                {
                    string key = items[0].Trim();
                    string tag = tags.SingleOrDefault(tagItem => string.Equals(key.Replace(" ", string.Empty), tagItem.Replace(" ", string.Empty), StringComparison.InvariantCultureIgnoreCase));
                    if (tag != null)
                    {
                        string value = items[1].Trim();
                        if (value == string.Empty)
                        {
                            value = null;
                        }
                        result.Add(tag, value);
                    }
                }
            }

            foreach (string tag in tags)
            {
                if (!result.ContainsKey(tag))
                {
                    result.Add(tag, null);
                }
            }

            return result;
        }

        public DataImageEntity GetImage(string filename, int size)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (File.Exists(filename))
            {
                byte[] data = File.ReadAllBytes(filename);
                return ImageManagement.Instance.Create(data, size);
            }
            return null;
        }

        public DataImageEntity GetLogo(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentNullException("folder");

            string path = Path.Combine(folder, LogoImage);
            return GetImage(path, Constants.LogoSize);
        }

        public DataImageEntity GetIcon(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentNullException("folder");

            string path = Path.Combine(folder, IconImage);
            return GetImage(path, Constants.IconSize);
        }

        public void Export(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentNullException("folder");
            if (!Directory.Exists(folder))
                throw new ArgumentOutOfRangeException("folder", string.Format("Folder \"{0}\" does not exist.", folder));

            string title = GetTitle();
            Console.WriteLine();
            Console.WriteLine("{0}. Экспорт из папки \"{1}\".", title, folder);
            string[] companies = Directory.GetDirectories(folder);

            try
            {
                if (companies.Length == 0)
                {
                    ExportEntity(folder);
                }
                else
                {
                    foreach (string company in companies)
                    {
                        bool success = ExportEntity(company);
                        if (success)
                        {
                            _successCount++;
                        }
                        else
                        {
                            _failedCount++;
                        }
                    }
                }
                Console.WriteLine("{0}. Экспорт успешно завершён (success:{1}, failed:{2}).", title, _successCount, _failedCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR]{0}. Ошибка при экспорте из папки \"{1}\", ошибка \"{2}\".", title, folder, ex.Message);
            }

            Console.WriteLine();
        }
    }
}