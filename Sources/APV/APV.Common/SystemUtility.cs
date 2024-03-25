using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using System.ServiceModel;
using System.Web;

namespace APV.Common
{
    public static class SystemUtility
    {
        #region Private

        private static readonly List<KeyValuePair<Assembly, Guid>> AssembliesIdentifiers = new List<KeyValuePair<Assembly, Guid>>();
        private static bool _isWebApplication;
        private static string _windowsName;

        private static int GetArchitecture()
        {
            string pa = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            return (((string.IsNullOrEmpty(pa)) || ((string.Compare(pa, 0, "x86", 0, 3, true) == 0)) ? 32 : 64));
        }

        #endregion

        #region Environment

        public static bool IsWebApplication
        {
            get
            {
                if (_isWebApplication)
                {
                    return true;
                }
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                bool isWebApplication = (entryAssembly == null) && ((HttpContext.Current != null) || (OperationContext.Current != null));
                if (isWebApplication)
                {
                    _isWebApplication = true;
                }
                return isWebApplication;
            }
        }

        public static bool IsWindowsService
        {
            get
            {
                return
                    (!IsWebApplication) &&
                    (!string.IsNullOrWhiteSpace(AppDomain.CurrentDomain.BaseDirectory)) &&
                    (!string.IsNullOrWhiteSpace(AppDomain.CurrentDomain.BaseDirectory)) &&
                    (Environment.CurrentDirectory.Trim('\\') != AppDomain.CurrentDomain.BaseDirectory.Trim('\\'));
            }
        }

        public static string CurrentDirectory
        {
            get
            {
                if (IsWebApplication)
                {
                    return HttpRuntime.AppDomainAppPath;
                }
                if (IsWindowsService)
                {
                    return AppDomain.CurrentDomain.BaseDirectory;
                }
                return Environment.CurrentDirectory;
            }
        }

        public static string BinDirectory
        {
            get
            {
                return (!IsWebApplication)
                    ? CurrentDirectory
                    : IOUtility.Combine(CurrentDirectory, "bin");
            }
        }

        /// <summary>
        /// Call it in global.asax on application init for web application
        /// </summary>
        public static void SetWebApplication()
        {
            _isWebApplication = true;
        }

        public static string GetWindowsName()
        {
            if (_windowsName != null)
            {
                return _windowsName;
            }

            // Get OperatingSystem information from the system namespace.
            OperatingSystem info = Environment.OSVersion;

            // Determine the platform.
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            object platform = searcher
                .Get()
                .OfType<ManagementObject>()
                .Select(x => x.GetPropertyValue("Caption"))
                .FirstOrDefault();

            string name = platform?.ToString();

            if (string.IsNullOrWhiteSpace(name))
            {
                switch (info.Platform)
                {
                    case PlatformID.Win32S:
                        name = "Windows 3.1";
                        break;

                    case PlatformID.WinCE:
                        name = "Windows CE";
                        break;

                    // Platform is Windows 95, Windows 98, 
                    // Windows 98 Second Edition, or Windows Me.
                    case PlatformID.Win32Windows:
                        switch (info.Version.Minor)
                        {
                            case 0:
                                name = "Windows 95";
                                break;

                            case 10:
                                name = (info.Version.Revision.ToString(CultureInfo.InvariantCulture) == "2222A")
                                           ? "Windows 98 Second Edition"
                                           : "Windows 98";
                                break;

                            case 90:
                                name = "Windows Me";
                                break;
                        }
                        break;

                    // Platform is Windows NT 3.51, Windows NT 4.0, Windows 2000,
                    // or Windows XP.
                    case PlatformID.Win32NT:

                        switch (info.Version.Major)
                        {
                            case 3:
                                name = "Windows NT 3.51";
                                break;

                            case 4:
                                name = "Windows NT 4.0";
                                break;

                            case 5:
                                switch (info.Version.Minor)
                                {
                                    case 0:
                                        name = "Windows 2000";
                                        break;

                                    case 1:
                                        name = "Windows WinXP";
                                        break;

                                    case 2:
                                        name = "Windows 2003";
                                        break;
                                }
                                break;

                            case 6:
                                switch (info.Version.Minor)
                                {
                                    case 0:
                                        name = "Windows Vista (2008 Server)";
                                        break;

                                    case 1:
                                        name = "Windows 7 (2008 Server R2)";
                                        break;

                                    case 2:
                                        name = "Windows 8 (2012 Server)";
                                        break;

                                    case 3:
                                        return "Windows 8.1 (2012 Server R2)";
                                }
                                break;

                            case 10:
                                name = "Windows 10 (Server 2016 Technical Preview)";
                                break;
                        }
                        break;
                }
            }

            if (name != null)
            {
                name = name.Trim();
                if (info.ServicePack != "")
                {
                    name += " " + info.ServicePack;
                }
                name += " x" + GetArchitecture().ToString(CultureInfo.InvariantCulture);
                name = name.Trim();
            }

            _windowsName = name;
            return _windowsName;
        }

        public static string GetApplicationVersion()
        {
            AssemblyName assemblyName = typeof(SystemUtility).Assembly.GetName();
            string version = assemblyName.Version.ToString();
            return version;
        }

        #endregion

        public static Guid GetRuntimeIdentifier(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            lock (AssembliesIdentifiers)
            {
                int count = AssembliesIdentifiers.Count;
                for (int i = 0; i < count; i++)
                {
                    KeyValuePair<Assembly, Guid> item = AssembliesIdentifiers[i];
                    if (ReferenceEquals(item.Key, assembly))
                    {
                        return item.Value;
                    }
                }
                Guid id = Guid.NewGuid();
                AssembliesIdentifiers.Add(new KeyValuePair<Assembly, Guid>(assembly, id));
                return id;
            }
        }
    }
}