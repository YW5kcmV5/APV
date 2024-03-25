using System;
using System.Data;
using System.Data.SqlClient;
using APV.DatabaseAccess.Interfaces;

namespace APV.DatabaseAccess.Providers.MsSql
{
    public class MsSqlDataReader : ISqlDataReader
    {
        private bool _disposed;
        private SqlDataReader _instance;

        public MsSqlDataReader(SqlDataReader instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _instance = instance;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_instance != null)
                {
                    _instance.Dispose();
                    _instance = null;
                }
            }
        }

        public bool HasRows
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

                return _instance.HasRows;
            }
        }

        public bool Read()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.Read();
        }

        public string GetName(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetOrdinal(name);
        }

        public bool GetBoolean(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetChar(i);
        }

        public long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetChars(i, fieldOffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetDouble(i);
        }

        public string GetString(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

            return _instance.IsDBNull(i);
        }

        public int FieldCount
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

                return _instance.FieldCount;
            }
        }

        public object this[int i]
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

                return _instance[i];
            }
        }

        public object this[string name]
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name, "The data reader already disposed.");

                return _instance[name];
            }
        }
    }
}