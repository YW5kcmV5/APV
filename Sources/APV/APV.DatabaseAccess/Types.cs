using System;

namespace APV.DatabaseAccess
{
    [Serializable]
    public enum DatabaseType
    {
        /// <summary>
        /// MS SQL
        /// </summary>
        MsSql,

        /// <summary>
        /// MY SQL
        /// </summary>
        MySql,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,
    }
}