using System;
//using Mono.Data.Sqlite;
using JhpDataSystem.model;
using System.Data.SqlClient;
//using SqliteException = System.Data.SqlClient.SqlException;

namespace JhpDataSystem.store
{
    public class TableStore: BaseTableStore
    {
        public TableStore(string tableName) : base(tableName)
        {
            _tableName = new KindName(tableName);
            _db = new LocalDB();
            createKindSql = "create table if not exists {0}(id nvarchar(32) primary key, datablob nvarchar(500));";
            insertSql = "insert or replace into {0}(id, datablob) values (@id, @datablob)";
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

        protected virtual bool Save(KindKey entityId, KindItem datablob)
        {
            //we create if does not exist
            var saveStatus = false;
            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var command = new SqlCommand(string.Format(insertSql, _tableName.Value), conn))
            {
                try
                {
                    conn.Open();
                    //check if our table tables, create if it doesn't
                    command.CommandText = string.Format(insertSql, _tableName.Value);
                    command.Parameters.AddWithValue("@id", entityId.Value);
                    command.Parameters.AddWithValue("@datablob", datablob.Value);
                    saveStatus = command.ExecuteNonQuery() > 0;
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
    }
}