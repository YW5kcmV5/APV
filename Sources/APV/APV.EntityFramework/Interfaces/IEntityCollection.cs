using System.Collections.Generic;

namespace APV.EntityFramework.Interfaces
{
    public interface IEntityCollection : IList<IEntity>
    {
        void SetReadOnly();

        IEntity Owner { get; }
    }
}