using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;
using System.Globalization;

namespace DataSmart.projects.vmc
{
    public class VmmcClientSummaryAdapter : BaseAdapter<VmmcClientSummary>
    {
        List<VmmcClientSummary> _myList;
        Activity _context;
        public int tMinus { get; set; }
        public VmmcClientSummaryAdapter(Activity context, ListView listview, List<VmmcClientSummary> clientList)
        {
            tMinus = -1;
            _context = context;
            _myList = clientList;
        }

        public override VmmcClientSummary this[int position]
        {
            get
            {
                return _myList[position];
            }
        }

        public override int Count
        {
            get
            {
                return _myList.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return _myList[position].getItemId();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var client = _myList[position];
            var myView = convertView ??
                 _context.LayoutInflater.Inflate(Resource.Layout.clientsummary, parent, false);
            //clientSummaryTDate
            var placementDate = client.MCDate;
            var daysElapsed = DateTime.Now.Subtract(placementDate).TotalDays;
            myView.FindViewById<TextView>(Resource.Id.clientSummaryTDate)
                .Text = Convert.ToString("Day " + Math.Floor(daysElapsed));
            myView.FindViewById<TextView>(Resource.Id.clientSummaryNames)
                .Text = Convert.ToString(client.FirstName + " " + client.LastName);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryCardSerial)
                .Text = "Card Id: " + Convert.ToString(client.FormSerial);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryMCNumber)
                .Text = "MC #: " + Convert.ToString(client.MCNumber);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryPlacementDate)
                .Text = client.MCDate.ToString("d MMM, yyyy", CultureInfo.InvariantCulture);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryTelephone)
                .Text = Convert.ToString(client.ClientTel);
            return myView;
        }
    }
}