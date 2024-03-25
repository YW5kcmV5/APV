using System;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
{
    public class FileDataLayerManager : BaseDataLayerManager<DataFileEntity, DataFileCollection>
    {
        public DataFileEntity Get(Guid tag)
        {
            throw new NotImplementedException();
        }

        public DataFileEntity Get(string name)
        {
            throw new NotImplementedException();
        }
    }
}