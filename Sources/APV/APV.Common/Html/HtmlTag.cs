using System;
using System.Collections.Generic;
using System.Linq;

namespace APV.Common.Html
{
    public sealed class HtmlTag
    {
        private readonly List<HtmlTag> _children = new List<HtmlTag>();
        private readonly List<string> _classes = new List<string>();
        private readonly List<HtmlAttribute> _attributes = new List<HtmlAttribute>();

        private string FormatString(string value)
        {
            value = value ?? string.Empty;
            value = value.Replace("&nbsp;", " ");
            value = value.Replace("&quot;", "\"");
            value = value.Replace("&ndash;", "–");
            value = value.Replace("&#8209;", "–");
            value = value.Trim();
            return value;
        }

        private int GetIndex()
        {
            if ((Parent != null) && (Parent.Children != null))
            {
                for (int i = 0; i < Parent.Children.Count; i++)
                {
                    if (Parent.Children[i] == this)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public HtmlTag(int tagId, HtmlTag parent)
        {
            TagId = tagId;
            Parent = parent;
        }

        public void SetAttributes(string attributes)
        {
            _attributes.Clear();
            _classes.Clear();
            Id = null;

            attributes = (attributes ?? string.Empty).Trim();

            string[] items = attributes.Split(new[] { "\" ", "\' " }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < items.Length; i++)
            {
                string item = items[i];
                string[] nameValue = item.Split(new[] { "='", "=\"" }, StringSplitOptions.RemoveEmptyEntries);
                string name = item;
                string value = null;
                if (nameValue.Length == 2)
                {
                    name = nameValue[0].Trim();
                    value = nameValue[1];
                    if (i == items.Length - 1)
                    {
                        value = value.Substring(0, value.Length - 1);
                    }
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    value = value ?? string.Empty;
                    var attribute = new HtmlAttribute
                    {
                        Name = name,
                        Value = value,
                    };
                    _attributes.Add(attribute);
                    if (string.Compare(name, "class", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        string[] values = value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        for (var j = 0; j < values.Length; j++)
                        {
                            string className = values[j].Trim().ToLowerInvariant();
                            if ((!string.IsNullOrWhiteSpace(className)) && (!_classes.Contains(className)))
                            {
                                _classes.Add(className);
                            }
                        }
                    }
                    if ((string.Compare(name, "id", StringComparison.InvariantCultureIgnoreCase) == 0) && (!string.IsNullOrWhiteSpace(value)))
                    {
                        Id = value;
                    }
                }
            }
        }

        public string GetAttributeValue(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            string value = string.Join(" ", _attributes.Where(attribute => attribute.Name == name).Select(attribute => attribute.Value));
            value = value.Trim();
            return value;
        }

        public List<HtmlTag> Find(string pattern)
        {
            var container = new HtmlContainer(Children);
            return container.Find(pattern);
        }

        public int TagId { get; private set; }

        public string TagName { get; set; }

        public string Id { get; private set; }

        public string GetClassName()
        {
            return string.Join(" ", _classes);
        }

        public List<string> Classes
        {
            get { return _classes; }
        }

        public string InnerHtml { get; set; }

        public string InnerText { get; set; }

        public string Text
        {
            get { return FormatString(InnerText); }
        }

        public HtmlTag Parent { get; private set; }

        public HtmlTag PreviousSibling 
        {
            get
            {
                int index = GetIndex();
                return ((index != -1) && (index > 0)) ? Parent.Children[index - 1] : null;
            }
        }

        public HtmlTag NextSibling
        {
            get
            {
                int index = GetIndex();
                return ((index != -1) && (index < Parent.Children.Count - 1)) ? Parent.Children[index + 1] : null;
            }
        }

        public List<HtmlTag> Children
        {
            get { return _children; }
        }
    }
}