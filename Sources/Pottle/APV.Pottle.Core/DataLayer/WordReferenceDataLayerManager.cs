using System.Collections.Generic;
using System.Text;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
{
    public class WordReferenceDataLayerManager : BaseDataLayerManager<WordReferenceEntity, WordReferenceCollection>
    {
        public WordReferenceCollection GetReferences(string[] words)
        {
            if ((words == null) || (words.Length == 0))
            {
                return new WordReferenceCollection();
            }

            var @params = new Dictionary<string, object>();
            var paramsList = new StringBuilder();
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (!string.IsNullOrWhiteSpace(word))
                {
                    string paramName = string.Format("@Param{0:000}", i);
                    paramsList.Append(paramName);
                    paramsList.Append(", ");
                    @params.Add(paramName, word);
                }
            }
            paramsList.Length -= 2;

            string whereSql = string.Format(@"JOIN [Word] ON [WordReference].WordId = [Word].WordId WHERE [Word].Name IN ({0})", paramsList);

            return GetList(whereSql, @params);
        }

        public WordReferenceCollection GetParentReferences(long wordId)
        {
            const string whereSql = @"WHERE ([WordReference].[WordId] = @wordId)";
            var @params = new Dictionary<string, object>
                {
                    { "@wordId", wordId },
                };
            return GetList(whereSql, @params);
        }

        public WordReferenceCollection GetOriginalReferences(long wordId)
        {
            const string whereSql = @"WHERE (([WordReference].[WordId] = @wordId) AND ([WordReference].[ReferenceType] = @referenceType))";
            var @params = new Dictionary<string, object>
                {
                    {"@wordId", wordId},
                    {"@referenceType", WordReferenceType.Original}
                };
            return GetList(whereSql, @params);
        }

        public WordReferenceCollection GetChildReferences(long referenceWordId)
        {
            const string whereSql = @"WHERE [WordReference].[ReferenceWordId] = @referenceWordId";
            var @params = new Dictionary<string, object>
                {
                    { "@referenceWordId", referenceWordId },
                };
            return GetList(whereSql, @params);
        }
    }
}