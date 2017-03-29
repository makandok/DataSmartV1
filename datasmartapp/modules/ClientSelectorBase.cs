using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;

namespace DataSmart.modules
{
    [Activity(Label = "Client Selector")]
    public class ClientSelectorBase<T> :  Activity, ListView.IOnItemClickListener where T : class, ILocalDbEntity, new()
    {
        //PpxClientSummaryAdapter _defaultAdapter = null;
        protected List<T> _allPrepexClients;
        protected T _selectedClient = null;
        protected string NEXT_TYPE = string.Empty;

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            //we get the selected client and return
            var t = _allPrepexClients[position];
            _selectedClient = t;
            doOnClick(_selectedClient);
        }

        protected virtual void doOnClick(T client)
        {
            Android.Widget.Toast.MakeText(this, client.ToString(), Android.Widget.ToastLength.Short).Show();
        }

        protected virtual List<T> getClientSummaries()
        {
            return null;
        }

        protected virtual void setClientSummaryAdapter(ListView view, List<T> clientSumaries )
        {
            throw new Exception("method setClientSummaryAdapter not overidden");
            //_allPrepexClients = new PpxLookupProvider().Get();
            //_defaultAdapter = new PpxClientSummaryAdapter(this, listview, _allPrepexClients);
            //listview.Adapter = _defaultAdapter;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientlist);

            if (this.Intent.Extras != null && this.Intent.Extras.ContainsKey(Constants.BUNDLE_NEXTACTIVITY_TYPE))
            {
                //KIND_PP_NEXTVIEW
                NEXT_TYPE = this.Intent.Extras.GetString(Constants.BUNDLE_NEXTACTIVITY_TYPE);
            }

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;

            listview.OnItemClickListener = this;

            _allPrepexClients = getClientSummaries();
            setClientSummaryAdapter(listview, _allPrepexClients);            

            //we hide the client summary options
            var rgroupCSOptions = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            rgroupCSOptions.Visibility = ViewStates.Gone;

            var buttonPerformAction = FindViewById<Button>(Resource.Id.buttonPerformAction);
            buttonPerformAction.Text = "Use Selected Client";
            buttonPerformAction.Click += performActionSpecified;
        }

        void performActionSpecified(object sender, EventArgs e)
        {
            if (_selectedClient == null)
            {
                Android.Widget.Toast.MakeText(this,
                    "No clients selected. Please select a client.",
                    Android.Widget.ToastLength.Long).Show();
                return;
            }

            var asString = Newtonsoft.Json.JsonConvert.SerializeObject(_selectedClient);
            var intent = new Intent().PutExtra(Constants.BUNDLE_SELECTEDCLIENT, asString);
            intent.PutExtra(Constants.BUNDLE_NEXTACTIVITY_TYPE, NEXT_TYPE);
            //intent.SetFlags(ActivityFlags.NewTask);
            SetResult(Result.Ok, intent);
            Finish();
        }
    }

}