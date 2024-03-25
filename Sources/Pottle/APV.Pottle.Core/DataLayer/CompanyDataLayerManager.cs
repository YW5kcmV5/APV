using System;
using System.Collections.Generic;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
{
    public class CompanyDataLayerManager : BaseDataLayerManager<CompanyEntity, CompanyCollection>
    {
        public CompanyEntity FindByAnyName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            const string sql = @"WHERE [Company].[Name] = @Name OR [Company].[LegalName] = @Name OR [Company].[CompanyName] = @Name";
            var @params = new Dictionary<string, object>
                {
                    { "@Name", name },
                };

            return Find(sql, @params);
        }
    }
}