using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;

namespace DataSmart.modules
{
    public class RecordSummaryAdapter : BaseAdapter<VisitSummary>
    {
        List<VisitSummary> _myList;
        Activity _context;
        public RecordSummaryAdapter(Activity context, ListView listview, List<VisitSummary> clientRecords)
        {
            _context = context;
            _myList = clientRecords;
        }

        public override VisitSummary this[int position]
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
            return _myList[position].Wrapped.getItemId();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var record = _myList[position];
            var myView = convertView ??
                 _context.LayoutInflater.Inflate(Resource.Layout.recordsummaryview, parent, false);
            var labelText = string.Format(
                "{0} - {1}", record.NiceKindName, record.Wrapped.VisitDate.ToShortDateString()
                );
            var view = myView.FindViewById<TextView>(Resource.Id.recordVisitType);
            view.Text = labelText;
            return myView;
        }
    }
}