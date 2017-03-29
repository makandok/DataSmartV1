using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;
using Android.Content;
using DataSmart.modules;

namespace DataSmart.projects.ppx.activity
{
    [Activity(Label = "Client Selector")]
    public class PpxClientSelectionActivity : ClientSelectorBase<PPClientSummary>
    {
        PpxClientSummaryAdapter _defaultAdapter = null;
        //List<PPClientSummary> _allPrepexClients;
        //PPClientSummary _selectedClient = null;
        //string NEXT_TYPE = string.Empty;

        protected override void doOnClick(PPClientSummary client)
        {
            Android.Widget.Toast.MakeText(this, client.Names, Android.Widget.ToastLength.Short).Show();
        }

        protected override List<PPClientSummary> getClientSummaries()
        {
            return new PpxLookupProvider().Get();
        }

        protected override void setClientSummaryAdapter(ListView view, List<PPClientSummary> clientSumaries)
        {
            _defaultAdapter = new PpxClientSummaryAdapter(this, view, clientSumaries);
            view.Adapter = _defaultAdapter;
        }

        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);
        //    SetContentView(Resource.Layout.clientlist);

        //    if (this.Intent.Extras != null && this.Intent.Extras.ContainsKey(Constants.KIND_PPX_NEXTVIEW))
        //    {
        //        //KIND_PP_NEXTVIEW
        //        NEXT_TYPE = this.Intent.Extras.GetString(Constants.KIND_PPX_NEXTVIEW);
        //    }

        //    var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
        //    listview.FastScrollEnabled = true;
        //    listview.FastScrollAlwaysVisible = true;

        //    listview.OnItemClickListener = this;

        //    _allPrepexClients = new PpxLookupProvider().Get();
        //    _defaultAdapter = new PpxClientSummaryAdapter(this, listview, _allPrepexClients);

        //    listview.Adapter = _defaultAdapter;

        //    //we hide the client summary options
        //    var rgroupCSOptions = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
        //    rgroupCSOptions.Visibility = ViewStates.Gone;

        //    var buttonPerformAction = FindViewById<Button>(Resource.Id.buttonPerformAction);
        //    buttonPerformAction.Text = "Use Selected Client";
        //    buttonPerformAction.Click += performActionSpecified;
        //}

        //void performActionSpecified(object sender, EventArgs e)
        //{
        //    if (_selectedClient == null)
        //    {
        //        Android.Widget.Toast.MakeText(this,
        //            "No clients selected. Please select a client.",
        //            Android.Widget.ToastLength.Long).Show();
        //        return;
        //    }

        //    var asString = Newtonsoft.Json.JsonConvert.SerializeObject(_selectedClient);
        //    var intent = new Intent().PutExtra(Constants.BUNDLE_SELECTEDCLIENT, asString);
        //    intent.PutExtra(Constants.KIND_PPX_NEXTVIEW, NEXT_TYPE);
        //    //intent.SetFlags(ActivityFlags.NewTask);
        //    SetResult(Result.Ok, intent);
        //    Finish();
        //}
    }
}