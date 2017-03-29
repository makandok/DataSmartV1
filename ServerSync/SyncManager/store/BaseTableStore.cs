using System;
//using Mono.Data.Sqlite;
using System.Collections.Generic;
using JhpDataSystem.model;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
//using SqliteException = System.Data.SqlClient.SqlException;


namespace JhpDataSystem.store
{
    public class BaseTableStore
    {
        public ProcessLogger MainLogger
        {
            get; set;
        }

        protected string TableType = "blob";
        protected KindName _tableName;
        protected LocalDB _db;

        protected string dropKindSql = "if object_id('{0}') is not null begin delete from {0}; drop table {0}; end";

        protected string createKindSql = "create table if not exists {0}(id nvarchar(32) primary key, datablob nvarchar(500));";
        protected string insertSql = "insert or replace into {0}(id, datablob) values (@id, @datablob)";

        protected const string deleteSql = "delete from {0} where id = @id";
        protected const string deleteAllSql = "delete from {0}";

        protected const string selectIdsForAll = "select id from {0}";
        protected const string selectCountForAll = "select count(id) from {0}";
        protected const string selectDatablobs = "select datablob from {0}";
        protected const string selectDatablobsById = "select datablob from {0} where id = @id";

        public BaseTableStore(string tableName)
        {
            _tableName = new KindName(tableName);
            _db = new LocalDB();
        }

        /// <summary>
        /// Creates Kind if it does not exist
        /// </summary>
        /// <returns>True if successful</returns>
        //dropAdRecreate
        public virtual bool build(bool dropAndRecreate = false)
        {
            //we create if does not exist
            var saveStatus = false;
            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var command = new SqlCommand(string.Format(createKindSql, _tableName.Value), conn))
            {
                try
                {
                    conn.Open();
                    if (dropAndRecreate)
                    {
                        command.CommandText = string.Format(dropKindSql, _tableName.Value);
                        command.ExecuteNonQuery();

                        command.CommandText = string.Format(createKindSql, _tableName.Value);
                    }
                    command.ExecuteNonQuery();
                    saveStatus = true;
                }
                catch (SqlException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return false;
                }

                conn.Close();
            }
            return saveStatus;
        }

        //public bool build()
        //{
        //    //we create if does not exist
        //    var saveStatus = false;
        //    using (var conn = new SqlConnection(_db.ConnectionString))
        //    using (var command = new SqlCommand(string.Format(createKindSql, _tableName.Value), conn))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            command.ExecuteNonQuery();
        //            saveStatus = true;
        //        }
        //        catch (SqlException ex)
        //        {
        //            if (MainLogger != null)
        //                MainLogger.Log(string.Format(
        //                    "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
        //            return false;
        //        }

        //        conn.Close();
        //    }
        //    return saveStatus;
        //}

        internal bool DeleteAll()
        {
            var saveStatus = false;
            using (var conn = new SqlConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
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
                catch (SqlException ex)
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
        private List<KindItem> GetBlob(KindKey entityId = null)
        {
            var toReturn = new List<KindItem>();
            using (var conn = new SqlConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
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
                catch (SqlException ex)
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
            using (var conn = new SqlConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
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
                catch (SqlException ex)
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
            using (var conn = new SqlConnection(_db.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
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
                catch (SqlException ex)
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

        public async Task<int> Count()
        {
            int toReturn = -1;
            using (var conn = new SqlConnection(_db.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();
                }
                catch (SqlException ex)
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
                catch (SqlException ex)
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
