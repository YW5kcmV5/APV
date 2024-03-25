using System;
using System.Text;

namespace APV.EntityFramework.DataLayer
{
    public sealed class SqlProviderLog : IDisposable
    {
        #region Constants

        /// <summary>
        /// [OPEN CONNECTION]
        /// </summary>
        public const string FirstIdent = @"[OPEN CONNECTION]";

        /// <summary>
        /// [CLOSE CONNECTION]
        /// </summary>
        public const string LastIdent = @"[CLOSE CONNECTION]";

        #endregion

        private readonly object _lock = new object();
        private readonly StringBuilder _log = new StringBuilder();
        private string _ident = string.Empty;
        private int _identValue;
        private DateTime _connectionOpenAt;
        private bool _disposed;

        private void WriteFirstIdent()
        {
            _connectionOpenAt = DateTime.UtcNow;
            if (_log.Length > 0)
            {
                _log.AppendLine();
            }
            _log.Append(FirstIdent);
            _log.AppendLine();
        }

        private void WriteLastIdent()
        {
            var processTimeInMlsec = (int)Math.Round((DateTime.UtcNow - _connectionOpenAt).TotalMilliseconds);
            _log.AppendFormat("{0}[{1} mlsec]", LastIdent, processTimeInMlsec);
            _log.AppendLine();
        }

        private void Write(string log)
        {
            log = log.Trim();
            log = log.Replace(";\n", ";").Replace(";\r\n", ";");
            while (log.Contains(";;"))
            {
                log = log.Replace(";;", ";");
                log = log.Replace(";;", ";");
            }
            log = log.Replace(";", ";\r\n");
            string[] lines = log.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                _log.AppendFormat("{0}{1}\r\n", _ident, line);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _log.Clear();
                _ident = null;
                _identValue = 0;
            }
        }

        public void AddSql(string sql, bool atomOperation = false)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                lock (_lock)
                {
                    if (atomOperation)
                    {
                        IncreaseIdent();
                        Write(sql);
                        DecreaseIdent();
                    }
                    else
                    {
                        Write(sql);
                    }
                }
            }
        }

        public void IncreaseIdent()
        {
            lock (_lock)
            {
                if (_identValue == 0)
                {
                    WriteFirstIdent();
                }
                _identValue++;
                _ident += "\t";
            }
        }

        public void DecreaseIdent()
        {
            lock (_lock)
            {
                if (_identValue > 0)
                {
                    _identValue--;
                    _ident = _ident.Substring(0, _ident.Length - 1);
                    if (_identValue == 0)
                    {
                        WriteLastIdent();
                    }
                }
            }
        }

        public override string ToString()
        {
            return _log.ToString();
        }
    }
}