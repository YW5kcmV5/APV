using System.Linq;
using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class LanguageCollection : BaseEntityCollection<LanguageEntity>
    {
        #region Constructors

        public LanguageCollection()
        {
        }

        public LanguageCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion

        public char[] GetWordChars()
        {
            return this
                .Cast<LanguageEntity>()
                .Where(language => !string.IsNullOrWhiteSpace(language.WordChars))
                .SelectMany(lan => lan.WordChars.ToArray())
                .ToArray();
        }
    }
}