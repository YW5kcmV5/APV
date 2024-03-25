using System;

namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class ErrorInfo : BaseInfo
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public int StatusCode { get; set; }

        public Exception Exception { get; set; }

        public bool NotFound
        {
            get { return (StatusCode == 404); }
        }
    }
}