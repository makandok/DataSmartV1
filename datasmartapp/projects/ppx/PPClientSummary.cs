using System;
using System.Collections.Generic;
using System.Linq;
using JhpDataSystem.model;
using JhpDataSystem;
using System.Globalization;

namespace DataSmart.projects.ppx
{
    [SQLite.Table(Constants.KIND_DERIVED_PPX_CLIENTSUMMARY)]
    public class PPClientSummary : ILocalDbEntity
    {
        public const string KindName = Constants.KIND_DERIVED_PPX_CLIENTSUMMARY;

        [SQLite.Ignore]
        public KindKey Id { get; set; }
        [SQLite.Ignore]
        public KindKey EntityId { get; set; }
        [SQLite.Ignore]
        public string KindMetaData { get; set; }

        public DateTime CoreActivityDate { get; set; }
        public ISaveableEntity build()
        {
            Id = new KindKey(KindKey); EntityId = new KindKey(KindKey);
            return this;
        }
                
        public int FormSerial { get; set; }

        private long _itemid = -1L;
        [SQLite.Ignore]
        public long itemId { get { return _itemid; } set { _itemid = value; } }

        [SQLite.PrimaryKey]
        public string KindKey { get; set; }

        public long getItemId()
        {
            if (_itemid == -1L && Id != null)
            {
                _itemid = Id.Value.GetHashCode();
            }
            return _itemid;
        }

        public string DeviceSize { get; set; }

        public string Names { get; set; }
        public int ClientNumber { get; set; }

        public DateTime PlacementDate { get; set; }
        public string Telephone { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }

        public DateTime Day3PhoneCallDate { get; set; }
        public DateTime Day6SmsReminderDate { get; set; }
        public DateTime Day7RemovalPhoneCall { get; set; }
        public DateTime Day9HomeVisitDate { get; set; }
        public DateTime Day14FollowupCallDate { get; set; }
        public DateTime Day49CallDate { get; set; }
        public DateTime Day56PhoneCall { get; set; }

        public List<NameValuePair> ToValuesList()
        {
            var toReturn = new List<NameValuePair>()
            {
                new NameValuePair() {Name=Constants.FIELD_ENTITYID,Value = EntityId.Value },
                new NameValuePair() {Name=Constants.FIELD_ID,Value = Id.Value },
                new NameValuePair() {Name=Constants.FIELD_PPX_PLACEMENTDATE,
                    Value = PlacementDate.ToString(CultureInfo.InvariantCulture) },

                new NameValuePair() {Name=Constants.FIELD_PPX_DATEOFVISIT,
                    Value = CoreActivityDate.ToString(CultureInfo.InvariantCulture) },

                new NameValuePair() {Name=Constants.FIELD_PPX_CARD_SERIAL,Value = FormSerial.ToString() },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTNAME,Value =Names },
                //new NameValuePair() {Name=Constants.FIELD_CARD_SERIAL,Value = FormSerial.ToString() },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTIDNUMBER,Value =ClientNumber.ToString() },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTTEL,Value = Telephone },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTPHYSICALADDR,Value = Address }
            };
            return toReturn;
        }

        public ILocalDbEntity Load(GeneralEntityDataset lookupEntry)
        {
            var expectedFields = Constants.PP_IndexedFieldNames;
            var fieldValues = lookupEntry.FieldValues;
            var allFields = (from field in lookupEntry.FieldValues
                             where expectedFields.Contains(field.Name)
                             select field).ToDictionary(x => x.Name, y => y.Value);
            foreach (var field in expectedFields)
            {
                if (!allFields.ContainsKey(field))
                {
                    allFields[field] = "";
                }
            }

            this.KindKey = lookupEntry.Id.Value;
            this.EntityId = new KindKey(KindKey);
            this.Id = lookupEntry.Id;
            this.KindMetaData = lookupEntry.KindMetaData;

            var deviceSize = string.Empty;
            if (allFields.TryGetValue(Constants.FIELD_PPX_DEVSIZE, out deviceSize))
            {
                this.DeviceSize = deviceSize;
            }

            var dateStr = allFields[Constants.FIELD_PPX_DATEOFVISIT];
            if (!string.IsNullOrWhiteSpace(dateStr))
            {
                this.PlacementDate = Convert.ToDateTime(dateStr);
            }
            else
                this.PlacementDate = DateTime.MinValue;

            CoreActivityDate = PlacementDate;

            var serial = allFields[Constants.FIELD_PPX_CARD_SERIAL];

            if (!string.IsNullOrWhiteSpace(serial))
            {
                this.FormSerial = Convert.ToInt32(serial);
            }
            else
                this.FormSerial = -1;

            this.Names = allFields[Constants.FIELD_PPX_CLIENTNAME];
            var mcnumber = allFields[Constants.FIELD_PPX_CLIENTIDNUMBER];
            
            if (!string.IsNullOrWhiteSpace(mcnumber))
            {
                this.ClientNumber = Convert.ToInt32(mcnumber);
            }
            else
                this.ClientNumber = -1;

            this.Telephone = allFields[Constants.FIELD_PPX_CLIENTTEL];
            this.Address = allFields[Constants.FIELD_PPX_CLIENTPHYSICALADDR];
            return this;
        }
    }
}