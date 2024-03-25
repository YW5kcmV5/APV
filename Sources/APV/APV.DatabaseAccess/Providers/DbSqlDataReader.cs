using System;
using System.Data;
using APV.DatabaseAccess.Interfaces;

namespace APV.DatabaseAccess.Providers
{
    public class DbSqlDataReader : ISqlDataReader
    {
        private bool _disposed;
        private readonly ISqlDataReader _instance;

        internal DbSqlDataReader(ISqlDataReader instance)
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
                _instance.Dispose();
            }
        }

        public bool HasRows
        {
            get { return _instance.HasRows; }
        }

        public bool Read()
        {
            return _instance.Read();
        }

        public string GetName(int i)
        {
            return _instance.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            return _instance.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            return _instance.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            return _instance.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _instance.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            return _instance.GetOrdinal(name);
        }

        public bool GetBoolean(int i)
        {
            return _instance.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _instance.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _instance.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _instance.GetChar(i);
        }

        public long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length)
        {
            return _instance.GetChars(i, fieldOffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            return _instance.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _instance.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _instance.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _instance.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return _instance.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return _instance.GetDouble(i);
        }

        public string GetString(int i)
        {
            return _instance.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            return _instance.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _instance.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            return _instance.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            return _instance.IsDBNull(i);
        }

        public int FieldCount
        {
            get { return _instance.FieldCount; }
        }

        public object this[int i]
        {
            get { return _instance[i]; }
        }

        public object this[string name]
        {
            get { return _instance[name]; }
        }
    }
}