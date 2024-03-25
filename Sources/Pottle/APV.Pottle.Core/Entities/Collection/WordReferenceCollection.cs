using System;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common;

namespace APV.Pottle.Core.Entities.Collection
{
    public class WordReferenceCollection : BaseEntityCollection<WordReferenceEntity>
    {
        #region Constructors

        public WordReferenceCollection()
        {
        }

        public WordReferenceCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion

        public WordReferenceCollection Where(PartOfSpeech partOfSpeech)
        {
            var result = new WordReferenceCollection();
            for (int i = 0; i < Count; i++)
            {
                WordReferenceEntity entity = this[i];
                if (entity.PartOfSpeech == partOfSpeech)
                {
                    result.Add(entity);
                }
            }
            return result;
        }

        public WordReferenceEntity Find(long referenceWordId, PartOfSpeech partOfSpeech, WordReferenceType referenceType)
        {
            for (int i = 0; i < Count; i++)
            {
                WordReferenceEntity entity = this[i];
                if ((entity.ReferenceWordId == referenceWordId) && (entity.PartOfSpeech == partOfSpeech) && (entity.ReferenceType == referenceType))
                {
                    return entity;
                }
            }
            return null;
        }

        public WordReferenceCollection Where(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var result = new WordReferenceCollection();
            for (int i = 0; i < Count; i++)
            {
                WordReferenceEntity entity = this[i];
                if (entity.Word.Name == name)
                {
                    result.Add(entity);
                }
            }
            return result;
        }

        public WordReferenceCollection WhereReference(string referenceName)
        {
            if (string.IsNullOrEmpty(referenceName))
                throw new ArgumentNullException("referenceName");

            var result = new WordReferenceCollection();
            for (int i = 0; i < Count; i++)
            {
                WordReferenceEntity entity = this[i];
                if (entity.ReferenceName == referenceName)
                {
                    result.Add(entity);
                }
            }
            return result;
        }

        public bool Contains(string name)
        {
            WordReferenceCollection references = Where(name);
            return (references.Count > 0);
        }

        public bool Contains(PartOfSpeech partOfSpeech)
        {
            WordReferenceCollection references = Where(partOfSpeech);
            return (references.Count > 0);
        }

        public bool ContainsReference(string referenceName)
        {
            WordReferenceCollection references = WhereReference(referenceName);
            return (references.Count > 0);
        }

        public PartsOfSpeech GetPartsOfSpeech()
        {
            var partsOfSpeech = PartsOfSpeech.Unknown;
            for (int i = 0; i < Count; i++)
            {
                WordReferenceEntity entity = this[i];
                if ((entity.ReferenceType == WordReferenceType.Original) ||
                    (entity.ReferenceType == WordReferenceType.Parent) ||
                    (entity.ReferenceType == WordReferenceType.ParentName))
                    partsOfSpeech |= (PartsOfSpeech) entity.PartOfSpeech;
            }
            return partsOfSpeech;
        }
    }
}