using JhpDataSystem.store;
using System;
using System.Data.SqlClient;

namespace SyncManager.store
{
    public class UniqueIdentifierMap
    {
        //string _createKindSql = "if object_id('{0}') is null create table {0}(recordid int identity(1,1)  primary key, id nvarchar(32) not null unique, entityid nvarchar(32), kindmetadata nvarchar(1000), editday int, editdate bigint, datablob nvarchar(max));";
        string _insertSql =
                @"
                    begin tran
                    if not exists (select * from UniqueIdMap where stringid = @stringid)
                    begin
                        insert into UniqueIdMap values (@stringid)
                    end
                    commit tran;
                    select IntegerId from UniqueIdMap where stringid = @stringid
                ";

        public int Save(string identifierKey)
        {
            var _db = new LocalDB();
            int toReturn = -1;
            //we create if does not exist
            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var command = new SqlCommand(string.Format(_insertSql), conn))
            {
                try
                {
                    conn.Open();
                    command.Parameters.AddWithValue("@stringid", identifierKey);
                    var res = Convert.ToInt32(command.ExecuteScalar());
                    toReturn = res;
                }
                catch (SqlException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return -1;
                }
                conn.Close();
            }
            return toReturn;
        }
        public ProcessLogger MainLogger
        {
            get; set;
        }
    }
}
