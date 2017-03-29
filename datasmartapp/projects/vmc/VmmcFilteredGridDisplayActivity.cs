using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;
using DataSmart.store;
using DataSmart.Utilities;
using System.Threading.Tasks;

namespace DataSmart.projects.vmc
{
    [Activity(Label = "Client List")]
    public class VmmcFilteredGridDisplayActivity : Activity, ListView.IOnItemClickListener
    {
        VmmcClientSummaryAdapter _defaultAdapter = null;
        List<VmmcClientSummary> _allPrepexClients;
        VmmcClientSummary _selectedClient = null;
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            _selectedClient = _allPrepexClients[position];
            Android.Widget.Toast.MakeText(this,
                _selectedClient.FirstName + " " + _selectedClient.LastName, Android.Widget.ToastLength.Short).Show();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientlist);

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;

            listview.OnItemClickListener = this;

            _allPrepexClients = new VmmcLookupProvider().Get();
            _defaultAdapter = new VmmcClientSummaryAdapter(this, listview, _allPrepexClients);

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
            Android.Widget.Toast.MakeText(this, t.FirstName + " " + t.LastName, Android.Widget.ToastLength.Short).Show();
        }        
    }
}