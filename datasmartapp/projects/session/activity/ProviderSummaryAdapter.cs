using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace DataSmart.projects.session
{
    public class ProviderSummaryAdapter : BaseAdapter<SiteProvider>
    {
        List<SiteProvider> _myList;
        Activity _context;
        public int tMinus { get; set; }
        public ProviderSummaryAdapter(Activity context, ListView listview, List<SiteProvider> clientList)
        {
            tMinus = -1;
            _context = context;
            _myList = clientList;
        }

        public override SiteProvider this[int position]
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
            //var placementDate = client.SessionDate;
            //var daysElapsed = DateTime.Now.Subtract(placementDate).TotalDays;
            myView.FindViewById<TextView>(Resource.Id.clientSummaryTDate)
                .Text = Convert.ToString("FirstName: " + client.FirstName);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryNames)
                .Text = Convert.ToString("MaidenName: " + client.MaidenName);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryCardSerial)
                .Text = Convert.ToString("SurName: " + client.SurName);
            //.Text = "Card Id: " + Convert.ToString(client.FormSerial);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryMCNumber)
                .Text = Convert.ToString("NRC: " + client.NRC);
            //.Text = "MC #: " + Convert.ToString(client.MCNumber);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryPlacementDate)
                .Text = Convert.ToString("Facility: " + client.ContactNumber);
            //.Text = client.MCDate.ToString("d MMM, yyyy", CultureInfo.InvariantCulture);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryTelephone)
                .Text = Convert.ToString("Cell: " + client.ContactNumber);
            //.Text = Convert.ToString(client.ClientTel);
            return myView;
        }
    }
}