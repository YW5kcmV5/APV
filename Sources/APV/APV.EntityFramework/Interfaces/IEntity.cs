using System;

namespace APV.EntityFramework.Interfaces
{
    public interface IEntity : IIdentifier, ICloneable
    {
        long TypeId { get; }

        bool IsNew { get; }

        byte[] Serialize();

        void Deserialize(byte[] dump);

        bool Equals(IEntity entity);

        IEntity Owner { get; }

        IEntityCollection Container { get; }
    }
}