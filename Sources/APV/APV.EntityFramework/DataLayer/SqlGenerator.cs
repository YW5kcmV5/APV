using System.Text;
using APV.Common.Attributes.Db;

namespace APV.EntityFramework.DataLayer
{
    internal static class SqlGenerator
    {
        private static string GenerateSimpleInsertSql(DbTableAttribute attribute, bool composite)
        {
            var sb = new StringBuilder();
            var sbValues = new StringBuilder();
            sb.AppendFormat("INSERT INTO [{0}] (", attribute.TableName);
            if (attribute.InnerKey != null)
            {
                string innerKeyFieldName = attribute.InnerKey.FieldName;
                sb.AppendFormat("[{0}], ", innerKeyFieldName);
                sbValues.AppendFormat("@{0}, ", innerKeyFieldName);
            }
            int fieldsCount = attribute.Fields.Length;
            for (int i = 0; i < fieldsCount; i++)
            {
                DbFieldAttribute field = attribute.Fields[i];
                if (!field.Key)
                {
                    sb.AppendFormat("[{0}], ", field.FieldName);
                    sbValues.AppendFormat("@{0}, ", field.FieldName);
                }
            }
            sb.Length -= 2;
            sbValues.Length -= 2;
            sb.AppendFormat(")");
            if (!composite)
            {
                sb.AppendFormat(" OUTPUT INSERTED.[{0}]", attribute.PrimaryKey.FieldName);
            }
            sb.AppendFormat(" VALUES ({0});", sbValues);
            if (composite)
            {
                sb.AppendFormat("\r\nDECLARE @{0} bigint = @@IDENTITY;", attribute.PrimaryKey.FieldName);
            }
            return sb.ToString();
        }

        public static string GenerateSimpleUpdateSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE [{0}] SET ", attribute.TableName);
            int fieldsCount = attribute.Fields.Length;
            for (int i = 0; i < fieldsCount; i++)
            {
                DbFieldAttribute field = attribute.Fields[i];
                if (!field.Key)
                {
                    sb.AppendFormat("[{0}] = @{1}, ", field.FieldName, field.FieldName);
                }
            }
            sb.Length -= 2;
            sb.AppendFormat("\r\n WHERE [{0}].[{1}]=@{2};", attribute.TableName, attribute.PrimaryKey.FieldName, attribute.PrimaryKey.FieldName);
            return sb.ToString();
        }

        public static string GenerateSelectSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();
            var sbInnerJoin = new StringBuilder();
            sb.Append("SELECT ");
            sb.AppendFormat("[{0}].*", attribute.TableName);

            int innerTablesCount = attribute.InnerTables.Length;
            for (int i = 0; i < innerTablesCount; i++)
            {
                DbTableAttribute table = attribute.InnerTables[i];
                sb.AppendFormat(", [{0}].*", table.TableName);
                string primaryKeyName = table.PrimaryKey.FieldName;
                sbInnerJoin.AppendFormat("\r\n INNER JOIN [{0}] ON [{1}].[{2}] = [{3}].[{4}]", table.TableName, table.TableName, primaryKeyName, attribute.TableName, primaryKeyName);
            }
            sb.AppendFormat("\r\n FROM [{0}] {1}", attribute.TableName, sbInnerJoin);
            return sb.ToString();
        }

        public static string GenerateSelectByKeySql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();
            var sbInnerJoin = new StringBuilder();
            sb.Append("SELECT ");
            int allFieldsCount = attribute.AllFields.Length;
            for (int i = 0; i < allFieldsCount; i++)
            {
                DbFieldAttribute field = attribute.AllFields[i];
                if (!field.IsPrimaryKey)
                {
                    sb.AppendFormat("[{0}].[{1}], ", field.Table.TableName, field.FieldName);
                }
            }
            sb.Length -= 2;

            int innerTablesCount = attribute.InnerTables.Length;
            for (int i = 0; i < innerTablesCount; i++)
            {
                DbTableAttribute table = attribute.InnerTables[i];
                string primaryKeyName = table.PrimaryKey.FieldName;
                sbInnerJoin.AppendFormat("\r\n INNER JOIN [{0}] ON [{1}].[{2}] = [{3}].[{4}]", table.TableName, table.TableName, primaryKeyName, attribute.TableName, primaryKeyName);
            }
            sb.AppendFormat("\r\n FROM [{0}] {1}", attribute.TableName, sbInnerJoin);
            sb.AppendFormat("\r\n WHERE [{0}].[{1}]=@{2};", attribute.TableName, attribute.PrimaryKey.FieldName, attribute.PrimaryKey.FieldName);
            return sb.ToString();
        }

        public static string GenerateSelectByNameSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();
            var sbInnerJoin = new StringBuilder();
            sb.Append("SELECT ");
            sb.AppendFormat("[{0}].*", attribute.TableName);

            int innerTablesCount = attribute.InnerTables.Length;
            for (int i = 0; i < innerTablesCount; i++)
            {
                DbTableAttribute table = attribute.InnerTables[i];
                sb.AppendFormat(", [{0}].*", table.TableName);
                string primaryKeyName = table.PrimaryKey.FieldName;
                sbInnerJoin.AppendFormat("\r\n INNER JOIN [{0}] ON [{1}].[{2}] = [{3}].[{4}]", table.TableName, table.TableName, primaryKeyName, attribute.TableName, primaryKeyName);
            }
            sb.AppendFormat("\r\n FROM [{0}] {1}", attribute.TableName, sbInnerJoin);
            sb.AppendFormat("\r\n WHERE [{0}].[{1}]=@{2};", attribute.TableName, attribute.NameField.FieldName, attribute.NameField.FieldName);
            return sb.ToString();
        }

        public static string GenerateSelectCountSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(*) FROM [{0}]", attribute.TableName);
            return sb.ToString();
        }

        public static string GenerateInsertSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();
            bool composite = attribute.Composite;

            sb.Append("BEGIN TRAN;\r\n");

            if (!composite)
            {
                sb.Append(GenerateSimpleInsertSql(attribute, false));
            }
            else
            {
                int length = attribute.InnerTables.Length;
                for (int i = length - 1; i >= 0; i--)
                {
                    DbTableAttribute innerTable = attribute.InnerTables[i];
                    sb.AppendFormat("{0}\r\n", GenerateSimpleInsertSql(innerTable, true));
                }

                sb.AppendFormat("{0}\r\n", GenerateSimpleInsertSql(attribute, true));
                sb.AppendFormat("SELECT @{0}", attribute.PrimaryKey.FieldName);
                for (int i = 0; i < length; i++)
                {
                    DbTableAttribute innerTable = attribute.InnerTables[i];
                    sb.AppendFormat(", @{0}", innerTable.PrimaryKey.FieldName);
                }
            }

            sb.Append("\r\nCOMMIT;");

            return sb.ToString();
        }

        public static string GenerateUpdateSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();

            sb.Append("BEGIN TRAN;\r\n");

            DbTableAttribute[] innerTables = attribute.InnerTables;
            int innerTableCount = innerTables.Length;
            for (int i = 0; i < innerTableCount; i++)
            {
                DbTableAttribute innerTable = innerTables[i];
                sb.AppendFormat("{0}\r\n", GenerateSimpleUpdateSql(innerTable));
            }
            sb.Append(GenerateSimpleUpdateSql(attribute));
            
            sb.Append("\r\nCOMMIT;");

            return sb.ToString();
        }

        public static string GenerateExistsSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();

            string tableName = attribute.TableName;
            string primaryKeyName = attribute.PrimaryKey.FieldName;
            sb.AppendFormat("IF (EXISTS (SELECT [{0}].[{1}] FROM [{2}] WHERE [{3}].{4} = @{5})) SELECT 1 ELSE SELECT 0;",
                tableName, primaryKeyName, tableName, tableName, primaryKeyName, primaryKeyName);

            return sb.ToString();
        }

        public static string GenerateExistsByNameSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();

            string tableName = attribute.TableName;
            string primaryKeyName = attribute.PrimaryKey.FieldName;
            string nameField = attribute.NameField.FieldName;
            sb.AppendFormat("IF (EXISTS (SELECT [{0}].[{1}] FROM [{2}] WHERE [{3}].{4} = @{5})) SELECT 1 ELSE SELECT 0;",
                tableName, primaryKeyName, tableName, tableName, nameField, nameField);

            return sb.ToString();
        }

        public static string GenerateDeleteByIdSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();
            var sbInnerTables = new StringBuilder();
            bool composite = attribute.Composite;

            sb.Append("\r\nBEGIN TRAN;");

            string tableName = attribute.TableName;
            string primaryKeyName = attribute.PrimaryKey.FieldName;

            if (composite)
            {
                int innerTablesCount = attribute.InnerTables.Length;
                for (int i = 0; i < innerTablesCount; i++)
                {
                    DbTableAttribute innerTable = attribute.InnerTables[i];
                    string innerTableName = innerTable.TableName;
                    string innerKeyName = innerTable.PrimaryKey.FieldName;

                    sb.AppendFormat("\r\nDECLARE @{0} bigint = (SELECT [{1}].[{2}] FROM [{3}] WHERE [{4}].[{5}] = @{6});",
                                    innerKeyName, tableName, innerKeyName, tableName, tableName, primaryKeyName, primaryKeyName);

                    sbInnerTables.AppendFormat("\r\nDELETE FROM [{0}] WHERE [{1}].[{2}] = @{3};", innerTableName, innerTableName, innerKeyName, innerKeyName);
                }
            }

            sb.AppendFormat("\r\nDELETE FROM [{0}] WHERE [{1}].[{2}] = @{3};", tableName, tableName, primaryKeyName, primaryKeyName);

            if (composite)
            {
                sb.Append(sbInnerTables);
            }

            sb.Append("\r\nCOMMIT;");

            return sb.ToString();
        }

        public static string GenerateDeleteSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();

            sb.Append("\r\nBEGIN TRAN;");

            string tableName = attribute.TableName;
            string primaryKeyName = attribute.PrimaryKey.FieldName;
            sb.AppendFormat("\r\nDELETE FROM [{0}] WHERE [{1}].[{2}] = @{3};", tableName, tableName, primaryKeyName, primaryKeyName);

            int innerTablesCount = attribute.InnerTables.Length;
            for (int i = 0; i < innerTablesCount; i++)
            {
                DbTableAttribute innerTable = attribute.InnerTables[i];
                string innerTableName = innerTable.TableName;
                string innerKeyName = innerTable.PrimaryKey.FieldName;
                sb.AppendFormat("\r\nDELETE FROM [{0}] WHERE [{1}].[{2}] = @{3};", innerTableName, innerTableName, innerKeyName, innerKeyName);
            }

            sb.Append("\r\nCOMMIT;");

            return sb.ToString();
        }

        public static string GenerateMarkAsDeletedSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();

            sb.Append("\r\nBEGIN TRAN;");

            string tableName = attribute.TableName;
            string primaryKeyName = attribute.PrimaryKey.FieldName;
            sb.AppendFormat("\r\nUPDATE [{0}] SET [{1}].[{2}] = 1 WHERE [{3}].[{4}] = @{5};",
                            tableName, tableName, attribute.DeletedField.FieldName, tableName, primaryKeyName, primaryKeyName);

            sb.Append("\r\nCOMMIT;");

            return sb.ToString();
        }

        public static string GenerateRestoreSql(DbTableAttribute attribute)
        {
            var sb = new StringBuilder();

            sb.Append("\r\nBEGIN TRAN;");

            string tableName = attribute.TableName;
            string primaryKeyName = attribute.PrimaryKey.FieldName;
            sb.AppendFormat("\r\nUPDATE [{0}] SET [{1}].[{2}] = 0 WHERE [{3}].[{4}] = @{5};",
                            tableName, tableName, attribute.DeletedField.FieldName, tableName, primaryKeyName, primaryKeyName);

            sb.Append("\r\nCOMMIT;");

            return sb.ToString();
        }

        public static string WrapSql(string sql, bool transaction)
        {
            sql = "\r\n" + sql.Trim();
            if (!sql.EndsWith(";"))
            {
                sql += ";";
            }

            var sb = new StringBuilder();

            if (transaction)
            {
                sb.Append("\r\nBEGIN TRAN;");
            }

            sb.Append(sql);
            
            if (transaction)
            {
                sb.Append("\r\nCOMMIT;");
            }

            return sb.ToString();
        }
    }
}