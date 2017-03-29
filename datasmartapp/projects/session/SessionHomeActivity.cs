using System.Collections.Generic;
using Android.App;
using Android.Widget;

namespace DataSmart.projects.session
{
    [Activity(Label = "Site Session Manager", Icon = "@drawable/jhpiego_logo")]
    public class SessionHomeActivity : BaseHomeActivity<SiteSession>
    {
        protected override void showDefaultHome()
        {
            showPPXHome();
        }

        protected override List<BaseWorkflowController> getActivityWFControllers()
        {
            return new List<BaseWorkflowController>();
        }

        void showPPXHome()
        {
            SetContentView(Resource.Layout.PrepexHome);
            var closeButton = FindViewById<Button>(Resource.Id.buttonClose);
            closeButton.Click += (sender, e) => {
                //close activity
                StartActivity(typeof(LauncherActivity));
            };

            var buttonClientEvaluation = FindViewById<Button>(Resource.Id.buttonClientEvaluation);
            buttonClientEvaluation.Click += (sender, e) =>
            {
                StartActivity(typeof(activity.SessionEditor));
            };
            buttonClientEvaluation.Text = "Manage Sessions";

            var buttonUnscheduled = FindViewById<Button>(Resource.Id.buttonUnscheduled);
            buttonUnscheduled.Visibility = Android.Views.ViewStates.Gone;

            var buttonDeviceRemovalVisit = FindViewById<Button>(Resource.Id.buttonDeviceRemovalVisit);
            buttonDeviceRemovalVisit.Text = "Manage Providers";
            buttonDeviceRemovalVisit.Click += (sender, e) =>
            {
                StartActivity(typeof(activity.ProviderGridDisplayActivity));
            };

            var buttonPostRemovalVisit = FindViewById<Button>(Resource.Id.buttonPostRemovalVisit);
            buttonPostRemovalVisit.Visibility = Android.Views.ViewStates.Gone;

            //buttonViewList
            var buttonViewList = FindViewById<Button>(Resource.Id.buttonViewList);
            buttonViewList.Visibility = Android.Views.ViewStates.Gone;

            //buttonEditRecords
            var buttonEditRecords = FindViewById<Button>(Resource.Id.buttonEditRecords);
            buttonEditRecords.Click += (sender, e) =>
            {
                sendToast("No action defined", ToastLength.Long);
            };

            var buttonSupplies = FindViewById<Button>(Resource.Id.buttonSupplies);
            buttonSupplies.Visibility = Android.Views.ViewStates.Gone;

            var buttonViewRecordSummaries = FindViewById<Button>(Resource.Id.buttonViewRecordSummaries);
            buttonViewRecordSummaries.Click += async (sender, e) =>
            {
                //await getClientSummaryReport(new VmmcLookupProvider(), Constants.VMMC_KIND_DISPLAYNAMES);
                sendToast("No action defined", ToastLength.Long);
            };
        }
    }
}