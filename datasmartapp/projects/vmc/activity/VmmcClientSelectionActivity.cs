using System.Collections.Generic;
using Android.App;
using Android.Widget;
using DataSmart.modules;

namespace DataSmart.projects.vmc.activity
{
    [Activity(Label = "Client Selector")]
    public class VmmcClientSelectionActivity : ClientSelectorBase<VmmcClientSummary>
    {
        VmmcClientSummaryAdapter _defaultAdapter = null;

        protected override void doOnClick(VmmcClientSummary client)
        {
            Android.Widget.Toast.MakeText(this,
                client.FirstName + " " + client.LastName, Android.Widget.ToastLength.Short).Show();
        }

        protected override List<VmmcClientSummary> getClientSummaries()
        {
            return new VmmcLookupProvider().Get();
        }

        protected override void setClientSummaryAdapter(ListView view, List<VmmcClientSummary> clientSumaries)
        {
            _defaultAdapter = new VmmcClientSummaryAdapter(this, view, clientSumaries);
            view.Adapter = _defaultAdapter;
        }
    }
}