using JhpDataSystem.store;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncManager.store
{
    public class FieldDictionaryStore
    {
        LocalDB _db;
        DataTable fieldDictionary;

        public string DatasetName { get; internal set; }

        public FieldDictionaryStore()
        {
            _db = new LocalDB();
            fieldDictionary = new DataTable() { TableName = "FieldDictionary" };
            fieldDictionary.Columns.Add("project");
            fieldDictionary.Columns.Add("dataType");
            fieldDictionary.Columns.Add("fieldName");
            fieldDictionary.Columns.Add("fieldType");
            fieldDictionary.Columns.Add("IsIndexed");
            fieldDictionary.Columns.Add("IsRequired");
            fieldDictionary.Columns.Add("Label");
            fieldDictionary.Columns.Add("name");
            fieldDictionary.Columns.Add("PageId");
            fieldDictionary.Columns.Add("pageName");
            fieldDictionary.Columns.Add("choiceName");
            fieldDictionary.Columns.Add("groupKey");
            fieldDictionary.Columns.Add("listName");
            fieldDictionary.Columns.Add("lookupValue");
            fieldDictionary.Columns.Add("tableName");
        }

        public void getFields(string projectName, List<MyFieldItem> fields)
        {
            foreach (var field in fields)
            {
                if (field.name == "ilsp_district_name")
                {

                }

                var row = fieldDictionary.NewRow();
                row["project"] = projectName;
                row["dataType"] = field.dataType;
                row["fieldName"] = field.fieldName;
                row["fieldType"] = field.fieldType;
                row["IsIndexed"] = field.IsIndexed;
                row["IsRequired"] = field.IsRequired;
                row["Label"] = field.Label;
                row["name"] = field.name;
                row["PageId"] = field.PageId;
                row["pageName"] = field.pageName;
                row["tableName"] = DatasetName;
                row["choiceName"] = "";
                row["groupKey"] = "";
                row["listName"] = field.listName ?? "";
                row["lookupValue"] = field.lookupValue ?? "";
                if (string.IsNullOrWhiteSpace(field.fieldName))
                {
                    //row["choiceName"] = "";
                }
                else if (field.name.Contains("_sx_"))
                {
                    var res = field.name.Split(
                        new string[] { "_sx_" }, StringSplitOptions.RemoveEmptyEntries);
                    var groupkey = res[1];

                    row["groupKey"] = groupkey;

                    if (field.fieldType == "Single Select" || field.fieldType == "MultiSelect")
                    {
                        var val = res[0].Replace(field.fieldName + "_", string.Empty);
                        row["choiceName"] = val;
                    }
                }
                else
                {
                    var val = field.name.Replace(field.fieldName + "_", string.Empty);
                    row["choiceName"] = val;
                }

                fieldDictionary.Rows.Add(row);
            }
        }

        public void saveToDb()
        {
            //we first clear the table
            using (var sqlConnection = new SqlConnection(_db.ConnectionString))
            using (var sqlCommand = new 
                SqlCommand(string.Format(
                    "delete from {0} where tablename = @tablename",
                    fieldDictionary.TableName), sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@tablename", DatasetName);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }

            //and save
            using (var sqlBulkCopy = new SqlBulkCopy(_db.ConnectionString)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = fieldDictionary.TableName
            })
            {
                sqlBulkCopy.WriteToServer(fieldDictionary);
                //try
                //{
                //    sqlBulkCopy.WriteToServer(_fieldValues);
                //}
                //catch(Exception ex)
                //{

                //}

                sqlBulkCopy.Close();
            }
        }

    }

}
