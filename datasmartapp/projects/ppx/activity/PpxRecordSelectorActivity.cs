using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;
using DataSmart.modules;

namespace DataSmart.projects.ppx.activity
{
    [Activity(Label = "Select Record")]
    public class PpxRecordSelectorActivity : RecordSelectorBase, ListView.IOnItemClickListener
    {
        protected override List<VisitSummary> getRecordsForClient(string entityId)
        {
            //we get entityid from the intent
            var records = getRecordSummaries(entityId);
            return (from table in records select new VisitSummary(table, Constants.PPX_KIND_DISPLAYNAMES[table.KindName])).ToList();
        }

        protected override string getCurrentClientId(string clientString)
        {
            var client = Newtonsoft.Json.JsonConvert.DeserializeObject<PPClientSummary>(clientString);
            return client.EntityId.Value;
        }
    }
}