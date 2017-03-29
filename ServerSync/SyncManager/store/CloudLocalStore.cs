using System;
using System.Collections.Generic;
using JhpDataSystem.model;
using System.Data.SqlClient;
using SyncManager.store;
using System.Linq;

namespace JhpDataSystem.store
{
    public class CloudLocalStore<T> : BaseTableStore where T:BlobEntity, new()
    {
        UniqueIdentifierMap _identifierMap;
        public CloudLocalStore(KindName tableName) : base(tableName.Value)
        {
            _identifierMap = new UniqueIdentifierMap();
            createKindSql = "if object_id('{0}') is null create table {0}(recordid int identity(1,1)  primary key, id nvarchar(32) not null unique, entityid nvarchar(32), kindmetadata nvarchar(1000), editday int, editdate bigint, datablob nvarchar(max));";
            insertSql =
                @"
                begin tran
                   update {0} with (serializable) set entityid=@entityid,kindmetadata=@kindmetadata,editday=@editday, editdate=@editdate,datablob =@datablob
                   where id = @id
                   if @@rowcount = 0
                   begin
                      insert into {0} (id,entityid,kindmetadata,editday,editdate,datablob)values(@id,@entityid,@kindmetadata,@editday,@editdate,@datablob);
                   end
                commit tran;
                select recordid from {0} where id = @id;
                ";
            //fix above to include editday and editdate
        }

        /// <summary>
        /// Replaces record in database if it exists or inserts a new one if it doesn't exist. Maintains key supplied
        /// </summary>
        /// <param name="key">Unique id of entity</param>
        /// <param name="dataToSave">Data to save</param>
        /// <returns>Entity Id if successful or default error code</returns>
        public ResultObject Update(T entity)
        {
            var result = Save(entity);
            if (!result.saveStatus)
            {
                return null;
            }
            return result;
        }

        private ResultObject Save(T entity)
        {
            ResultObject toReturn = new ResultObject();
            //we create if does not exist
            var entityIdInt = _identifierMap.Save(entity.EntityId);
            var idInt = _identifierMap.Save(entity.Id);

            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var command = new SqlCommand(string.Format(insertSql, _tableName.Value), conn))
            {
                try
                {
                    conn.Open();
                    //check if our table tables, create if it doesn't
                    command.CommandText = string.Format(insertSql, _tableName.Value);
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.Parameters.AddWithValue("@entityid", entity.EntityId);

                    command.Parameters.AddWithValue("@editdate", entity.EditDate);
                    command.Parameters.AddWithValue("@editday", entity.EditDay);

                    command.Parameters.AddWithValue("@datablob", entity.DataBlob);

                    command.Parameters.AddWithValue("@kindmetadata", entity.KindMetaData);
                    //command.Parameters.AddWithValue("@datablob", entity.Id);

                    var res = Convert.ToInt32(command.ExecuteScalar());
                    toReturn = new ResultObject() { recordId = res, saveStatus = res > 0 };
                }
                catch (SqlException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return null;
                }

                conn.Close();
            }
            return toReturn;
        }

        List<T> getEntities(SqlDataReader reader)
        {
            var toReturn = new List<T>();
            while (reader.Read())
            {
                var entity = new T();
                entity.Id = Convert.ToString(reader["id"]);
                entity.EntityId = Convert.ToString(reader["entityid"]);
                entity.EditDate = Convert.ToInt64(reader["editdate"]);
                entity.EditDay = Convert.ToInt32(reader["editday"]);
                entity.DataBlob = Convert.ToString(reader["datablob"]);
                entity.KindMetaData = Convert.ToString(reader["kindmetadata"]);
                entity.RecordId = Convert.ToInt32(reader["recordid"]);
                toReturn.Add(entity);
            }
            return toReturn;
        }

        internal T GetLatestEntity()
        {
            T entity = null;
            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var command = new SqlCommand(string.Format(insertSql, _tableName.Value), conn))
            {
                try
                {
                    conn.Open();
                    //check if our table tables, create if it doesn't
                    command.CommandText = string.Format(
                        "select top 1 * from {0} order by editdate desc", _tableName.Value);
                    using (var reader = command.ExecuteReader())
                    {
                        entity = getEntities(reader).FirstOrDefault();
                    }
                }
                catch (SqlException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return null;
                }
                conn.Close();
            }
            return entity;
        }

        internal List<T> GetUnsyncedLocalEntities(KindName cloudTable, KindName localTable)
        {
            var diffSql = string.Format(
                @"
select a.* From {0} a join 
(
	select id, editdate from {0} except select id, editdate from {1}
	)b on a.Id = b.id
", cloudTable.Value, localTable.Value);

            var toReturn = new List<T>();
            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var command = new SqlCommand(string.Format(insertSql, _tableName.Value), conn))
            {
                try
                {
                    conn.Open();
                    command.CommandText = diffSql;
                    using (var reader = command.ExecuteReader())
                    {
                        toReturn = getEntities(reader);
                    }
                }
                catch (SqlException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return null;
                }
                conn.Close();
            }
            return toReturn;
        }
    }

    public class ResultObject
    {
        public bool saveStatus { get; set; }
        public int recordId { get; set; }
    }

    public class CloudLocalStore : CloudLocalStore<CloudEntity>
    {
        public CloudLocalStore(KindName tableName) : base(tableName)
        {
        }
    }
}
