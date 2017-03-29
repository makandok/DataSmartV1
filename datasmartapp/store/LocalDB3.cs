using System;

namespace DataSmart.store
{
    public class OutDb: LocalDB3
    {
        public new string defaultDatabaseName = "JhpOutDb.db3";
        public OutDb():base()
        {

        }
    }

    public class LocalDB3
    {
        public string defaultDatabaseName = "JhpDefaultDB3.db3";
        //public string ConnectionString { get; set; }
        public LocalDB3()
        {
            var personalFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var dbpath = System.IO.Path.Combine(personalFolder, defaultDatabaseName);
            var localDb = new SQLite.SQLiteConnection(dbpath);
            DB = localDb;
        }

        public SQLite.SQLiteConnection DB{get;set;}

        public string newId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}