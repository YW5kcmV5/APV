using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using APV.DatabaseAccess.Providers;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Common.Attributes.Db;

namespace APV.EntityFramework.DataLayer
{
    public abstract class BaseDataLayerManager<TEntity, TEntityCollection> : IDataLayerManager<TEntity>
        where TEntity : BaseEntity
        where TEntityCollection : BaseEntityCollection<TEntity>
    {
        private static readonly IContextManager ContextManager = EntityFrameworkManager.GetContextManager();

        private readonly DbTableAttribute _attribute = DbTableAttribute.GetAttribute(typeof(TEntity));

        private string _selectSql;
        private string _selectByKeySql;
        private string _selectByNameSql;
        private string _selectCountSql;
        private string _insertSql;
        private string _updateSql;
        private string _deleteByIdSql;
        private string _deleteSql;
        private string _markAsDeletedSql;
        private string _restoreSql;
        private string _existsSql;
        private string _existsByNameSql;

        private static void ExecuteNonQuery(DbSqlCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            SqlProvider.ThreadInstance.ExecuteNonQuery(command);
        }

        private static object ExecuteScalar(DbSqlCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            return SqlProvider.ThreadInstance.ExecuteScalar(command);
        }

        private static DbSqlDataReader ExecuteReader(DbSqlCommand command, CommandBehavior commandBehavior = CommandBehavior.SingleResult | CommandBehavior.CloseConnection)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            return SqlProvider.ThreadInstance.ExecuteReader(command, commandBehavior);
        }

        private static object Copy(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is byte[])
            {
                return ((byte[])value).Copy();
            }
            if (value is string[])
            {
                return ((string[])value).Copy();
            }
            if (value is XmlDocument)
            {
                return ((XmlDocument)value).Copy();
            }
            return value;
        }

        private void InternalFill(TEntity entity, IDataRecord row)
        {
            DbFieldAttribute[] fields = _attribute.AllFieldsWithoutPrimaryKey;
            int fieldsCount = fields.Length;
            for (int i = 0; i < fieldsCount; i++)
            {
                DbFieldAttribute field = fields[i];
                object value = row.GetValue(i);
                field.SetDbValue(entity, value);
            }
        }

        private void AddParameters(TEntity entity, DbSqlCommand command, bool addPrimaryKey)
        {
            DateTime @now = GetUtcNow();
            long userId = ContextManager.GetContext().UserId;
            long instanceId = ContextManager.GetInstanceId();
            DbFieldAttribute[] allFields = _attribute.AllFields;
            int fieldsCount = allFields.Length;
            bool isNew = entity.IsNew;
            for (int i = 0; i < fieldsCount; i++)
            {
                DbFieldAttribute field = allFields[i];
                if ((addPrimaryKey) || (!field.Key))
                {
                    object value = null;
                    if (((isNew) && (field.Generation == DbFieldAttribute.GenerationMode.OnCreate)) ||
                        (field.Generation == DbFieldAttribute.GenerationMode.OnUpdate))
                    {
                        if (field.IsDateTime)
                        {
                            value = @now;
                            field.SetValue(entity, value);
                        }
                        else if (field.IsGuid)
                        {
                            value = Guid.NewGuid();
                            field.SetValue(entity, value);
                        }
                        else if (field.SpecialField == DbSpecialField.UserId)
                        {
                            value = userId;
                            field.SetValue(entity, value);
                        }
                        else if (field.SpecialField == DbSpecialField.InstanceId)
                        {
                            value = instanceId;
                            field.SetValue(entity, value);
                        }
                    }
                    value = value ?? field.GetDbValue(entity);
                    
                    if ((value == null) && (!field.Nullable))
                        throw new ArgumentOutOfRangeException("entity", string.Format("Property \"{0}\" in entity \"{1}\" is not nullable but is null.", typeof(TEntity).Name, field.PropertyName));

                    if ((field.IsString) && (field.MaxLength > 0))
                    {
                        var stringValue = (value as string) ?? string.Empty;
                        if (stringValue.Length > field.MaxLength)
                            throw new ArgumentOutOfRangeException("entity", string.Format("Property \"{0}\" length is too long  in entity \"{1}\", max length is {2}.", typeof(TEntity).Name, field.PropertyName, field.MaxLength));
                    }
                    
                    command.Parameters.AddWithValue(string.Format("@{0}", field.FieldName), value);
                }
            }
        }

        private bool FindAndFill(TEntity entity, long id)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (!_attribute.Support(DbOperation.GetById))
                throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"GetById\" operation.", typeof(TEntity).Name));

            if (id == SystemConstants.UnknownId)
            {
                return false;
            }

            var cachedEntity = DataLayerCacheManager.Get<TEntity>(id);
            if (cachedEntity != null)
            {
                Copy(entity, cachedEntity);
                return true;
            }

            if (_selectByKeySql == null)
            {
                _selectByKeySql = SqlGenerator.GenerateSelectByKeySql(_attribute);
            }

            var command = new DbSqlCommand(_selectByKeySql);
            command.Parameters.AddWithValue(string.Format("@{0}", _attribute.PrimaryKey.FieldName), id);
            using (DbSqlDataReader row = ExecuteReader(command))
            {
                if ((!row.HasRows) || (!row.Read()))
                {
                    return false;
                }

                InternalFill(entity, row);
                _attribute.PrimaryKey.SetValue(entity, id);
            }

            DataLayerCacheManager.Update(entity);

            return true;
        }

        public DateTime GetUtcNow()
        {
            DateTime now = DateTime.UtcNow;
            int mlsec = 10 * (int)(now.Millisecond / 10.0);
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, mlsec, DateTimeKind.Utc);
            return now;
        }

        public Type GetEntityType()
        {
            return typeof(TEntity);
        }

        public bool Equals(TEntity x, TEntity y)
        {
            if (x == y)
            {
                return true;
            }
            if ((x == null) || (y == null) || (x.GetType() != y.GetType()))
            {
                return false;
            }

            DbFieldAttribute[] allFields = _attribute.AllFields;
            int allFieldsCount = allFields.Length;
            for (int i = 0; i < allFieldsCount; i++)
            {
                DbFieldAttribute field = allFields[i];
                object xValue = field.GetValue(x);
                object yValue = field.GetValue(y);
                if (!Comparator.Equals(xValue, yValue))
                {
                    return false;
                }
            }
            return true;
        }

        public void Fill(TEntity entity, IDataRecord row)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (row == null)
                throw new ArgumentNullException("row");

            DbFieldAttribute[] allFields = _attribute.AllFields;
            int allFieldsCount = allFields.Length;
            for (int i = 0; i < allFieldsCount; i++)
            {
                DbFieldAttribute field = allFields[i];
                int index = (field.Index ?? (field.Index = row.GetOrdinal(field.FieldName))).Value;
                object value = row.GetValue(index);
                field.SetDbValue(entity, value);
            }
        }

        public TEntity Fill(IDataRecord row, IEntityCollection container = null)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            var entity = (container != null)
                             ? (TEntity) Activator.CreateInstance(typeof (TEntity), container)
                             : (TEntity) Activator.CreateInstance(typeof (TEntity));

            Fill(entity, row);
            return entity;
        }

        public void Fill(TEntity entity, long id)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (!FindAndFill(entity, id))
                throw new ArgumentOutOfRangeException("id", string.Format("{0} with id \"{1}\" does not exist.", typeof(TEntity).Name, id));
        }

        public TEntity Find(long id)
        {
            var entity = (TEntity)Activator.CreateInstance(typeof(TEntity));
            return (FindAndFill(entity, id)) ? entity : null;
        }

        public TEntity Find(string whereSql, Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(whereSql))
                throw new ArgumentNullException("whereSql");

            parameters = parameters ?? new Dictionary<string, object>();

            if (_selectSql == null)
            {
                _selectSql = SqlGenerator.GenerateSelectSql(_attribute);
            }

            string sql = SqlGenerator.WrapSql(string.Format("{0}\r\n {1};", _selectSql, whereSql.Trim()), false);

            TEntity entity;
            var command = new DbSqlCommand(sql);
            foreach (KeyValuePair<string, object> param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            using (DbSqlDataReader row = ExecuteReader(command))
            {
                if ((!row.HasRows) || (!row.Read()))
                {
                    return null;
                }

                entity = Fill(row);
            }

            DataLayerCacheManager.Update(entity);
            return entity;
        }

        public TEntityCollection Find(long[] identifiers, IEntity container = null)
        {
            if (identifiers == null)
                throw new ArgumentNullException("identifiers");

            if (identifiers.Length == 0)
            {
                return (container != null)
                           ? (TEntityCollection) Activator.CreateInstance(typeof (TEntityCollection), container)
                           : (TEntityCollection) Activator.CreateInstance(typeof (TEntityCollection));
            }

            var @params = new Dictionary<string, object>();
            var paramsList = new StringBuilder();
            for (int i = 0; i < identifiers.Length; i++)
            {
                long id = identifiers[i];
                string paramName = string.Format("@Param{0:000}", i);
                paramsList.Append(paramName);
                paramsList.Append(", ");
                @params.Add(paramName, id);
            }
            paramsList.Length -= 2;

            string keyFieldName = _attribute.PrimaryKey.FieldName;
            string whereSql = string.Format("WHERE [{0}].{1} IN ({2})", _attribute.TableName, keyFieldName, paramsList);
            TEntityCollection collection = GetList(whereSql, @params, container);
            collection.SetReadOnly();
            return collection;
        }

        public virtual TEntity FindByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if ((!_attribute.Support(DbOperation.GetByName)) || (_attribute.NameField == null))
                throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"GetByName\" operation.", typeof(TEntity).Name));

            if (_selectByNameSql == null)
            {
                _selectByNameSql = SqlGenerator.GenerateSelectByNameSql(_attribute);
            }

            TEntity entity;
            var command = new DbSqlCommand(_selectByNameSql);
            command.Parameters.AddWithValue(string.Format("@{0}", _attribute.NameField.FieldName), name);

            using (DbSqlDataReader row = ExecuteReader(command))
            {
                if ((!row.HasRows) || (!row.Read()))
                {
                    return null;
                }

                entity = Fill(row);
            }

            DataLayerCacheManager.Update(entity);
            return entity;
        }

        public TEntity Get(long id)
        {
            TEntity entity = Find(id);

            if (entity == null)
                throw new ArgumentOutOfRangeException("id", string.Format("{0} with id \"{1}\" does not exist.", typeof(TEntity).Name, id));

            return entity;
        }

        public TEntity GetByName(string name)
        {
            TEntity entity = FindByName(name);

            if (entity == null)
                throw new ArgumentOutOfRangeException("name", string.Format("{0} with name \"{1}\" does not exist.", typeof(TEntity).Name, name));

            return entity;
        }

        public TEntity Get(string sql, Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            TEntity entity = Find(sql, parameters);

            if (entity == null)
                throw new InvalidOperationException(string.Format("Specified {0} does not exist.", typeof(TEntity).Name));

            return entity;
        }

        public TEntityCollection GetList(string whereSql, int top)
        {
            return GetList(whereSql, null, null, top);
        }

        public TEntityCollection GetList(string whereSql, Dictionary<string, object> parameters, int top)
        {
            return GetList(whereSql, parameters, null, top);
        }

        public TEntityCollection GetList(string whereSql, Dictionary<string, object> parameters = null, IEntity container = null, int? top = null)
        {
            parameters = parameters ?? new Dictionary<string, object>();

            if (_selectSql == null)
            {
                _selectSql = SqlGenerator.GenerateSelectSql(_attribute);
            }
            whereSql = (whereSql ?? string.Empty).Trim();

            string sql = (!string.IsNullOrEmpty(whereSql))
                             ? string.Format("{0}\r\n {1};", _selectSql, whereSql)
                             : string.Format("{0};", _selectSql);

            if (top != null)
            {
                sql = (sql.Replace("SELECT ", string.Format("SELECT TOP {0} ", top)));
            }

            sql = SqlGenerator.WrapSql(sql, false);

            var collection = (container != null)
                                 ? (TEntityCollection) Activator.CreateInstance(typeof (TEntityCollection), container)
                                 : (TEntityCollection) Activator.CreateInstance(typeof (TEntityCollection));

            var command = new DbSqlCommand(sql);
            foreach (KeyValuePair<string, object> param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            using (DbSqlDataReader row = ExecuteReader(command))
            {
                if (row.HasRows)
                {
                    while (row.Read())
                    {
                        TEntity entity = Fill(row, collection);
                        collection.Add(entity);
                    }
                }
            }

            return collection;
        }

        public IEntityCollection GetCollection(IEntity entity, string keyFieldName, long keyId)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (string.IsNullOrEmpty(keyFieldName))
                throw new ArgumentNullException("keyFieldName");

            string whereSql = string.Format("WHERE [{0}].{1} = @Key", _attribute.TableName, keyFieldName);
            var @params = new Dictionary<string, object> { { "@Key", keyId } };
            IEntityCollection collection = GetList(whereSql, @params, entity);
            collection.SetReadOnly();
            return collection;
        }

        public IEntityCollection GetCollection(IEntity entity, Type keyEntityType)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (keyEntityType == null)
                throw new ArgumentNullException("keyEntityType");

            long keyId = entity.Id;
            string keyFieldName = _attribute.GetKeyFieldName(keyEntityType);
            IDataLayerManager manager = EntityFrameworkManager.GetManager(keyEntityType);
            return manager.GetCollection(entity, keyFieldName, keyId);
        }

        public IEntityCollection GetAll()
        {
            if (!_attribute.Support(DbOperation.GetAll))
                throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"GetAll\" operation.", typeof(TEntity).Name));

            IEntityCollection collection = GetList(string.Empty);
            collection.SetReadOnly();
            return collection;
        }

        public long GetCount()
        {
            if (_selectCountSql == null)
            {
                _selectCountSql = SqlGenerator.GenerateSelectCountSql(_attribute);
            }

            var command = new DbSqlCommand(_selectCountSql);
            object result = ExecuteScalar(command);
            return (result is long)
                       ? (long) result
                       : (result is decimal)
                             ? (long) (decimal) result
                             : (int) result;
        }

        public bool Exists(long id)
        {
            if (id == SystemConstants.UnknownId)
            {
                return false;
            }

            var cachedEntity = DataLayerCacheManager.Get<TEntity>(id);
            if (cachedEntity != null)
            {
                return true;
            }

            if (_existsSql == null)
            {
                _existsSql = SqlGenerator.GenerateExistsSql(_attribute);
            }

            var command = new DbSqlCommand(_existsSql);
            command.Parameters.AddWithValue(string.Format("@{0}", _attribute.PrimaryKey.FieldName), id);
            return ((int)ExecuteScalar(command) == 1);
        }

        public bool Exists(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return Exists(entity.Id);
        }

        public bool Exists(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if ((!_attribute.Support(DbOperation.GetByName)) || (_attribute.NameField == null))
                throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"GetByName\" operation.", typeof(TEntity).Name));

            if (_existsByNameSql == null)
            {
                _existsByNameSql = SqlGenerator.GenerateExistsByNameSql(_attribute);
            }

            var command = new DbSqlCommand(_existsByNameSql);
            command.Parameters.AddWithValue(string.Format("@{0}", _attribute.NameField.FieldName), name);
            return ((int)ExecuteScalar(command) == 1);
        }

        public void Delete(long id)
        {
            if (!_attribute.Support(DbOperation.Delete))
                throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"Delete\" operation.", typeof(TEntity).Name));

            if (id == SystemConstants.UnknownId)
            {
                return;
            }

            DataLayerCacheManager.Delete<TEntity>(id);

            if (_deleteByIdSql == null)
            {
                _deleteByIdSql = SqlGenerator.GenerateDeleteByIdSql(_attribute);
            }

            var command = new DbSqlCommand(_deleteByIdSql);
            command.Parameters.AddWithValue(string.Format("@{0}", _attribute.PrimaryKey.FieldName), id);
            ExecuteNonQuery(command);
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (!_attribute.Support(DbOperation.Delete))
                throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"Delete\" operation.", typeof(TEntity).Name));

            var id = (long)_attribute.PrimaryKey.GetValue(entity);

            DataLayerCacheManager.Delete<TEntity>(id);

            if (_deleteSql == null)
            {
                _deleteSql = SqlGenerator.GenerateDeleteSql(_attribute);
            }

            var command = new DbSqlCommand(_deleteSql);
            command.Parameters.AddWithValue(string.Format("@{0}", _attribute.PrimaryKey.FieldName), id);

            DbTableAttribute[] innerTables = _attribute.InnerTables;
            int innerTablesCount = innerTables.Length;
            for (int i = 0; i < innerTablesCount; i++)
            {
                DbTableAttribute innerTable = innerTables[i];
                var innerTableId = (long)innerTable.PrimaryKey.GetValue(entity);
                command.Parameters.AddWithValue(string.Format("@{0}", innerTable.PrimaryKey.FieldName), innerTableId);
            }

            ExecuteNonQuery(command);
        }

        public void Delete(IEntityCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            var typedCollection = collection as TEntityCollection;

            if (typedCollection == null)
                throw new ArgumentOutOfRangeException(string.Format("Data layer manager \"{0}\" can not be used for entity collection \"{1}\".", GetType().Name, collection.GetType().Name));

            Delete(typedCollection);
        }

        public void Delete(TEntityCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            //TODO: implement in one request
            foreach (TEntity entity in collection)
            {
                Delete(entity);
            }
        }

        public void MarkAsDeleted(long id)
        {
            if (!_attribute.Support(DbOperation.MarkAsDeleted))
                throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"MarkAsDeleted\" operation.", typeof(TEntity).Name));
            if (id == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("id", "Specified entity is new (is not stored in database).");
            if (_attribute.DeletedField == null)
                throw new InvalidOperationException(string.Format("Boolean field marked as \"DbSpecialField.Deleted\" can not be found in database entity \"{0}\"  definition.", typeof(TEntity).Name));

            DataLayerCacheManager.Delete<TEntity>(id);

            if (_markAsDeletedSql == null)
            {
                _markAsDeletedSql = SqlGenerator.GenerateMarkAsDeletedSql(_attribute);
            }

            var command = new DbSqlCommand(_markAsDeletedSql);
            command.Parameters.AddWithValue(string.Format("@{0}", _attribute.PrimaryKey.FieldName), id);
            ExecuteNonQuery(command);
        }

        public void MarkAsDeleted(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var id = (long)_attribute.PrimaryKey.GetValue(entity);
            MarkAsDeleted(id);

            _attribute.DeletedField.SetValue(entity, true);
        }

        public void Restore(long id)
        {
            if (!_attribute.Support(DbOperation.MarkAsDeleted))
                throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"MarkAsDeleted\" operation.", typeof(TEntity).Name));
            if (id == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("id", "Specified entity is new (is not stored in database).");
            if (_attribute.DeletedField == null)
                throw new InvalidOperationException(string.Format("Boolean field marked as \"DbSpecialField.Deleted\" can not be found in database entity \"{0}\"  definition.", typeof(TEntity).Name));

            DataLayerCacheManager.Delete<TEntity>(id);

            if (_restoreSql == null)
            {
                _restoreSql = SqlGenerator.GenerateRestoreSql(_attribute);
            }

            var command = new DbSqlCommand(_restoreSql);
            command.Parameters.AddWithValue(string.Format("@{0}", _attribute.PrimaryKey.FieldName), id);
            ExecuteNonQuery(command);
        }

        public void Restore(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var id = (long)_attribute.PrimaryKey.GetValue(entity);
            Restore(id);

            _attribute.DeletedField.SetValue(entity, false);
        }

        public long CreateOrUpdate(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var id = (long)_attribute.PrimaryKey.GetValue(entity);
            bool isNew = (id == SystemConstants.UnknownId);

            if (isNew)
            {
                if (!_attribute.Support(DbOperation.Create))
                    throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"Create\" operation.", typeof(TEntity).Name));

                if (_insertSql == null)
                {
                    _insertSql = SqlGenerator.GenerateInsertSql(_attribute);
                }

                var createCommand = new DbSqlCommand(_insertSql);
                AddParameters(entity, createCommand, false);

                if (_attribute.Composite)
                {
                    using (DbSqlDataReader reader = ExecuteReader(createCommand))
                    {
                        reader.Read();
                        id = (long)reader[0];
                        int innerTablesCount = _attribute.InnerTables.Length;
                        for (int i = 0; i < innerTablesCount; i++)
                        {
                            DbTableAttribute innerTable = _attribute.InnerTables[i];
                            var innerTabelId = (long)reader[i + 1];
                            innerTable.PrimaryKey.SetValue(entity, innerTabelId);
                        }
                    }
                }
                else
                {
                    id = (long)ExecuteScalar(createCommand);
                }

                _attribute.PrimaryKey.SetValue(entity, id);
                DataLayerCacheManager.Update(entity);
                return id;
            }

            if (!_attribute.Support(DbOperation.Update))
                throw new NotSupportedException(string.Format("Database entity \"{0}\" does not support \"Update\" operation.", typeof(TEntity).Name));

            if (_updateSql == null)
            {
                _updateSql = SqlGenerator.GenerateUpdateSql(_attribute);
            }

            var updateCommand = new DbSqlCommand(_updateSql);
            AddParameters(entity, updateCommand, true);
            ExecuteNonQuery(updateCommand);

            DataLayerCacheManager.Update(entity);
            return id;
        }

        public void CreateOrUpdate(IEntityCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            var typedCollection = collection as TEntityCollection;

            if (typedCollection == null)
                throw new ArgumentOutOfRangeException(string.Format("Data layer manager \"{0}\" can not be used for entity collection \"{1}\".", GetType().Name, collection.GetType().Name));

            CreateOrUpdate(typedCollection);
        }

        public void CreateOrUpdate(TEntityCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            //TODO: implement in one request

            foreach (TEntity entity in collection)
            {
                CreateOrUpdate(entity);
            }
        }

        public void Reload(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var id = (long)_attribute.PrimaryKey.GetValue(entity);
            bool isNew = (id == SystemConstants.UnknownId);
            if (isNew)
                throw new ArgumentOutOfRangeException("entity", string.Format("Specified {0} entity is new (is not stored in database).", typeof(TEntity).Name));
            if (!FindAndFill(entity, id))
                throw new ArgumentOutOfRangeException("entity", string.Format("{0} with id \"{1}\" does not exist.", typeof(TEntity).Name, id));
        }

        public byte[] Serialize(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            DbFieldAttribute[] allFields = _attribute.AllFields;
            int allFieldsCount = allFields.Length;
            var dump = new object[allFieldsCount];
            for (int i = 0; i < allFieldsCount; i++)
            {
                dump[i] = allFields[i].GetValue(entity);
            }

            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, dump);
                return stream.ToArray();
            }
        }

        public void Deserialize(TEntity entity, byte[] dump)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (dump == null)
                throw new ArgumentNullException("dump");

            object[] data;
            using (var stream = new MemoryStream(dump))
            {
                data = (object[])new BinaryFormatter().Deserialize(stream);
            }

            DbFieldAttribute[] allFields = _attribute.AllFields;
            int allFieldsCount = allFields.Length;
            for (int i = 0; i < allFieldsCount; i++)
            {
                allFields[i].SetValue(entity, data[i]);
            }
        }

        public void Copy(TEntity entity, TEntity from)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (from == null)
                throw new ArgumentNullException("from");
            if (entity.GetType() != from.GetType())
                throw new ArgumentOutOfRangeException("from", string.Format("Copied entities must have the same type, but destionation entity has type \"{0}\" and source entity has type \"{1}\".", entity.GetType().FullName, from.GetType().FullName));
            
            if (ReferenceEquals(entity, from))
            {
                return;
            }

            //byte[] dump = Serialize(from);
            //Deserialize(entity, dump);

            DbFieldAttribute[] allFields = _attribute.AllFields;
            int allFieldsCount = allFields.Length;
            for (int i = 0; i < allFieldsCount; i++)
            {
                object value = allFields[i].GetValue(from);
                object copy = Copy(value);
                allFields[i].SetValue(entity, copy);
            }
        }

        public TEntity Deserialize(byte[] dump)
        {
            var entity = (TEntity)Activator.CreateInstance(typeof(TEntity));
            Deserialize(entity, dump);
            return entity;
        }

        public TEntity Clone(TEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            var copy = (TEntity)Activator.CreateInstance(typeof(TEntity));
            DbFieldAttribute[] allFields = _attribute.AllFields;
            int allFieldsCount = allFields.Length;
            for (int i = 0; i < allFieldsCount; i++)
            {
                DbFieldAttribute field = allFields[i];
                object value = field.GetValue(entity);
                field.SetValue(copy, value);
            }
            return entity;
        }

        public long[] GetKeys(string sql, Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            parameters = parameters ?? new Dictionary<string, object>();

            sql = SqlGenerator.WrapSql(sql, true);

            var command = new DbSqlCommand(sql);
            foreach (KeyValuePair<string, object> param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            var keys = new List<long>();
            using (DbSqlDataReader row = ExecuteReader(command))
            {
                if (row.HasRows)
                {
                    while (row.Read())
                    {
                        var key = (long)row[0];
                        keys.Add(key);
                    }
                }
            }

            return keys.ToArray();
        }

        public void Execute(string sql, Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            parameters = parameters ?? new Dictionary<string, object>();

            sql = SqlGenerator.WrapSql(sql, true);

            var command = new DbSqlCommand(sql);
            foreach (KeyValuePair<string, object> param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            ExecuteNonQuery(command);
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            parameters = parameters ?? new Dictionary<string, object>();

            sql = SqlGenerator.WrapSql(sql, true);

            var command = new DbSqlCommand(sql);
            foreach (KeyValuePair<string, object> param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            return ExecuteScalar(command);
        }

        #region IManager

        IEntityCollection IDataLayerManager.GetCollection(IEntity entity, string keyFieldName, long keyId)
        {
            return GetCollection(entity, keyFieldName, keyId);
        }

        void IDataLayerManager.Fill(IEntity entity, long id)
        {
            Fill((TEntity)entity, id);
        }

        bool IDataLayerManager.Equals(IEntity x, IEntity y)
        {
            return Equals(x as TEntity, y as TEntity);
        }

        byte[] IDataLayerManager.Serialize(IEntity entity)
        {
            return Serialize((TEntity)entity);
        }

        void IDataLayerManager.Copy(IEntity entity, IEntity from)
        {
            Copy((TEntity)entity, (TEntity)from);
        }

        void IDataLayerManager.Deserialize(IEntity entity, byte[] dump)
        {
            Deserialize((TEntity) entity, dump);
        }

        IEntity IDataLayerManager.Clone(IEntity entity)
        {
            return Clone((TEntity)entity);
        }

        #endregion
    }
}