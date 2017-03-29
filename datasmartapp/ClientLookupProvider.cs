using System.Collections.Generic;
using System.Linq;
using JhpDataSystem.model;
using DataSmart.store;

namespace DataSmart
{
    public class ClientLookupProvider<T> where T : class, ILocalDbEntity, new()
    {
        protected string _kindName;
        public ClientLookupProvider(string kindName)
        {
            _kindName = kindName;
        }

        public virtual List<T> Get()
        {
            var all = new LocalDB3().DB
                .Table<T>()
                .ToList<T>()
                .OrderByDescending(t => t.CoreActivityDate)
                .ToList();
            all.ForEach(t => { t.build(); });
            return all;
        }
        public virtual int Update(List<T> clients)
        {
            var all = new LocalDB3().DB
                .UpdateAll(clients);
            return all;
        }
        public virtual int InsertOrReplace(List<T> clients)
        {
            var db = new LocalDB3().DB;
            foreach(var client in clients)
                db.InsertOrReplace(client);
            return 0;
        }
        public virtual int GetCount()
        {
            var all = new LocalDB3().DB
                .ExecuteScalar<int>("select count(*) from " + _kindName);
            return all;
        }
    }
}