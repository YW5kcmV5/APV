using APV.EntityFramework;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class KeywordReferenceManagement : BaseManagement<KeywordReferenceEntity, KeywordReferenceCollection, KeywordReferenceDataLayerManager>
    {
        public static readonly KeywordReferenceManagement Instance = (KeywordReferenceManagement)EntityFrameworkManager.GetManagement<KeywordReferenceEntity>();
    }
}