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
                var res = new TableStore(table.Value).GetAllBlobs();
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
                var res = new TableStore(table.Value).Count();
                toReturn.Add(new NameValuePair() { Name = table.Value, Value = res.Result.ToString() });
            }
            return toReturn;
        }
    }

}
