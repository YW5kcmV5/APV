using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
{
    public class WordDataLayerManager : BaseDataLayerManager<WordEntity, WordCollection>
    {
        public void Clear()
        {
            const string sql =
@"DELETE FROM [WordReference];
DELETE FROM [Word]";
            Execute(sql);
        }
    }
}