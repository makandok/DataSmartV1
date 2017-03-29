using System;
using System.Collections.Generic;
using JhpDataSystem.model;
using JhpDataSystem;

namespace DataSmart.projects.session
{
    public class SiteSessionTable
    {
        public int TableId { get; set; }
        public decimal StartTime { get; set; }
        public decimal EndTime { get; set; }
        public KindKey Provider { get; set; }
        public KindKey Assistant { get; set; }
        public KindKey Counsellor { get; set; }
        public KindKey HygieneAssistant { get; set; }
        public List<KindKey> Circumcisions { get; set; }
    }

    public class SiteSession : ILocalDbEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }
        public string KindMetaData { get; set; }

        public string FacilityId { get; set; }
        public DateTime SessionDate { get; set; }

        public List<SiteSessionTable> Tables { get; set; }

        public DateTime CoreActivityDate
        {
            get
            {
                return SessionDate; ;
            }
            set
            {
            }
        }

        public ISaveableEntity build()
        {
            return this;
        }

        public List<NameValuePair> ToValuesList()
        {
            return null;
        }

        public ILocalDbEntity Load(GeneralEntityDataset clientSummary)
        {
            return this;
        }

        public long getItemId()
        {
            return Id == null ? -1L : Id.GetHashCode();
        }
    }

    //public class ServiceProvider
    //{
    //    public KindKey Id { get; set; }
    //    public string FirstName { get; set; }
    //    public string SurName { get; set; }
    //    public string MaidenName { get; set; }
    //    public string NRC { get; set; }
    //    public string ContactNumber { get; set; }
    //    public string HomeFacility { get; set; }
    //}

    public class SiteProvider : ILocalDbEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }
        public string KindMetaData { get; set; }

        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string MaidenName { get; set; }

        public string NRC { get; set; }

        public string ContactNumber { get; set; }
        public string HomeFacility { get; set; }

        public DateTime CoreActivityDate { get; set; }

        public GeneralEntityDataset asGeneralEntity(KindName name)
        {
            return new GeneralEntityDataset()
            {
                Id = this.Id,
                EntityId = this.EntityId,
                KindMetaData = this.KindMetaData,
                FormName = name.Value,
                FieldValues = ToValuesList()
            };
        }

        public long getItemId()
        {
            return Id == null ? -1L : Id.GetHashCode();
        }

        public ISaveableEntity build()
        {
            return this;
        }

        public List<NameValuePair> ToValuesList()
        {
            return new List<NameValuePair>() {
                    new NameValuePair() {Name=Constants.FIELD_ID, Value=this.Id.Value },
                    new NameValuePair() {Name=Constants.FIELD_ENTITYID, Value=this.EntityId.Value },
                    new NameValuePair() {Name=Constants.SYS_FIELD_FirstName, Value=this.FirstName },
                    new NameValuePair() {Name=Constants.SYS_FIELD_SurName, Value=this.SurName},
                    new NameValuePair() {Name=Constants.SYS_FIELD_MaidenName, Value=this.MaidenName},
                    new NameValuePair() {Name=Constants.SYS_FIELD_NRC, Value=this.NRC },
                    new NameValuePair() {Name=Constants.SYS_FIELD_ContactNumber, Value=this.ContactNumber},
                    new NameValuePair() {Name=Constants.SYS_FIELD_HomeFacilityName, Value=this.HomeFacility },
                };
        }

        public ILocalDbEntity Load(GeneralEntityDataset clientSummary)
        {
            return this;
        }
    }
}