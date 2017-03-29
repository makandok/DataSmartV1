using System.Collections.Generic;

namespace JhpDataSystem.model
{
    public class AppUser : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }

        public string UserId { get; set; }
        public string Names { get; set; }
        public string KnownBolg { get; set; }

        public string KindMetaData { get; set; }
        public long getItemId()
        {
            return Id == null ? -1L : Id.GetHashCode();
        }
        public GeneralEntityDataset asGeneralEntity(KindName name)
        {
            return new GeneralEntityDataset()
            {
                Id = this.Id,
                EntityId = this.EntityId,

                KindMetaData = this.KindMetaData,

                FormName = name.Value,
                FieldValues = new List<NameValuePair>() {
                    new NameValuePair() {Name=Constants.FIELD_ID, Value=this.Id.Value },
                    new NameValuePair() {Name=Constants.FIELD_ENTITYID, Value=this.EntityId.Value },

                    new NameValuePair() {Name=Constants.SYS_FIELD_USERID, Value=this.UserId },
                    new NameValuePair() {Name=Constants.SYS_FIELD_USERNAMES, Value=this.Names},
                    new NameValuePair() {Name=Constants.SYS_FIELD_PASSWDHASH, Value=this.KnownBolg },
                }
            };
        }
    }
}