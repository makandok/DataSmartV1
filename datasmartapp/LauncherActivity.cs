using System;
using Android.App;
using Android.Widget;
using Android.OS;
using DataSmart.projects;

namespace DataSmart
{
    [Activity(Label = "Available Functionality", Icon = "@drawable/jhpiego_logo")]
    public class LauncherActivity : Activity
    {
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            showHomePage();
        }

        void showHomePage()
        {
            SetContentView(Resource.Layout.Main);
            //buttonCloseApp
            var buttonCloseApp = FindViewById<Button>(Resource.Id.buttonCloseApp);
            buttonCloseApp.Click += (x, y) =>
            {
                this.FinishAffinity();
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());                
            };

            var loggedInUserText = FindViewById<Button>(Resource.Id.tLoggedInUser);
            if (AppInstance.Instance.CurrentUser != null)
            {
                var user = AppInstance.Instance.CurrentUser;
                loggedInUserText.Text = string.Format(user.User.Names + " ({0})", user.User.UserId);
            }
            else
            {
                //we show the login form
                doLogOut();
            }
            loggedInUserText.Click += showMenuLoggedInUser;

            var buttonPrepexHome = FindViewById<Button>(Resource.Id.buttonPrepexHome);
            buttonPrepexHome.Click += (x, y) =>
            {
                var contextManager = new PpxContextManager(this.Assets, this);
                AppInstance.Instance.SetProjectContext(contextManager);
                StartActivity(typeof(projects.ppx.PPXHomeActivity));
            };

            var buttonVmmcHome = FindViewById<Button>(Resource.Id.buttonVmmcHome);
            buttonVmmcHome.Click += (x, y) => 
            {
                var contextManager = new VmmcContextManager(this.Assets, this);
                AppInstance.Instance.SetProjectContext(contextManager);
                StartActivity(typeof(projects.vmc.VmmcHomeActivity));
            };
            //fetchData
            var fetchData = FindViewById<Button>(Resource.Id.fetchData);
            fetchData.Visibility = Android.Views.ViewStates.Invisible;


            var buttonSiteSession = FindViewById<Button>(Resource.Id.buttonSiteSession);
            buttonSiteSession.Click += (x, y) =>
            {
                StartActivity(typeof(projects.session.SessionHomeActivity));
            };
            buttonSiteSession.Visibility = Android.Views.ViewStates.Invisible;
        }

        void showDialog(string title, string message)
        {
            new Android.App.AlertDialog.Builder(this)
            .SetTitle(title)
            .SetMessage(message)
            .SetPositiveButton("OK", (senderAlert, args) => { })
            .Create()
            .Show();
        }

        private void showMenuLoggedInUser(object sender, EventArgs e)
        {
            new Android.App.AlertDialog.Builder(this)
                    .SetTitle("Confirm Action")
                    .SetMessage("Do you want to log out")
                    .SetPositiveButton("OK", (senderAlert, args) =>
                    {
                        doLogOut();
                    })
                    .SetNegativeButton("Cancel", (a, b) => { return; })
                    .Create()
                    .Show();
        }

        private void doLogOut()
        {
            AppInstance.Instance.CurrentUser = null;
            var tuser = FindViewById<Button>(Resource.Id.tLoggedInUser);
            if (tuser != null)
            { tuser.Text = "Not Logged In"; }
            //we show the log in screen
            showLoginForm();
        }

        void showLoginForm()
        {
            StartActivity(typeof(MainActivity));
        }
    }
}