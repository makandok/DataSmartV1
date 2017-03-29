using JhpDataSystem.model;
using JhpDataSystem.store;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace SyncManager.store
{
    public class FieldValueStore: BaseTableStore
    {
        string _kindName;
        public FieldValueStore(string kindName):base(kindName)
        {
            _kindName = kindName;
            //check if the table exists
            //create if it doesn't
            createKindSql = "if object_id('{0}') is null create table {0}(id int identity(1,1) primary key, recordid int, fieldname nvarchar(255), fieldvalue nvarchar(255), foreign key (recordid) references {1}(recordid) on delete cascade on update set null);";
            initialise();
        }

        public int batchSize { get; set; }        

        DataTable _fieldValues = null;

        int _currentRecs = 0;
        public void initialise(List<FieldItem> fields = null)
        {
            _currentRecs = 0;
            if (_fieldValues != null)
            {
                _fieldValues.Clear();
                _fieldValues.AcceptChanges();
            }
            else
            {
                var myFieldValues = new DataTable();
                myFieldValues.Columns.Add(new DataColumn("recordid", typeof(int)));
                myFieldValues.Columns.Add(new DataColumn("fieldname", typeof(string)));
                myFieldValues.Columns.Add(new DataColumn("fieldvalue", typeof(string)));
                _fieldValues = myFieldValues;
            }
        }

        object _localLock = new object();
        public bool Save(GeneralEntityDataset entity, LocalEntity localEntity, int recordId)
        {
            lock (_localLock)
            {
                addToCache(entity, recordId);
                _currentRecs++;

                if (_currentRecs >= batchSize)
                {
                    //we save
                    finalise();
                }                
            }
            return true;
        }

        public void finalise()
        {
            if (_currentRecs > 0)
            {
                saveToDb();
            }
            initialise();
        }

        public bool addToCache(GeneralEntityDataset entity, int recordId)
        {
            foreach (var fv in entity.FieldValues)
            {
                _fieldValues.Rows.Add(
                    recordId, fv.Name, fv.Value);
            }
            return true;
        }

        void saveToDb()
        {
            if (_currentRecs == 0) return;
            using (var sqlBulkCopy = new SqlBulkCopy(_db.ConnectionString)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = _kindName
            })
            {
                foreach(DataColumn column in _fieldValues.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                sqlBulkCopy.WriteToServer(_fieldValues);
                sqlBulkCopy.Close();
            }
        }

        public override bool build(bool dropAndRecreate = false)
        {
            //we create if does not exist
            var saveStatus = false;
            var createTableSql = string.Format(createKindSql, _tableName.Value, _tableName.Value.Replace("_fvs", ""));
            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var command = new SqlCommand(createTableSql, conn))
            {
                try
                {
                    conn.Open();
                    if (dropAndRecreate)
                    {
                        command.CommandText = string.Format(dropKindSql, _tableName.Value);
                        command.ExecuteNonQuery();

                        command.CommandText = createTableSql;
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
    }
}
