using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JhpDataSystem;

namespace DataSmart.projects.session.activity
{
    [Activity(Label = "Provider List")]
    public class ProviderGridDisplayActivity : Activity, ListView.IOnItemClickListener
    {
        ProviderSummaryAdapter _defaultAdapter = null;
        List<SiteProvider> _allPrepexClients;
        SiteProvider _selectedClient = null;
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            _selectedClient = _allPrepexClients[position];
            Android.Widget.Toast.MakeText(this,
                _selectedClient.FirstName +
                (string.IsNullOrWhiteSpace(_selectedClient.MaidenName) ? "" : (" " + _selectedClient.MaidenName))
                + " " + _selectedClient.SurName,
                Android.Widget.ToastLength.Short).Show();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientlist);

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;

            listview.OnItemClickListener = this;

            _allPrepexClients = new
                TablestoreLookupProvider<SiteProvider>(Constants.KIND_SITEPROVIDER)
                .Get();
            _defaultAdapter = new ProviderSummaryAdapter(this, listview, _allPrepexClients);

            listview.Adapter = _defaultAdapter;

            var rgroup = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            rgroup.Visibility = ViewStates.Gone;

            //buttonPerformAction
            var buttonPerformAction = FindViewById<Button>(Resource.Id.buttonPerformAction);
            buttonPerformAction.Visibility = ViewStates.Gone;
            //buttonPerformAction.Click += performActionSpecified;
        }

        void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = _allPrepexClients[position];
            Android.Widget.Toast.MakeText(this,
                                t.FirstName +
                (string.IsNullOrWhiteSpace(t.MaidenName) ? "" : (" " + t.MaidenName))
                + " " + t.SurName
                , Android.Widget.ToastLength.Short).Show();
        }        
    }
}