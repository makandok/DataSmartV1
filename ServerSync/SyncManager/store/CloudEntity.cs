using JhpDataSystem;
using System;

namespace SyncManager.store
{
    public class BlobEntity
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public string KindMetaData { get; set; }
        public string DataBlob { get; set; }
        //public DateTime DateAdded { get; set; }
        public string FormName { get; set; }
        public long EditDate { get; set; }
        public int EditDay { get; set; }
        public int RecordId { get; set; }
    }

    public class CloudEntity: BlobEntity
    {
    }

    public class LocalEntity: BlobEntity
    {
    }

    public class DeidEntity : BlobEntity
    {
    }

    public class EntityConverter
    {
        public LocalEntity toLocalEntity(CloudEntity entity)
        {
            var decrypted = entity.DataBlob.Decrypt();
            return new LocalEntity()
            {
                Id = entity.Id,
                EntityId = entity.EntityId,
                DataBlob = decrypted.Value,
                EditDate = entity.EditDate,
                EditDay = entity.EditDay,
                FormName = entity.FormName,
                KindMetaData = entity.KindMetaData
            };
        }

        public CloudEntity toCloudEntity(LocalEntity entity)
        {
            var encrypted = entity.DataBlob.Encrypt();
            return new CloudEntity()
            {
                Id = entity.Id,
                EntityId = entity.EntityId,
                DataBlob = encrypted.Value,
                EditDate = entity.EditDate,
                EditDay = entity.EditDay,
                FormName = entity.FormName,
                KindMetaData = entity.KindMetaData
            };
        }

        public DeidEntity toDeidEntity(LocalEntity entity)
        {
            var deidBlob = entity.DataBlob.deidentifyBlob(entity.FormName);
            return new DeidEntity()
            {
                Id = entity.Id,
                EntityId = entity.EntityId,
                DataBlob = deidBlob,
                EditDate = entity.EditDate,
                EditDay = entity.EditDay,
                FormName = entity.FormName,
                KindMetaData = entity.KindMetaData
            };
        }
    }
}
