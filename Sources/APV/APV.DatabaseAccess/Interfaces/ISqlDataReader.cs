using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APV.DatabaseAccess.Interfaces
{
    public interface ISqlDataReader : IDataRecord, IDisposable
    {
        bool HasRows { get; }

        bool Read();
    }
}