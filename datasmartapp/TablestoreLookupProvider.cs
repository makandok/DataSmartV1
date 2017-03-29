using System.Collections.Generic;
using JhpDataSystem.model;
using DataSmart.store;
using Newtonsoft.Json;

namespace DataSmart
{
    public class TablestoreLookupProvider<T> : ClientLookupProvider<T> where T : class, ILocalDbEntity, new()
    {
        public TablestoreLookupProvider(string kindName) : base(kindName)
        {
        }

        public override List<T> Get()
        {
            //we use tablestore
            var kindItems = new TableStore(_kindName).GetAllBlobs();
            var toReturn = new List<T>();
            foreach (var kindItem in kindItems)
            {
                var ge = JsonConvert.DeserializeObject<GeneralEntityDataset>(kindItem.Value);
                toReturn.Add(new T().Load(ge) as T);
            }
            return toReturn;
        }

        public override int Update(List<T> clients)
        {
            foreach (var client in clients)
            {
                var ge = new GeneralEntityDataset()
                {
                    EntityId = client.EntityId,
                    FormName = _kindName,
                    Id = client.Id,
                    FieldValues = client.ToValuesList(),
                    KindMetaData = client.KindMetaData
                };
                var saveable = new DbSaveableEntity(ge);
                saveable.Save();
            }
            return 0;
        }

        public override int InsertOrReplace(List<T> clients)
        {
            return Update(clients);
        }

        public override int GetCount()
        {
            return new TableStore(_kindName).Count().Result;
        }
    }


}