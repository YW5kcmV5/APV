using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class WordReferenceManagement : BaseManagement<WordReferenceEntity, WordReferenceCollection, WordReferenceDataLayerManager>
    {
        [ClientAccess]
        public WordReferenceCollection GetReferences(string[] words)
        {
            return DatabaseManager.GetReferences(words);
        }

        [ClientAccess]
        public WordReferenceCollection GetOriginalReferences(long wordId)
        {
            return DatabaseManager.GetOriginalReferences(wordId);
        }

        [ClientAccess]
        public WordReferenceCollection GetParentReferences(long wordId)
        {
            return DatabaseManager.GetParentReferences(wordId);
        }

        [ClientAccess]
        public WordReferenceCollection GetChildReferences(long wordId)
        {
            return DatabaseManager.GetChildReferences(wordId);
        }

        public static readonly WordReferenceManagement Instance = (WordReferenceManagement)EntityFrameworkManager.GetManagement<WordReferenceEntity>();
    }
}