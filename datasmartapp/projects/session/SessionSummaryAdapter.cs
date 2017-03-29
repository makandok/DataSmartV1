using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace DataSmart.projects.session
{
    public class SessionSummaryAdapter : BaseAdapter<SiteSession>
    {
        List<SiteSession> _myList;
        Activity _context;
        public int tMinus { get; set; }
        public SessionSummaryAdapter(Activity context, ListView listview, List<SiteSession> clientList)
        {
            tMinus = -1;
            _context = context;
            _myList = clientList;
        }

        public override SiteSession this[int position]
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
            var placementDate = client.SessionDate;
            var daysElapsed = DateTime.Now.Subtract(placementDate).TotalDays;
            myView.FindViewById<TextView>(Resource.Id.clientSummaryTDate)
                .Text = Convert.ToString("Days: " + Math.Floor(daysElapsed));
            myView.FindViewById<TextView>(Resource.Id.clientSummaryNames)
                .Text = Convert.ToString(client.FacilityId);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryCardSerial)
                .Text ="Tables: "+ (client.Tables == null ? 0 : client.Tables.Count).ToString();
                //.Text = "Card Id: " + Convert.ToString(client.FormSerial);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryMCNumber)
                .Visibility = ViewStates.Invisible;
                //.Text = "MC #: " + Convert.ToString(client.MCNumber);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryPlacementDate)
                .Visibility = ViewStates.Invisible;
                //.Text = client.MCDate.ToString("d MMM, yyyy", CultureInfo.InvariantCulture);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryTelephone)
                .Visibility = ViewStates.Invisible;
                //.Text = Convert.ToString(client.ClientTel);
            return myView;
        }
    }
}