using System;
using System.Collections.Generic;
using System.Linq;
using JhpDataSystem.model;
using JhpDataSystem;
using System.Globalization;

namespace DataSmart.projects.vmc
{
    [SQLite.Table(Constants.KIND_DERIVED_VMMC_CLIENTSUMMARY)]
    public class VmmcClientSummary : ILocalDbEntity
    {
        public static string KindName { get { return Constants.KIND_DERIVED_PPX_CLIENTSUMMARY; } }
        private long _itemid = -1L;
        [SQLite.Ignore]
        public long itemId { get { return _itemid; } set { _itemid = value; } }
        public long getItemId()
        {
            if (_itemid == -1L && Id != null)
            {
                _itemid = Id.Value.GetHashCode();
            }
            return _itemid;
        }

        public int FormSerial { get; set; }

        [SQLite.PrimaryKey]
        public string KindKey { get; set; }

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
            //CoreActivityDate = MCDate;
            return this;
        }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime MCDate { get; set; }

        public int MCNumber { get; set; }
        public string ClientTel { get; set; }
        public DateTime DateOfBirth { get; set; }

        public List<NameValuePair> ToValuesList()
        {
            var toReturn = new List<NameValuePair>()
            {
                new NameValuePair() {Name=Constants.FIELD_ENTITYID,Value = EntityId.Value },
                new NameValuePair() {Name=Constants.FIELD_ID,Value = Id.Value },
                new NameValuePair() {Name=Constants.FIELD_VMMC_DATEOFVISIT,Value = CoreActivityDate.ToString(CultureInfo.InvariantCulture) },
                new NameValuePair() {Name=Constants.FIELD_VMMC_MCDATE,
                    Value = MCDate.ToString(CultureInfo.InvariantCulture) },

                new NameValuePair() {Name=Constants.FIELD_VMMC_CARD_SERIAL,Value = FormSerial.ToString() },
                 new NameValuePair() {Name=Constants.FIELD_VMMC_MCNUMBER,Value = MCNumber.ToString() },
                new NameValuePair() {Name=Constants.FIELD_VMMC_CLIENTFIRSTNAME,Value =FirstName },
                new NameValuePair() {Name=Constants.FIELD_VMMC_CLIENTLASTNAME,Value =LastName },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTTEL,Value = ClientTel },
                new NameValuePair() {Name=Constants.FIELD_VMMC_DOB,Value = DateOfBirth.ToString(CultureInfo.InvariantCulture) },
            };
            return toReturn;
        }

        public ILocalDbEntity Load(GeneralEntityDataset lookupEntry)
        {
            var expectedFields = Constants.VMMC_IndexedFieldNames;
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
            this.Id = new KindKey(KindKey);
            this.KindMetaData = lookupEntry.KindMetaData;

            var dateStr = allFields[Constants.FIELD_VMMC_DATEOFVISIT];
            DateTime coreActivityDate;
            if (DateTime.TryParse(dateStr, out coreActivityDate))
            {
                this.CoreActivityDate = coreActivityDate;
            }

            var mcdatestr = allFields[Constants.FIELD_VMMC_MCDATE];
            DateTime mcdate;
            if (DateTime.TryParse(mcdatestr, out mcdate))
            {
                this.MCDate = mcdate;
            }

            this.LastName = allFields[Constants.FIELD_VMMC_CLIENTLASTNAME];
            this.FirstName = allFields[Constants.FIELD_VMMC_CLIENTFIRSTNAME];

            var formSerial = allFields[Constants.FIELD_VMMC_CARD_SERIAL];
            if (!string.IsNullOrWhiteSpace(formSerial))
            {
                this.FormSerial = Convert.ToInt32(formSerial);
            }
            else
                this.FormSerial = -1;

            var vmmcNumber = allFields[Constants.FIELD_VMMC_MCNUMBER];
            if (!string.IsNullOrWhiteSpace(vmmcNumber))
            {
                this.MCNumber = Convert.ToInt32(vmmcNumber);
            }
            else
                this.MCNumber = -1;         

            this.ClientTel = allFields[Constants.FIELD_VMMC_CLIENTTEL];

            var dobStr = allFields[Constants.FIELD_VMMC_DOB];
            DateTime dob;
            if (DateTime.TryParse(dobStr, out dob))
            {
                this.DateOfBirth = dob;
            }
            return this;
        }
    }
}