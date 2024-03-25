using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using APV.Common.Extensions;
using APV.GraphicsLibrary;

namespace APV.Avtoliga.UnitTest.Resources
{
    public static class ResourceManager
    {
        private static byte[] LoadDataFromResource(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Type type = typeof(ResourceManager);
            Assembly assembly = type.Assembly;
            string resourceNamespace = type.Namespace;
            string resourceName = string.Format("{0}.{1}", resourceNamespace, name);
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new ArgumentOutOfRangeException("name", string.Format("Unknown resource name \"{0}\".", name));

                return stream.ToByteArray();
            }
        }

        private static Image LoadImageFromResource(string name)
        {
            byte[] data = LoadDataFromResource(name);
            return data.ToImage();
        }

        public static readonly Image NoPhoto = LoadImageFromResource("NoPhoto.png");

        public static readonly Image DepoLogo = LoadImageFromResource("Depo.png");

        public static readonly Image EagleEyesLogo = LoadImageFromResource("EagleEyesLogo.jpg");

        public static readonly Image SonarLogo = LoadImageFromResource("Sonar.png");

        public static readonly Image TycLogo = LoadImageFromResource("Tyc.png");

        public static readonly Image TygLogo = LoadImageFromResource("Tyg.png");

        public static readonly Image NewsLogo = LoadImageFromResource("News.jpg");

        public static readonly Image NewsHolidayLogo = LoadImageFromResource("NewsHoliday.jpg");

        public static readonly Image NewsSchedulerLogo = LoadImageFromResource("NewsScheduler.png");

        public static readonly Image NewsWorkHoursLogo = LoadImageFromResource("NewsWorkHours.png");
    }
}