using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;
using DataSmart.store;
using Android.Content;

namespace DataSmart.modules
{
    public class RecordSelectorBase : Activity, ListView.IOnItemClickListener
    {
        protected List<VisitSummary> _allItem;
        protected RecordSummary _selectedItem = null;

        public string DisplayName { get; set; }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            //we get the selected client and return
            _selectedItem = _allItem[position].Wrapped;
        }

        protected List<RecordSummary> getRecordSummaries(string entityId)
        {
            //we get entityid from the intent
            return new LocalDB3().DB.Query<RecordSummary>(
               string.Format("select * from {0} where EntityId = @entityid",
               Constants.KIND_DERIVED_RECORDSUMMARY), entityId);
        }

        protected virtual List<VisitSummary> getRecordsForClient(string entityId)
        {
            return null;
        }

        protected virtual string getCurrentClientId(string clientString)
        {
            return null;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (this.Intent.Extras == null)
                return;
            doOnCreate(savedInstanceState);
        }

        protected void doOnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (this.Intent.Extras == null)
                return;

            SetContentView(Resource.Layout.clientlist);

            var clientString = this.Intent.Extras
                .GetString(Constants.BUNDLE_SELECTEDCLIENT);

            var entityId = getCurrentClientId(clientString);

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;
            listview.OnItemClickListener = this;

            _allItem = getRecordsForClient(entityId);

            var adapter = new RecordSummaryAdapter(this, listview, _allItem);
            listview.Adapter = adapter;

            //we hide the client summary options
            var rgroupCSOptions = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            rgroupCSOptions.Visibility = ViewStates.Gone;

            var buttonPerformAction = FindViewById<Button>(Resource.Id.buttonPerformAction);
            buttonPerformAction.Text = "Edit Selected Record";
            buttonPerformAction.Click += performActionSpecified;
        }

        protected void performActionSpecified(object sender, EventArgs e)
        {
            if (_selectedItem == null)
            {
                Android.Widget.Toast.MakeText(this,
                    "No clients selected. Please select a client.",
                    Android.Widget.ToastLength.Long).Show();
                return;
            }

            var intent = new Intent()
                .PutExtra(Constants.BUNDLE_SELECTEDCLIENT,
                    this.Intent.Extras.GetString(Constants.BUNDLE_SELECTEDCLIENT))
                .PutExtra(Constants.BUNDLE_SELECTEDRECORD_ID, _selectedItem.Id)
                .PutExtra(Constants.BUNDLE_SELECTEDRECORD,
                Newtonsoft.Json.JsonConvert.SerializeObject(_selectedItem));
            SetResult(Result.Ok, intent);
            Finish();
        }
    }

}