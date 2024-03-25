using System;
using System.Collections.Generic;
using System.Globalization;
using APV.Common;
using APV.Common.Html;
using APV.Pottle.WebParsers.InfoEntities;
using APV.Pottle.WebParsers.ResultEntities;

namespace APV.Pottle.WebParsers
{
    public abstract class BaseParser<TParserInfo>
        where TParserInfo : BaseParserInfo
    {
        protected abstract TParserInfo[] Parse(AbsoluteUri url, HtmlDocument doc);

        protected string GetString(HtmlDocument doc, string pattern, bool mandatory = true)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException("pattern");

            List<HtmlTag> tags = doc.Find(pattern);
            string value = (tags.Count == 1) ? tags[0].Text : null;

            if (mandatory)
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException(string.Format("String value can not be found in html for pattern \"{0}\".", pattern));
            }

            return value;
        }

        protected double GetDouble(HtmlDocument doc, string pattern)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException("pattern");

            List<HtmlTag> tags = doc.Find(pattern);
            string stringValue = (tags.Count == 1) ? tags[0].Text : null;

            double? doubleValue = ParseDouble(stringValue);

            if (doubleValue == null)
                throw new InvalidOperationException(string.Format("String value can not be found in html for pattern \"{0}\" (\"{1}\").", pattern, stringValue));

            return doubleValue.Value;
        }

        protected double? ParseDouble(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Trim().Replace(",", ".");
                double doubleValue;
                if (double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out doubleValue))
                {
                    return doubleValue;
                }
            }
            return null;
        }

        protected int GetInteger(HtmlDocument doc, string pattern, bool greaterThanZero = true)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException("pattern");

            int? intValue = null;
            List<HtmlTag> tags = doc.Find(pattern);
            string stringValue = (tags.Count == 1) ? tags[0].Text : null;

            if (!string.IsNullOrWhiteSpace(stringValue))
            {
                int value;
                if ((int.TryParse(stringValue, out value)))
                {
                    if ((greaterThanZero) && (value <= 0))
                        throw new InvalidOperationException(string.Format("Imvalid integer value \"{0}\" (less or zero) for pattern \"{1}\".", value, pattern));

                    intValue = value;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("String value \"{0}\" can not be converted to integer for pattern \"{1}\".", stringValue, pattern));
                }
            }

            if (intValue == null)
                throw new InvalidOperationException(string.Format("String value can not be found in html for pattern \"{0}\".", pattern));

            return intValue.Value;
        }

        protected bool Exists(HtmlDocument doc, string pattern, string value = null)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException("pattern");

            List<HtmlTag> tags = doc.Find(pattern);
            if (tags.Count != 0)
            {
                if (value != null)
                {
                    value = value.Trim();
                    string stringValue = tags[0].Text;
                    return (value == stringValue);
                }
                return true;
            }
            return false;
        }

        public ParseResult<TParserInfo> Parse(AbsoluteUri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            var result = (ParseResult<TParserInfo>)Activator.CreateInstance(typeof(ParseResult<TParserInfo>), new object[] { url });
            try
            {
                string html = url.GetHtml(true, 3);

                if (string.IsNullOrWhiteSpace(html))
                {
                    result.NotFound = true;
                    throw new InvalidOperationException(string.Format("Response data is empty or page is not found for url \"{0}\".", url));
                }

                result.Html = html;

                HtmlDocument doc = HtmlParser.Parse(html, result.HtmlHashCode);

                TParserInfo[] data = Parse(url, doc);

                if (data == null)
                    throw new InvalidOperationException(string.Format("Parse \"{0}\" returned null.", GetType().FullName));

                result.Data = data;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
                result.Error = ex.ToTraceString();
            }
            return result;
        }
    }
}