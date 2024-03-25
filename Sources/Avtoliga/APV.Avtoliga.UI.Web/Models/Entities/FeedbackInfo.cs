using APV.Avtoliga.Common;
using APV.Common.Extensions;

namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class FeedbackInfo : BaseInfo
    {
        public long FeedbackId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Text { get; set; }

        public string TextP { get; set; }

        public FeedbackType Type { get; set; }

        public string CreatedAt { get; set; }

        public int Likes { get; set; }

        public bool Liked { get; set; }

        public bool IsValid()
        {
            return (!string.IsNullOrWhiteSpace(Name)) && (!string.IsNullOrWhiteSpace(Email)) &&
                   (!string.IsNullOrWhiteSpace(Text)) && (Email.IsValidEmail());
        }
    }
}