using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using APV.Common.Extensions;

namespace APV.Common.Html
{
    public static class HtmlParser
    {
        private enum PositionType
        {
            Open,

            Close,

            OpenAndClose
        }

        private enum TagType
        {
            Regular,

            Comment,

            Doctype,

            Script,

            Br
        }

        private class TagPosition
        {
            public int FirstIndex;

            public int LastIndex;

            public PositionType PositionType;

            public TagType TagType;

            public string TagName;

            public string Attributes;
        }

        [DebuggerDisplay("{ToDebugString()}")]
        private class TagInfo
        {
            public TagPosition OpenTag;

            public TagPosition CloseTag;

            public readonly List<TagInfo> Children = new List<TagInfo>();

            public HtmlTag Fill(HtmlTag parent, string html, ref int tagId)
            {
                string tagName = (OpenTag != null) ? OpenTag.TagName : "html";
                string innerHtml;
                string innerText;

                if ((OpenTag == null) || (CloseTag == null))
                {
                    innerHtml = html;
                    innerText = innerHtml;
                }
                else if (OpenTag.PositionType == PositionType.OpenAndClose)
                {
                    innerHtml = string.Empty;
                    innerText = string.Empty;
                }
                else
                {
                    int length = CloseTag.FirstIndex - OpenTag.LastIndex - 1;
                    innerHtml = html.Substring(OpenTag.LastIndex + 1, length);

                    if (Children.Count == 0)
                    {
                        innerText = innerHtml;
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        for (var i = 0; i < Children.Count; i++)
                        {
                            bool first = (i == 0);
                            bool last = (i == Children.Count - 1);
                            int firstIndex = (first) ? OpenTag.LastIndex + 1 : Children[i - 1].CloseTag.LastIndex + 1;
                            int lastIndex = Children[i].OpenTag.FirstIndex - 1;
                            int textLength = lastIndex - firstIndex + 1;
                            string text = html.Substring(firstIndex, textLength);
                            sb.Append(text);
                            if (last)
                            {
                                firstIndex = Children[i].CloseTag.LastIndex + 1;
                                lastIndex = CloseTag.FirstIndex - 1;
                                textLength = lastIndex - firstIndex + 1;
                                text = html.Substring(firstIndex, textLength);
                                sb.Append(text);
                            }
                        }
                        innerText = sb.ToString();
                    }
                }

                var tag = new HtmlTag(tagId, parent)
                    {
                        TagName = tagName,
                        InnerHtml = innerHtml,
                        InnerText = innerText,
                    };
                tagId++;

                if (OpenTag != null)
                {
                    tag.SetAttributes(OpenTag.Attributes);
                }

                foreach (TagInfo childTagInfo in Children)
                {
                    HtmlTag childTag = childTagInfo.Fill(tag, html, ref tagId);
                    tag.Children.Add(childTag);
                }

                return tag;
            }

            public string ToDebugString()
            {
                if (OpenTag.PositionType == PositionType.OpenAndClose)
                {
                    return $"<{OpenTag.TagName}/>";
                }
                if (OpenTag.PositionType == PositionType.Open)
                {
                    return $"<{OpenTag.TagName}>";
                }
                if (OpenTag.PositionType == PositionType.Close)
                {
                    return $"</{OpenTag.TagName}>";
                }
                return "*";
            }
        }

        private static TagPosition GetNextTagPosition(string html, int startIndex)
        {
            int firstIndex = html.IndexOf("<", startIndex, StringComparison.InvariantCultureIgnoreCase);
            if (firstIndex != -1)
            {
                int lastIndex = html.IndexOf(">", firstIndex, StringComparison.InvariantCultureIgnoreCase);
                if (lastIndex != -1)
                {
                    PositionType tagPositionType;
                    var tagType = TagType.Regular;
                    string tagNameWithAttributes = html.Substring(firstIndex + 1, lastIndex - firstIndex - 1).Trim();
                    string lowTagNameWithAttributes = tagNameWithAttributes.ToLowerInvariant();
                    if (lowTagNameWithAttributes.StartsWith("!--"))
                    {
                        //Comment
                        lastIndex = html.IndexOf("-->", firstIndex, StringComparison.InvariantCultureIgnoreCase) + 2;
                        lowTagNameWithAttributes = html.Substring(firstIndex + 4, lastIndex - firstIndex - 6).Trim().ToLowerInvariant();
                        tagPositionType = PositionType.OpenAndClose;
                        tagType = TagType.Comment;
                    }
                    else if (lowTagNameWithAttributes.StartsWith("!"))
                    {
                        //doc type
                        lowTagNameWithAttributes = lowTagNameWithAttributes.Substring(1, lowTagNameWithAttributes.Length - 1).Trim();
                        tagPositionType = PositionType.OpenAndClose;
                        tagType = TagType.Doctype;
                    }
                    else if (lowTagNameWithAttributes.StartsWith("script"))
                    {
                        //script
                        lastIndex = html.IndexOf("</script>", firstIndex, StringComparison.InvariantCultureIgnoreCase) + 8;
                        lowTagNameWithAttributes = html.Substring(firstIndex + 7, lastIndex - firstIndex - 16).Trim().ToLowerInvariant();
                        tagPositionType = PositionType.OpenAndClose;
                        tagType = TagType.Script;
                    }
                    else if (lowTagNameWithAttributes.StartsWith("br"))
                    {
                        //script
                        lowTagNameWithAttributes = string.Empty;
                        tagPositionType = PositionType.OpenAndClose;
                        tagType = TagType.Br;
                    }
                    else if (lowTagNameWithAttributes.EndsWith("/"))
                    {
                        lowTagNameWithAttributes = lowTagNameWithAttributes.Substring(0, lowTagNameWithAttributes.Length - 1).Trim();
                        tagPositionType = PositionType.OpenAndClose;
                    }
                    else if (lowTagNameWithAttributes.StartsWith("/"))
                    {
                        lowTagNameWithAttributes = lowTagNameWithAttributes.Substring(1, lowTagNameWithAttributes.Length - 1).Trim();
                        tagPositionType = PositionType.Close;
                    }
                    else
                    {
                        tagPositionType = PositionType.Open;
                    }

                    string tagName = lowTagNameWithAttributes;
                    string attributes = string.Empty;
                    if (tagType == TagType.Regular)
                    {
                        int attributeIndex = lowTagNameWithAttributes.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase);
                        if (attributeIndex != -1)
                        {
                            tagName = lowTagNameWithAttributes.Substring(0, attributeIndex);
                            attributes = tagNameWithAttributes.Substring(attributeIndex + 1, tagNameWithAttributes.Length - attributeIndex - 1);
                            //TODO: parse attributes
                        }
                        else
                        {
                            tagName = lowTagNameWithAttributes;
                            attributes = string.Empty;
                        }
                    }
                    else if (tagType == TagType.Comment)
                    {
                        tagName = "comment";
                        attributes = tagNameWithAttributes;
                    }
                    else if (tagType == TagType.Script)
                    {
                        tagName = "script";
                        attributes = tagNameWithAttributes;
                    }
                    else if (tagType == TagType.Doctype)
                    {
                        tagName = "doctype";
                        attributes = tagNameWithAttributes;
                    }
                    else if (tagType == TagType.Br)
                    {
                        tagName = "br";
                        attributes = string.Empty;
                    }

                    return new TagPosition
                        {
                            TagName = tagName,
                            Attributes = attributes,
                            PositionType = tagPositionType,
                            FirstIndex = firstIndex,
                            LastIndex = lastIndex,
                            TagType = tagType,
                        };
                }
            }
            return null;
        }

        public static HtmlDocument Parse(string html, byte[] htmlHashCode = null)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return null;
            }

            html = html.Trim();
            int startIndex = 0;
            
            var stack = new Stack<TagInfo>();
            var root = new TagInfo();
            stack.Push(root);

            while (true)
            {
                TagPosition tagPosition = GetNextTagPosition(html, startIndex);
                if ((tagPosition == null) || (stack.Count == 0))
                {
                    break;
                }

                PositionType positionType = tagPosition.PositionType;

                startIndex = tagPosition.LastIndex;

                TagInfo lastTagInfo = stack.Peek();

                if (positionType == PositionType.Open)
                {
                    var tagInfo = new TagInfo
                        {
                            OpenTag = tagPosition,
                            CloseTag = null,
                        };
                    stack.Push(tagInfo);
                    lastTagInfo.Children.Add(tagInfo);
                }
                else if (positionType == PositionType.Close)
                {
                    if (lastTagInfo.OpenTag.TagName != tagPosition.TagName)
                    {
                        bool exists = false;
                        TagInfo[] elements = stack.ToArray();
                        for (var i = 1; i < elements.Length - 1; i++)
                        {
                            if (elements[i].OpenTag.TagName == tagPosition.TagName)
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            continue;
                        }

                        while (true)
                        {
                            //Close previous tag while the correct tag name will not be found
                            if (lastTagInfo.OpenTag.TagName == tagPosition.TagName)
                            {
                                break;
                            }
                            TagInfo tagToClose = stack.Pop();
                            tagToClose.OpenTag.PositionType = PositionType.OpenAndClose;
                            tagToClose.CloseTag = tagToClose.OpenTag;
                            lastTagInfo = stack.Peek();
                        }
                    }

                    lastTagInfo.CloseTag = tagPosition;
                    stack.Pop();
                }
                else if (positionType == PositionType.OpenAndClose)
                {
                    var tagInfo = new TagInfo
                        {
                            OpenTag = tagPosition,
                            CloseTag = tagPosition,
                        };
                    lastTagInfo.Children.Add(tagInfo);
                }
            }

            int tagId = 0;
            HtmlTag htmlTag = root.Fill(null, html, ref tagId);
            htmlHashCode = htmlHashCode ?? html.Hash256();
            var document = new HtmlDocument(htmlHashCode, htmlTag.Children, true);
            return document;
        }
    }
}