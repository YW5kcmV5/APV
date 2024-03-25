using System;
using System.Collections.Generic;
using System.Text;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
{
    public class KeywordDataLayerManager : BaseDataLayerManager<KeywordEntity, KeywordCollection>
    {
        public KeywordEntity Find(long entityTypeId, long entityId)
        {
            const string whereSql = @"WHERE (EntityTypeId = @EntityTypeId) AND (EntityId = @EntityId)";
            var @params = new Dictionary<string, object>
                {
                    { "@EntityTypeId", entityTypeId },
                    { "@EntityId", entityId }
                };

            return Find(whereSql, @params);
        }

        public KeywordEntity Get(long entityTypeId, long entityId)
        {
            KeywordEntity entity = Find(entityTypeId, entityId);

            if (entity == null)
                throw new InvalidOperationException(string.Format("Specified KeywordEntity does not exist (entityTypeId=\"{0}\";entityId=\"{1}\").", entityTypeId, entityId));

            return entity;
        }

        public void Delete(long entityTypeId, long entityId)
        {
            const string whereSql = @"
DECLARE @KeywordId bigint = (SELECT [Keyword].[KeywordId] FROM [Keyword] WHERE (EntityTypeId = @EntityTypeId) AND (EntityId = @EntityId));
DELETE [KeywordReference] WHERE ([KeywordReference].KeywordId = @KeywordId);
DELETE [Keyword] WHERE ([Keyword].KeywordId = @KeywordId);";
            var @params = new Dictionary<string, object>
                {
                    { "@EntityTypeId", entityTypeId },
                    { "@EntityId", entityId }
                };

            Execute(whereSql, @params);
        }

        public KeywordCollection Search(string[] words, long entityTypeId)
        {
            if ((words == null) || (words.Length == 0))
            {
                return new KeywordCollection();
            }

            var @params = new Dictionary<string, object>
                {
                    { "@EntityTypeId", entityTypeId }
                };

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

            string sql = string.Format(
@"JOIN [KeywordReference] ON [KeywordReference].KeywordId = [Keyword].KeywordId
	WHERE
		[Keyword].EntityTypeId = @EntityTypeId AND
		[KeywordReference].Word IN ({0})
	GROUP BY
		[Keyword].KeywordId, [Keyword].EntityId, [Keyword].EntityTypeId
	ORDER BY SUM([KeywordReference].Points)", paramsList);

            return GetList(sql, @params);
        }

        public KeywordCollection Search(string[] words)
        {
            if ((words == null) || (words.Length == 0))
            {
                return new KeywordCollection();
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

            string sql = string.Format(
@"JOIN [KeywordReference] ON [KeywordReference].KeywordId = [Keyword].KeywordId
	WHERE
		[KeywordReference].Word IN ({0})
	GROUP BY
		[Keyword].KeywordId, [Keyword].EntityId, [Keyword].EntityTypeId
	ORDER BY SUM([KeywordReference].Points)", paramsList);

            return GetList(sql, @params);
        }
    }
}