using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Remoting.Contexts;
using APV.Common;
using APV.DatabaseAccess.Providers;
using APV.EntityFramework.Interfaces;
using APV.Common.Timers;

namespace APV.EntityFramework.DataLayer
{
    public sealed class SqlProvider : IContextProperty, IDisposable
    {
        #region Constants

        /// <summary>
        /// "DataLayer.SqlProvider"
        /// </summary>
        /// 
        public const string SqlProviderThreadPropertyName = "DataLayer.SqlProvider";

        #endregion

        #region Private

        [ThreadStatic]
        private static SqlProvider _provider;

        private static readonly IContextManager ContextManager = EntityFrameworkManager.GetContextManager();
        private static readonly SqlProviderSettings SqlSettings = ContextManager.GetSqlSettings() ?? SqlProviderSettings.Default;
        private static readonly object GlobalLock = new object();
        private static long _globalId;
        private readonly SqlProviderLog _log = new SqlProviderLog();
        private readonly object _lock = new object();
        private readonly Stack<TransactionScope> _transactions = new Stack<TransactionScope>();
        private readonly long _id;
        private bool _disposed;
        private DbSqlConnection _connection;
        private int _transactionIndex;
        private bool _inScope;
        private bool _rollbacked;
        private bool _commited;
        private WatchdogTimer _watchdogTimer;
        private bool _watchdogException;

        private static SqlProvider GetThreadInstance()
        {
            lock (GlobalLock)
            {
                return _provider ?? (_provider = new SqlProvider(_globalId++));
            }
        }

        private SqlProvider(long id)
        {
            _id = id;
        }

        private static DbSqlConnection CreateConnection()
        {
            string connectionStrig = ContextManager.GetConnectionString();
            var connection = new DbSqlConnection(connectionStrig);
            connection.Open();
            return connection;
        }

        private DbSqlConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = CreateConnection();
                //99%
                int timeoutInMlSec = 990*_connection.ConnectionTimeout;
                if (timeoutInMlSec > 0)
                {
                    _watchdogTimer = new WatchdogTimer(timeoutInMlSec, OnWatchdog);
                }
            }
            else if (_watchdogTimer != null)
            {
                _watchdogTimer.Reset();
            }
            return _connection;
        }

        private void Execute(string sql)
        {
            if (_connection == null)
            {
                sql = SqlSettings.SqlCommandSettings + sql;
                _log.AddSql(sql);
                _log.IncreaseIdent();
            }
            else
            {
                _log.AddSql(sql);
            }
            DbSqlConnection connection = GetConnection();
            DbSqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        private void BeginTransaction()
        {
            if (!_inScope)
            {
                _log.IncreaseIdent();
                string sql = string.Format("BEGIN TRAN T{0};", _id);
                Execute(sql);
                _inScope = true;
            }
        }

        private void CommitTransaction()
        {
            if (_inScope)
            {
                string sql = string.Format("COMMIT TRAN T{0};", _id);
                _log.DecreaseIdent();
                Execute(sql);
                _log.DecreaseIdent();
                _connection.Close();
                _connection = null;
                _inScope = false;
                _commited = true;
                if (_watchdogTimer != null)
                {
                    _watchdogTimer.Dispose();
                    _watchdogTimer = null;
                }
            }
        }

        private void OnWatchdog()
        {
            lock (_lock)
            {
                if (_inScope)
                {
                    _watchdogException = true;
                    Rollback();
                }
            }
        }

        private void RollbackTransaction()
        {
            if (_inScope)
            {
                string sql = string.Format("ROLLBACK TRAN T{0};", _id);
                _log.DecreaseIdent();
                Execute(sql);
                _log.DecreaseIdent();
                _connection.Close();
                _connection = null;
                _inScope = false;
                _rollbacked = true;
                if (_watchdogTimer != null)
                {
                    _watchdogTimer.Dispose();
                    _watchdogTimer = null;
                }
            }
        }

        private void Rollback()
        {
            lock (_lock)
            {
                if (_watchdogTimer != null)
                {
                    _watchdogTimer.Dispose();
                    _watchdogTimer = null;
                }
                if (_transactions.Count > 0)
                {
                    _transactions.Clear();
                    RollbackTransaction();
                }
            }
        }

        #endregion

        #region Internal

        internal void AddTransaction(TransactionScope scope)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (scope.Provider == null)
                throw new ArgumentOutOfRangeException("scope", string.Format("Sql provider is not defined in transaction scope."));
            if (scope.Provider.Id != Id)
                throw new ArgumentOutOfRangeException("scope", string.Format("Transaction scope created in parallel thread."));
            if (_disposed)
                throw new InvalidOperationException("Sql provider in current thread is disposed.");
            
            if (_watchdogException)
            {
                _watchdogException = false;
                throw new InvalidOperationException(string.Format("Timeout exception. The transaction is rollback (sql provider id \"{0}\").", _id));
            }

            lock (_lock)
            {
                _transactionIndex++;
                _transactions.Push(scope);
            }
        }

        internal void Commit(TransactionScope scope)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (scope.Provider == null)
                throw new ArgumentOutOfRangeException("scope", string.Format("Sql provider is not defined in transaction scope."));
            if (scope.Provider.Id != Id)
                throw new ArgumentOutOfRangeException("scope", string.Format("Transaction scope created in parallel thread."));
            if (_disposed)
                throw new InvalidOperationException("Sql provider in current thread is disposed.");

            if (_watchdogException)
            {
                _watchdogException = false;
                throw new InvalidOperationException(string.Format("Timeout exception. The transaction is rollback (sql provider id \"{0}\").", _id));
            }

            lock (_lock)
            {
                TransactionScope lastScope = (_transactions.Count > 0) ? _transactions.Pop() : null;
                if (lastScope == null)
                    throw new InvalidOperationException(string.Format("Invalid transaction commit sequence. The transaction with index \"{0}\" can be found (sql provider id \"{1}\").", scope.Index, _id));
                if (scope != lastScope)
                    throw new InvalidOperationException(string.Format("Invalid transaction commit sequence. The transaction with index \"{0}\" can be commited, but not \"{1}\" (sql provider id \"{2}\").", lastScope.Index, scope.Index, _id));
                if (_rollbacked)
                    throw new InvalidOperationException(string.Format("Invalid transaction commit sequence. The transaction already rollbacked (sql provider id \"{0}\").", _id));

                if (_transactions.Count == 0)
                {
                    CommitTransaction();
                }
            }
        }

        internal void Rollback(TransactionScope scope)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (scope.Provider == null)
                throw new ArgumentOutOfRangeException("scope", string.Format("Sql provider is not defined in transaction scope."));
            if (scope.Provider.Id != Id)
                throw new ArgumentOutOfRangeException("scope", string.Format("Transaction scope created in parallel thread."));
            if (_disposed)
                throw new InvalidOperationException("Sql provider in current thread is disposed.");

            if (_watchdogException)
            {
                _watchdogException = false;
                throw new InvalidOperationException(string.Format("Timeout exception. The transaction is rollback (sql provider id \"{0}\").", _id));
            }

            lock (_lock)
            {
                TransactionScope lastScope = (_transactions.Count > 0) ? _transactions.Pop() : null;
                if (lastScope == null)
                    throw new InvalidOperationException(string.Format("Invalid transaction rollback sequence. The transaction with index \"{0}\" can be found (sql provider id \"{1}\").", scope.Index, _id));
                if (scope != lastScope)
                    throw new InvalidOperationException(string.Format("Invalid transaction rollback sequence. The transaction with index \"{0}\" can be rollback, but not \"{1}\" (sql provider id \"{2}\").", lastScope.Index, scope.Index, _id));
                if (_commited)
                    throw new InvalidOperationException(string.Format("Invalid transaction rollback sequence. The transaction already commited (sql provider id \"{0}\").", _id));

                RollbackTransaction();
            }
        }

        internal long GetTransactionIndex()
        {
            return _transactionIndex;
        }

        #endregion

        #region IContextProperty

        bool IContextProperty.IsNewContextOK(Context newContext)
        {
            return (newContext != null);
        }

        void IContextProperty.Freeze(Context context)
        {
        }

        string IContextProperty.Name
        {
            get { return SqlProviderThreadPropertyName; }
        }

        #endregion

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Rollback();
            }
        }

        public bool CheckConnection(out string errorMessage)
        {
            try
            {
                errorMessage = null;
                using (CreateConnection())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToTraceString();
                return false;
            }
        }

        public void ExecuteNonQuery(DbSqlCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (_disposed)
                throw new InvalidOperationException("Sql provider in current thread is disposed.");

            if (_watchdogException)
            {
                _watchdogException = false;
                throw new InvalidOperationException(string.Format("Timeout exception. The transaction is rollback (sql provider id \"{0}\").", _id));
            }

            lock (_lock)
            {
                if (_transactions.Count == 0)
                {
                    command.CommandText = SqlSettings.SqlCommandSettings + command.CommandText;
                    _log.AddSql(command.CommandText, true);
                    using (DbSqlConnection connection = CreateConnection())
                    {
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    BeginTransaction();
                    _log.AddSql(command.CommandText);
                    command.Connection = GetConnection();
                    command.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(DbSqlCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (_disposed)
                throw new InvalidOperationException("Sql provider in current thread is disposed.");

            if (_watchdogException)
            {
                _watchdogException = false;
                throw new InvalidOperationException(string.Format("Timeout exception. The transaction is rollback (sql provider id \"{0}\").", _id));
            }

            lock (_lock)
            {
                if (_transactions.Count == 0)
                {
                    command.CommandText = SqlSettings.SqlCommandSettings + command.CommandText;
                    _log.AddSql(command.CommandText, true);
                    using (DbSqlConnection connection = CreateConnection())
                    {
                        command.Connection = connection;
                        return command.ExecuteScalar();
                    }
                }
                BeginTransaction();
                _log.AddSql(command.CommandText);
                command.Connection = GetConnection();
                return command.ExecuteScalar();
            }
        }

        public DbSqlDataReader ExecuteReader(DbSqlCommand command, CommandBehavior commandBehavior = CommandBehavior.SingleResult | CommandBehavior.CloseConnection)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (_disposed)
                throw new InvalidOperationException("Sql provider in current thread is disposed.");
            if ((_inScope) && (_watchdogException))
                throw new InvalidOperationException(string.Format("Timeout exception. The transaction is rollback (sql provider id \"{0}\").", _id));

            if (_watchdogException)
            {
                _watchdogException = false;
                throw new InvalidOperationException(string.Format("Timeout exception. The transaction is rollback (sql provider id \"{0}\").", _id));
            }

            lock (_lock)
            {
                if (_transactions.Count == 0)
                {
                    command.CommandText = SqlSettings.SqlCommandSettings + command.CommandText;
                    _log.AddSql(command.CommandText, true);
                    commandBehavior |= CommandBehavior.CloseConnection;
                    command.Connection = CreateConnection();
                    return command.ExecuteReader(commandBehavior);
                }
                BeginTransaction();
                _log.AddSql(command.CommandText);
                commandBehavior &= (~CommandBehavior.CloseConnection);
                command.Connection = GetConnection();
                return command.ExecuteReader(commandBehavior);
            }
        }

        #region Properties

        public long Id
        {
            get { return _id; }
        }

        public string Log
        {
            get { return _log.ToString(); }
        }

        public bool Commited
        {
            get { return _commited; }
        }

        public bool Rollbacked
        {
            get { return _rollbacked; }
        }

        public static SqlProvider ThreadInstance
        {
            get { return GetThreadInstance(); }
        }

        #endregion
    }
}