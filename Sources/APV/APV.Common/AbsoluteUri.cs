using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace APV.Common
{
    [DebuggerDisplay("{Url}")]
    [Serializable]
    [DataContract(Namespace = SystemConstants.NamespaceData)]
    [XmlRoot(Namespace = SystemConstants.NamespaceData)]
    public class AbsoluteUri : IEquatable<AbsoluteUri>
    {
        #region Constants

        /// <summary>
        /// "80"
        /// </summary>
        public const int DefaultPortNumber = 80;

        /// <summary>
        /// "443"
        /// </summary>
        public const int DefaultSslPortNumber = 443;

        /// <summary>
        /// "http://"
        /// </summary>
        public const string DefaultSchema = "http://";

        /// <summary>
        /// "https://"
        /// </summary>
        public const string DefaultSslSchema = "https://";

        /// <summary>
        /// "www"
        /// </summary>
        public const string DefaultDomain = "www";

        #endregion

        private string _sourceUrl;
        private string _url;
        private string _urlWithoutQuery;
        private string _schema;
        private string _host;
        private string _port;
        private string _path;
        private string _query;
        private bool _isDefaultPort;
        private bool _isDefaultSchema;
        private bool _isDefaultDomain;
        private int _portNumber;
        private int _hashCode;

        private void Parse(string url)
        {
            _sourceUrl = url;

            //SCHEMA://HOST:PORT/PATH/?QUERY
            _portNumber = DefaultPortNumber;
            _schema = DefaultSchema;
            _host = string.Empty;
            _port = string.Empty;
            _path = string.Empty;
            _query = string.Empty;

            string pathAndQuery = string.Empty;

            url = url.Trim();
            while (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            //Schema
            int index = url.IndexOf("://", StringComparison.Ordinal);
            if (index != -1)
            {
                _schema = url.Substring(0, index + 3).ToLowerInvariant();
                if (_schema == DefaultSslSchema)
                {
                    _portNumber = DefaultSslPortNumber;
                }
                url = url.Substring(index + 3);
            }

            //pathAndQuery
            index = url.IndexOf("/", StringComparison.Ordinal);
            if (index != -1)
            {
                pathAndQuery = url.Substring(index + 1);
                url = url.Substring(0, index);
            }

            //Port
            index = url.IndexOf(":", StringComparison.Ordinal);
            if (index != -1)
            {
                string portValue = url.Substring(index + 1);
                _portNumber = int.Parse(portValue);
                if (((_schema == DefaultSchema) && (_portNumber != DefaultPortNumber)) ||
                    ((_schema == DefaultSslSchema) && (_portNumber != DefaultSslPortNumber)) ||
                    ((_schema != DefaultSchema) && (_schema != DefaultSslSchema)))
                {
                    _port = url.Substring(index);
                }
                url = url.Substring(0, index);
            }

            //Host
            _host = url.ToLowerInvariant();

            //Path
            index = pathAndQuery.IndexOf("/?", StringComparison.Ordinal);
            if (index != -1)
            {
                _query = pathAndQuery.Substring(index + 1);
                _path = pathAndQuery.Substring(0, index);//ToLowerInvariant();
            }
            else
            {
                _path = pathAndQuery;//.ToLowerInvariant();
            }

            _isDefaultPort = ((_schema == DefaultSchema) && (_portNumber == DefaultPortNumber)) ||
                             ((_schema == DefaultSslSchema) && (_portNumber == DefaultSslPortNumber));
            _isDefaultSchema = (_schema == DefaultSchema);
            _isDefaultDomain = (_host.StartsWith(DefaultDomain + "."));

            _urlWithoutQuery = $"{_schema}{_host}{_port}";
            _url = _urlWithoutQuery;
            if (!string.IsNullOrEmpty(_path))
            {
                _url = _url + "/" + _path;
            }
            if (!string.IsNullOrEmpty(_query))
            {
                _url = _url + "/" + _query;
            }
            _hashCode = $"AbsoluteUri.{_url}".GetHashCode();
        }

        static AbsoluteUri()
        {
            SetIdnIriSupport(true);
        }

        private AbsoluteUri()
        {
        }

        public AbsoluteUri(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));
            if (!IsWellFormedAbsoluteUriString(url))
                throw new ArgumentOutOfRangeException(nameof(url), $"Url \"{url}\" is not weel formed absolute url.");

            Parse(url);
        }

        public AbsoluteUri(AbsoluteUri domain, string relativeUrl)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));
            if (string.IsNullOrEmpty(relativeUrl))
                throw new ArgumentNullException(nameof(relativeUrl));

            relativeUrl = relativeUrl.Trim();
            relativeUrl = (!relativeUrl.StartsWith("/")) ? "/" + relativeUrl : relativeUrl;

            //SCHEMA://HOST:PORT/PATH/?QUERY
            string combinedUrl = $"{domain._schema}{domain._host}{domain._port}{relativeUrl}";

            if (!IsWellFormedAbsoluteUriString(combinedUrl))
                throw new ArgumentOutOfRangeException(nameof(relativeUrl), $"Relative url \"{relativeUrl}\" is not weel formed url.");

            Parse(combinedUrl);
        }

        public Uri GetUri()
        {
            return new Uri(Url);
        }

        public bool Equals(AbsoluteUri other)
        {
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (_hashCode != other._hashCode)
            {
                return false;
            }
            return (string.Compare(_url, other._url, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AbsoluteUri);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return _url;
        }

        [DataMember(Name = "Value", IsRequired = true, Order = 0)]
        [XmlElement(ElementName = "Value", IsNullable = false, Order = 0)]
        public string SourceUrl
        {
            get { return _sourceUrl; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value), "Data can not be deserialized. Url is not defined or empty.");
                if (!IsWellFormedAbsoluteUriString(value))
                    throw new ArgumentOutOfRangeException(nameof(value), $"Data can not be deserialized. Url \"{value}\" is not well formed absolute url.");

                Parse(value);
            }
        }

        public string Url
        {
            get { return _url; }
        }

        public string UrlWithoutQuery
        {
            get { return _urlWithoutQuery; }
        }

        public string Schema
        {
            get { return _schema; }
        }

        public bool IsDefaultSchema
        {
            get { return _isDefaultSchema; }
        }

        public string Host
        {
            get { return _host; }
        }

        public bool IsDefaultDomain
        {
            get { return _isDefaultDomain; }
        }

        public string Port
        {
            get { return _port; }
        }

        public int PortNumber
        {
            get { return _portNumber; }
        }

        public bool IsDefaultPort
        {
            get { return _isDefaultPort; }
        }

        public string Path
        {
            get { return _path; }
        }

        public string Query
        {
            get { return _query; }
        }

        public string Page
        {
            get
            {
                string page = _path;
                int index = page.LastIndexOf("/", StringComparison.Ordinal);
                if (index != -1)
                {
                    page = page.Substring(index + 1);
                }
                return page;
            }
        }

        public static bool IsWellFormedAbsoluteUriString(string url)
        {
            url = url ?? string.Empty;
            url = url.ToLowerInvariant();
            if (!url.Contains("://"))
            {
                url = DefaultSchema + url;
            }
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        public static void SetIdnIriSupport(bool enable)
        {
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
            //Important: Try IsWellFormedUriString() once to initialize static fields: s_IdnScope, s_IriParsing
            Uri.IsWellFormedUriString("http://example.com/query=ü", UriKind.Absolute);
// ReSharper restore ReturnValueOfPureMethodIsNotUsed

            //Get the assembly that contains the class
            Assembly assembly = Assembly.GetAssembly(typeof(Uri));
            //Use the assembly in order to get the type of the class
            Type uriType = assembly.GetType("System.Uri", true);
            const BindingFlags setFieldBindingFlags = BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic;
            UriIdnScope value = (enable) ? UriIdnScope.All : UriIdnScope.None;
            uriType.InvokeMember("s_IdnScope", setFieldBindingFlags, null, null, new object[] { value });
            uriType.InvokeMember("s_IriParsing", setFieldBindingFlags, null, null, new object[] { enable });
        }

        public static bool operator ==(AbsoluteUri x, AbsoluteUri y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(x, null))
            {
                return false;
            }
            return x.Equals(y);
        }

        public static bool operator !=(AbsoluteUri x, AbsoluteUri y)
        {
            return !(x == y);
        }
    }
}