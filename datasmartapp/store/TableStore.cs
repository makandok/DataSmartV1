using System;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using JhpDataSystem.model;
using System.Threading.Tasks;
using JhpDataSystem;

namespace DataSmart.store
{
    public class MultiTableStore
    {
        protected LocalDB _db;
        public List<KindName> Kinds { get; set; }

        public MultiTableStore()
        {
            _db = new LocalDB();
        }

        public List<KindItem> getRecordBlobs()
        {
            if (Kinds == null) return null;

            var toReturn = new List<KindItem>();
            foreach (var table in Kinds)
            {
                var res = new TableStore(table).GetAllBlobs();
                toReturn.AddRange(res);
            }
            return toReturn;
        }

        internal List<NameValuePair> getAllBobsCount()
        {
            if (Kinds == null) return new List<NameValuePair>();

            var toReturn = new List<NameValuePair>();
            foreach (var table in Kinds)
            {
                var res = new TableStore(table).Count();
                toReturn.Add(new NameValuePair() { Name = table.Value, Value = res.Result.ToString() });
            }
            return toReturn;
        }
    }

    public class TableStore
    {
        public ProcessLogger MainLogger
        {
            get;set;
        }
        protected string TableType = "blob";
        protected KindName _tableName;
        protected LocalDB _db;        
        protected const string createKindSql = "create table if not exists {0}(id nvarchar(32) primary key, datablob nvarchar(500));";
        protected const string insertSql = "insert or replace into {0}(id, datablob) values (@id, @datablob)";
        protected const string deleteSql = "delete from {0} where id = @id";
        protected const string deleteAllSql = "delete from {0}";

        protected const string selectIdsForAll = "select id from {0}";
        protected const string selectCountForAll = "select count(id) from {0}";
        protected const string selectDatablobs = "select datablob from {0}";
        protected const string selectDatablobsById = "select datablob from {0} where id = @id";
        /// <summary>
        /// Creates new instance of a Kind store, which allows Get, Put, Delete and Update
        /// </summary>
        /// <param name="tableName">Kind name</param>
        public TableStore(KindName tableName)
        {
            _tableName = tableName;
            _db = new LocalDB();
        }
        public TableStore(string tableName)
        {
            _tableName = new KindName(tableName);
            _db = new LocalDB();
        }
        /// <summary>
        /// Creates Kind if it does not exist
        /// </summary>
        /// <returns>True if successful</returns>
        public bool build()
        {
            //we create if does not exist
            var saveStatus = false;
            using (var conn = new SqliteConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return false;
                }

                try
                {
                    //check if our table tables, create if it doesn't
                    var command = conn.CreateCommand();
                    command.CommandText = string.Format(createKindSql, _tableName.Value);
                    command.ExecuteNonQuery();
                    saveStatus = true;
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error creating table{0}{1}", Environment.NewLine, ex.ToString()));
                }
                finally
                {
                    conn.Close();
                }
            }
            return saveStatus;
        }

        internal bool DeleteAll()
        {
            var saveStatus = false;
            using (var conn = new SqliteConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return false;
                }

                try
                {
                    //check if our table tables, create if it doesn't

                    var command = conn.CreateCommand();
                    command.CommandText = string.Format(deleteAllSql, _tableName.Value);
                    command.ExecuteNonQuery();
                    saveStatus = true;
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error deleting records from database table {2}{0}{1}", Environment.NewLine, ex.ToString(), _tableName.Value));
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
            return saveStatus;
        }

        /// <summary>
        /// Replaces record in database if it exists or inserts a new one if it doesn't exist. Maintains key supplied
        /// </summary>
        /// <param name="key">Unique id of entity</param>
        /// <param name="dataToSave">Data to save</param>
        /// <returns>Entity Id if successful or default error code</returns>
        public KindKey Update(KindKey key, KindItem dataToSave)
        {
            var saved = Save(key, dataToSave);
            if (!saved)
            {
                return new KindKey(Constants.DBSAVE_ERROR);
            }
            return key;
        }
        /// <summary>
        /// Assigns an Id and Inserts record into database
        /// </summary>
        /// <param name="dataToSave">Data to save</param>
        /// <returns>Unique record identifier</returns>
        public KindKey Put(KindItem dataToSave)
        {
            var id = new KindKey(_db.newId());
            var saved = Save(id, dataToSave);
            if(!saved)
            {
                id = new KindKey(Constants.DBSAVE_ERROR) ;
            }
            return id;
        }

        public List<KindItem> GetAllBlobs()
        {
            return GetBlob(null);
        }

        public List<KindItem> Get(KindKey entityId)
        {
            if (entityId == null)
                throw new ArgumentNullException("entityId is null");

            return GetBlob(entityId);
        }

        /// <summary>
        /// Returns matching entities by key supplied
        /// </summary>
        /// <param name="entityId">Id to search by</param>
        /// <returns>List of matching entities</returns>        
        private List<KindItem> GetBlob(KindKey entityId=null)
        {
           var toReturn = new List<KindItem>();
            using (var conn = new SqliteConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return new List<KindItem>() { new KindItem(Constants.DBSAVE_ERROR) };
                }

                try
                {
                    //check if our table tables, create if it doesn't
                    var command = conn.CreateCommand();
                    if (entityId == null)
                    {
                        command.CommandText = string.Format(selectDatablobs, _tableName.Value);
                    }
                    else
                    {
                        command.CommandText = string.Format(selectDatablobsById, _tableName.Value);
                        command.Parameters.AddWithValue("@id", entityId.Value);
                    }
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        toReturn.Add(new KindItem(reader.GetString(0)));
                    }
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error reading data from database{0}{1}", Environment.NewLine, ex.ToString()));
                    return new List<KindItem>() { new KindItem(Constants.DBSAVE_ERROR) };
                }
                finally
                {
                    conn.Close();
                }
            }
            return toReturn;
        }
        /// <summary>
        /// Returns Ids of all entities
        /// </summary>
        /// <returns>Returns Ids of all entities</returns>
        public List<KindKey> GetKeys()
        {
            var toReturn = new List<KindKey>();
            using (var conn = new SqliteConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return new List<KindKey>() { new KindKey(Constants.DBSAVE_ERROR) };
                }

                try
                {
                    //check if our table tables, create if it doesn't
                    var command = conn.CreateCommand();
                    command.CommandText = string.Format(selectIdsForAll, _tableName.Value);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        toReturn.Add(new KindKey(reader.GetString(0)));
                    }
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error reading data from database{0}{1}", Environment.NewLine, ex.ToString()));
                    return new List<KindKey>() { new KindKey(Constants.DBSAVE_ERROR) };
                }
                finally
                {
                    conn.Close();
                }
            }
            return toReturn;
        }
        /// <summary>
        /// Deletes entities by Id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>returns True if succcessful</returns>
        public bool Delete(KindKey entityId)
        {
            var saveStatus = false;
            using (var conn = new SqliteConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return false;
                }

                try
                {
                    //check if our table tables, create if it doesn't
                    
                    var command = conn.CreateCommand();
                    command.CommandText = string.Format(deleteSql, _tableName.Value);
                    command.Parameters.AddWithValue("@id", entityId.Value);
                    command.ExecuteNonQuery();
                    saveStatus = true;
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error deleting record from database{0}{1}", Environment.NewLine, ex.ToString()));
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
            return saveStatus;
        }

        bool Save(KindKey entityId, KindItem datablob)
        {
            var saveStatus = false;
            using (var conn = new SqliteConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return false;
                }

                try
                {
                    //check if our table tables, create if it doesn't
                    var command = conn.CreateCommand();
                    command.CommandText = string.Format(insertSql, _tableName.Value);
                    command.Parameters.AddWithValue("@id", entityId.Value);
                    command.Parameters.AddWithValue("@datablob", datablob.Value );
                    command.ExecuteNonQuery();
                    saveStatus = true;
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error saving to database{0}{1}", Environment.NewLine, ex.ToString()));
                }
                finally
                {
                    conn.Close();
                }
            }
            return saveStatus;
        }

        public async Task<int> Count()
        {
            int toReturn = -1;
            using (var conn = new SqliteConnection(_db.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return -1; // new NameValuePair() { Name = _tableName.Value, Value = Constants.DBSAVE_ERROR };
                }

                try
                {
                    //check if our table tables, create if it doesn't
                    var command = conn.CreateCommand();
                    command.CommandText = string.Format(selectCountForAll, _tableName.Value);
                    var reader = await command.ExecuteScalarAsync();
                    var res = Convert.ToInt32(reader);
                    toReturn = res; // new NameValuePair() { Name = _tableName.Value, Value = res };
                }
                catch (SqliteException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error reading data from database{0}{1}", Environment.NewLine, ex.ToString()));
                    return -1; // new NameValuePair() { Name = _tableName.Value, Value = Constants.DBSAVE_ERROR };
                }
                finally
                {
                    conn.Close();
                }
            }
            return toReturn;
        }
    }
}