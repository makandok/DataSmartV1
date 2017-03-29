using Android.App;
using System;

namespace JhpDataSystem
{
    public class WaitDialogHelper
    {
        Activity _context;
        Action<string, Android.Widget.ToastLength> _makeToast;
        public WaitDialogHelper(Activity context, Action<string, Android.Widget.ToastLength> makeToast)
        {
            _context = context;
            _makeToast = makeToast;
        }

        //ProgressDialog getDialog()
        //{
        //    var progress = new ProgressDialog(_context)
        //    {
        //        Indeterminate = true
        //    };
        //    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
        //    progress.SetTitle("Please wait...");
        //    progress.SetMessage("Synchronising with server");
        //    progress.SetCancelable(true);
        //    return progress;
        //}

        public async System.Threading.Tasks.Task<int> showDialog(
            Func<Action<string, Android.Widget.ToastLength>, System.Threading.Tasks.Task<int>> worker)
        {
            var progressDialog = ProgressDialog.Show(_context, "Please wait...", "Synchronising with server", true);
            await worker(_makeToast);
            progressDialog.Hide();
            //new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            //{
            ////_context.RunOnUiThread(
            ////        () => worker(_makeToast)

            ////        //Android.Widget.Toast.MakeText(_context, 
            ////        //"Toast within progress dialog.", Android.Widget.ToastLength.Short).Show()

            ////        );
            //    _context.RunOnUiThread(
            //        () => progressDialog.Hide());
            //})).Start();

            return 0;
        }
    }
}