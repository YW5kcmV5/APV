using System;
using System.Collections.Generic;
using System.Linq;

namespace APV.Common.Html
{
    public class HtmlContainer
    {
        private readonly List<HtmlTag> _children = new List<HtmlTag>();
        private readonly SortedList<string, List<HtmlTag>> _id = new SortedList<string, List<HtmlTag>>();
        private readonly SortedList<string, List<HtmlTag>> _tagName = new SortedList<string, List<HtmlTag>>();
        private readonly SortedList<string, List<HtmlTag>> _class = new SortedList<string, List<HtmlTag>>();

        private void AddToId(HtmlTag tag)
        {
            string id = tag.Id;
            if (!string.IsNullOrWhiteSpace(id))
            {
                int index = _id.IndexOfKey(id);
                if (index != -1)
                {
                    _id.Values[index].Add(tag);
                }
                else
                {
                    _id.Add(id, new List<HtmlTag> { tag });
                }
            }
        }

        private void AddToTagName(HtmlTag tag)
        {
            List<string> classes = tag.Classes;
            if (classes.Contains("item_area"))
            {
            }

            string tagName = tag.TagName;
            if (!string.IsNullOrWhiteSpace(tagName))
            {
                int index = _tagName.IndexOfKey(tagName);
                if (index != -1)
                {
                    _tagName.Values[index].Add(tag);
                }
                else
                {
                    _tagName.Add(tagName, new List<HtmlTag> { tag });
                }
            }
        }

        private void AddToClassName(HtmlTag tag)
        {
            List<string> classes = tag.Classes;

            foreach (string className in classes)
            {
                if (!string.IsNullOrWhiteSpace(className))
                {
                    int index = _class.IndexOfKey(className);
                    if (index != -1)
                    {
                        _class.Values[index].Add(tag);
                    }
                    else
                    {
                        _class.Add(className, new List<HtmlTag> { tag });
                    }
                }
            }
        }

        private void Fill(HtmlTag tag, bool recursive)
        {
            AddToId(tag);
            AddToTagName(tag);
            AddToClassName(tag);

            if (recursive)
            {
                foreach (HtmlTag childTag in tag.Children)
                {
                    Fill(childTag, true);
                }
            }
        }

        private List<HtmlTag> FindLeft(string pattern)
        {
            string id = null;
            string tagName = null;
            int? collectionIndex = null;
            List<string> classes = null;

            if (!string.IsNullOrWhiteSpace(pattern))
            {
                pattern = pattern.Trim();

                if (pattern.EndsWith("]"))
                {
                    int index = pattern.IndexOf("[");
                    if (index != -1)
                    {
                        string collectionIndexValue = pattern.Substring(index + 1, pattern.Length - index - 2);
                        pattern = pattern.Substring(0, index);
                        int value;
                        if ((!string.IsNullOrWhiteSpace(collectionIndexValue)) && (int.TryParse(collectionIndexValue, out value)) && (value >= 0))
                        {
                            collectionIndex = value;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(pattern))
                {
                    int index = pattern.IndexOf(".");
                    if (index != -1)
                    {
                        string classesString = pattern.Substring(index, pattern.Length - index);
                        classes = classesString.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries).ToList();
                        pattern = pattern.Substring(0, index);
                    }

                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        index = pattern.IndexOf("#");
                        if (index != -1)
                        {
                            id = pattern.Substring(index + 1);
                            pattern = pattern.Substring(0, index);
                        }

                        if (!string.IsNullOrWhiteSpace(pattern))
                        {
                            tagName = pattern;
                        }
                    }
                }

                //Search
                List<HtmlTag> result = FindTag(id, tagName, classes);
                if (collectionIndex != null)
                {
                    result = (collectionIndex.Value < result.Count)
                        ? new List<HtmlTag> { result[collectionIndex.Value] }
                        : new List<HtmlTag>();
                }
                return result;
            }
            return new List<HtmlTag>();
        }

        public HtmlContainer(IEnumerable<HtmlTag> children, bool recursive = false)
        {
            if (children == null)
                throw new ArgumentNullException("children");

            foreach (HtmlTag tag in children)
            {
                Add(tag, recursive);
            }
        }

        public void Add(HtmlTag tag, bool recursive)
        {
            if (tag != null)
            {
                Fill(tag, recursive);
                Children.Add(tag);
            }
        }

        public List<HtmlTag> FindByTagName(string tagName)
        {
            var result = new List<HtmlTag>();
            if (!string.IsNullOrEmpty(tagName))
            {
                int index = _tagName.IndexOfKey(tagName);
                if (index != -1)
                {
                    result.AddRange(_tagName.Values[index]);
                }
            }
            return result;
        }

        public List<HtmlTag> FindById(string id)
        {
            var result = new List<HtmlTag>();
            if (!string.IsNullOrEmpty(id))
            {
                int index = _id.IndexOfKey(id);
                if (index != -1)
                {
                    result.AddRange(_id.Values[index]);
                }
            }
            return result;
        }

        public List<HtmlTag> FindByClass(string className)
        {
            var result = new List<HtmlTag>();
            if (!string.IsNullOrEmpty(className))
            {
                int index = _class.IndexOfKey(className);
                if (index != -1)
                {
                    result.AddRange(_class.Values[index]);
                }
            }
            return result;
        }

        public List<HtmlTag> FindByClass(IEnumerable<string> classes)
        {
            var result = new List<HtmlTag>();
            if (classes != null)
            {
                foreach (string className in classes)
                {
                    result.AddRange(FindByClass(className));
                }
            }
            return result;
        }

        public List<HtmlTag> FindTag(string id, string tagName = null, IEnumerable<string> classes = null)
        {
            var result = new List<HtmlTag>();
            if (!string.IsNullOrEmpty(id))
            {
                int index = _id.IndexOfKey(id);
                if (index == -1)
                {
                    return result;
                }
                var container = new HtmlContainer(_id.Values[index]);
                return container.FindTag(null, tagName, classes);
            }
            
            if (!string.IsNullOrEmpty(tagName))
            {
                int index = _tagName.IndexOfKey(tagName);
                if (index == -1)
                {
                    return result;
                }
                List<HtmlTag> items = _tagName.Values[index];
                var container = new HtmlContainer(items);
                return container.FindTag(null, null, classes);
            }

            string[] classNames = (classes != null) ? classes.Where(className => !string.IsNullOrEmpty(className)).ToArray() : new string[0];
            if (classNames.Length > 0)
            {
                string className = classNames[0];
                classNames = classNames.Skip(1).ToArray();
                int index = _class.IndexOfKey(className);
                if (index == -1)
                {
                    return result;
                }
                var container = new HtmlContainer(_class.Values[index]);
                return container.FindTag(null, null, classNames);
            }

            result.AddRange(Children);
            return result;
        }

        //pattern:
        //  #id [pattern]
        //  .className [pattern]
        //  tagName [pattern]
        //  #id.className [pattern]
        //  tagName.#id [pattern]
        //  tagName.#id.className [pattern]
        //  tagName.className [pattern]
        public List<HtmlTag> Find(string pattern)
        {
            pattern = pattern ?? string.Empty;
            string[] elements = pattern.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string first = elements[0];
            List<HtmlTag> left = FindLeft(first);

            string last = (elements.Length > 1) ? string.Join(" ", elements.Skip(1)) : null;
            if (string.IsNullOrEmpty(last))
            {
                return left;
            }

            List<HtmlTag> children = left.SelectMany(tag => tag.Children).ToList();
            var container = new HtmlContainer(children, true);
            List<HtmlTag> result = container.Find(last);

            return result;
        }

        public List<HtmlTag> Children
        {
            get { return _children; }
        }
    }

    public sealed class HtmlDocument : HtmlContainer
    {
        public HtmlDocument(byte[] htmlHashCode, IEnumerable<HtmlTag> children, bool recursive)
            : base(children, recursive)
        {
            if (htmlHashCode == null)
                throw new ArgumentNullException("htmlHashCode");

            HtmlHashCode = htmlHashCode;
        }

        public byte[] HtmlHashCode { get; private set; }

        public static HtmlDocument Load(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            string html = HtmlUtility.GetHtml(url);
            return HtmlParser.Parse(html);
        }
    }
}