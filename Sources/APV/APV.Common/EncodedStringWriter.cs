using System;
using System.IO;
using System.Text;

namespace APV.Common
{
    public class EncodedStringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        public EncodedStringWriter(StringBuilder sb, Encoding encoding)
            : base(sb)
        {
            _encoding = encoding;
        }

        public EncodedStringWriter(StringBuilder sb, IFormatProvider provider)
            : base(sb, provider)
        {
            _encoding = Encoding.UTF8;
        }

        public override Encoding Encoding
        {
            get { return _encoding; }
        }
    }
}