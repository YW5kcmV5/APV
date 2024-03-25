using System;

namespace APV.EntityFramework.DataLayer
{
    public sealed class TransactionScope : IDisposable
    {
        private bool _disposed;
        private SqlProvider _provider;
        private bool _commited;
        private bool _rollbacked;
        private readonly long _index;

        private void Rollback()
        {
            if (_disposed)
                throw new InvalidOperationException("Transaction scope is disposed.");
            if (_commited)
                throw new InvalidOperationException("The already commited transaction can not be rollback.");
            if (_rollbacked)
                throw new InvalidOperationException("The already rollbacked transaction can not be rollback.");

            _rollbacked = true;
            _provider.Rollback(this);
        }

        public TransactionScope()
            : this(SqlProvider.ThreadInstance)
        {
        }

        public TransactionScope(SqlProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            _provider = provider;
            _index = provider.GetTransactionIndex();
            _provider.AddTransaction(this);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if ((!_commited) && (!_rollbacked))
                {
                    Rollback();
                }
                _disposed = true;
                _provider = null;
            }
        }

        public void Commit()
        {
            if (_disposed)
                throw new InvalidOperationException("Transaction scope is disposed.");
            if (_commited)
                throw new InvalidOperationException("The already rollbacked transaction can not be commit.");
            if (_rollbacked)
                throw new InvalidOperationException("The already commited transaction can not be commit.");

            _commited = true;
            _provider.Commit(this);
        }

        #region Properties

        public SqlProvider Provider
        {
            get { return _provider; }
        }

        public bool Commited
        {
            get { return _commited; }
        }

        public bool Rollbacked
        {
            get { return _rollbacked; }
        }

        public long Index
        {
            get { return _index; }
        }

        #endregion
    }
}