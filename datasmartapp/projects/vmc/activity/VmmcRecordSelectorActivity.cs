using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;
using DataSmart.modules;

namespace DataSmart.projects.vmc.activity
{
    [Activity(Label = "Select Record")]
    public class VmmcRecordSelectorActivity : RecordSelectorBase, ListView.IOnItemClickListener
    {
        protected override List<VisitSummary> getRecordsForClient(string entityId)
        {
            //we get entityid from the intent
            var records = getRecordSummaries(entityId);
            return (from table in records select new VisitSummary(table, Constants.VMMC_KIND_DISPLAYNAMES[table.KindName])).ToList();
        }

        protected override string getCurrentClientId(string clientString)
        {
            var client = Newtonsoft.Json.JsonConvert.DeserializeObject<VmmcClientSummary>(clientString);
            return client.EntityId.Value;
        }
    }
}