using System;
using System.Runtime.Serialization;
using APV.Common;
using APV.Common.Extensions;
using APV.Pottle.Common;
using APV.Pottle.WebParsers.InfoEntities;

namespace APV.Pottle.WebParsers.ResultEntities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class ParseResult<TParserInfo> where TParserInfo : BaseParserInfo
    {
        private string _html;
        private byte[] _htmlHashCode;

        public ParseResult(AbsoluteUri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            Url = url;
            Timestamp = DateTime.UtcNow;
        }

        [DataMember(IsRequired = true, Order = 0)]
        public AbsoluteUri Url { get; private set; }

        [DataMember(Order = 1)]
        public DateTime Timestamp { get; private set; }

        [DataMember(Order = 2)]
        public bool Success { get; set; }

        [DataMember(Order = 3)]
        public bool NotFound { get; set; }

        [DataMember(Order = 4)]
        public string Error { get; set; }

        [DataMember(IsRequired = true, Order = 5)]
        public byte[] HtmlHashCode
        {
            get { return (_htmlHashCode ?? (_htmlHashCode = ((_html ?? string.Empty).Hash256()))); }
            private set { _htmlHashCode = value; }
        }

        [IgnoreDataMember]
        public Exception Exception { get; set; }

        [DataMember(Order = 6)]
        public TParserInfo[] Data { get; set; }

        [DataMember(IsRequired = true, Order = 7)]
        public string Html
        {
            get { return _html; }
            set
            {
                if (_html != value)
                {
                    _html = value;
                    _htmlHashCode = null;
                }
            }
        }
    }
}