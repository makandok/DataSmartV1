using System;
using System.Data.SqlClient;

namespace JhpDataSystem.store
{
    public class LocalDB
    {
        //public const string defaultDatabaseName = "JhpDefaultDB1.db3";
        public string ConnectionString { get; set; }
        public LocalDB()
        {
            var connString = SyncManager.Properties.
                Settings.Default.TestDbConnString;
            ConnectionString = connString;
        }

        public string newId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}