using JhpDataSystem.model;
using Newtonsoft.Json;
using System;

namespace JhpDataSystem.store
{
    public class DbSaveableEntity
    {
        public ISaveableEntity Entity { get; set; }

        public DbSaveableEntity(ISaveableEntity entity)
        {
            Entity = entity;
        }

        public KindName kindName { get; set; }

        public KindKey Id { get { return Entity.Id; } set { Entity.Id = value; } }

        public KindKey EntityId { get { return Entity.EntityId; } set { Entity.EntityId = value; } }

        public string getJson()
        {
            return JsonConvert.SerializeObject(Entity);
        }

        public string getEncryptedJson()
        {
            return getEncryptedEntity().Value.Value;
        }

        public EncryptedKindEntity getEncryptedEntity()
        {
            var asString = JsonConvert.SerializeObject(Entity);
            var encrypted = asString.Encrypt();
            return new EncryptedKindEntity(encrypted);
        }

        public static T fromJson<T>(KindItem jsonString) where T : ISaveableEntity
        {
            return JsonConvert.DeserializeObject<T>(jsonString.Value);
        }

        public void Save()
        {
            //if (Entity == null)
            //    throw new ArgumentNullException("Wrapped entity can not be null");
            //Save(new KindItem(getJson()));
        }

        public void Save(KindItem kindItem)
        {
            //if (Entity == null)
            //    throw new ArgumentNullException("Wrapped entity can not be null");
            //AppInstance.Instance.LocalEntityStoreInstance.Save(Entity.Id, kindName, kindItem);
        }
    }
}