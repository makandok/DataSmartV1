using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;
using DataSmart.store;
using DataSmart.Utilities;
using System.Threading.Tasks;

namespace DataSmart.projects.ppx
{
    [Activity(Label = "Client List")]
    public class PpxFilteredGridDisplayActivity : Activity, ListView.IOnItemClickListener
    {
        PpxClientSummaryAdapter _defaultAdapter = null;
        List<PPClientSummary> _allPrepexClients;
        PPClientSummary _selectedClient = null;
        List<int> _listOptions = null;
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            _selectedClient = _allPrepexClients[position];
            Android.Widget.Toast.MakeText(this, _selectedClient.Names, Android.Widget.ToastLength.Short).Show();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientlist);

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;

            listview.OnItemClickListener = this;

            _allPrepexClients = new PpxLookupProvider().Get();
            _defaultAdapter = new PpxClientSummaryAdapter(this, listview, _allPrepexClients);

            listview.Adapter = _defaultAdapter;

            //we bind the events for the action buttons on top
            _listOptions = new List<int>()
            {
                Resource.Id.buttonCSummmCall3,Resource.Id.buttonCSummSms6,Resource.Id.buttonCSummSmsOrCall7,
                Resource.Id.buttonCSummCall14,Resource.Id.buttonCSummCall49,Resource.Id.buttonCSummCall56
            };

            var rgroup = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            rgroup.CheckedChange += applyFilter;

            //buttonPerformAction
            var buttonPerformAction = FindViewById<Button>(Resource.Id.buttonPerformAction);
            buttonPerformAction.Click += performActionSpecified;
        }

        //void startClientRecordEdit(object sender, EventArgs e)
        //{
        //    //get selected client
        //    if (_selectedClient == null)
        //    {
        //        Toast.MakeText(this, Resource.String.ppx_noclientselected, Android.Widget.ToastLength.Long).Show();
        //        return;
        //    }
        //    //get list of forms for this client
        //    //allow user to pick the form to edit
        //    //show editor
        //}

            void performActionSpecified(object sender, EventArgs e)
        {
            var rgroup = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            if (rgroup != null)
            {
                //we get the checked item
                var checkedButtonId = rgroup.CheckedRadioButtonId;
                switch (checkedButtonId)
                {
                    //case Resource.Id.buttonCSummmAll:
                    case Resource.Id.buttonCSummSms6:
                        //we send an SMS after confirmation
                        var clients = getClients4TMinus(6);
                        if (clients.Count == 0)
                        {
                            Toast.MakeText(this, Resources.GetString(Resource.String.confirm_action),
                                ToastLength.Long).Show();
                            return;
                        }

                        //get check if we havent already sent an SMS
                        //we use the placement date to track this cohort
                        //if so, we alert and return or ask user to still send
                        var alreadySmsd = clients.Any(t => t.Day6SmsReminderDate != DateTime.MinValue);
                        if (alreadySmsd)
                        {
                            new AlertDialog.Builder(this)
                                .SetTitle(Resources.GetString(Resource.String.confirm_action))
                                .SetMessage(string.Format(
                                Resources.GetString(Resource.String.sms_clientsalreadysmsd),
                                clients.Count, clients.Count == 1 ? "" : "s"))
                                .SetPositiveButton("OK", async (senderAlert, args) =>
                                {
                                    showSendSmsDialog(clients);
                                })
                                .SetNegativeButton("Cancel", (senderAlert, args) => { })
                                .Create()
                                .Show();
                        }
                        else
                        {
                            //we prompt
                            showSendSmsDialog(clients);
                        }

                        break;

                    case Resource.Id.buttonCSummmCall3:
                    case Resource.Id.buttonCSummSmsOrCall7:
                    case Resource.Id.buttonCSummCall14:
                    case Resource.Id.buttonCSummCall49:
                    case Resource.Id.buttonCSummCall56:
                        Toast.MakeText(this,
                            Resources.GetString(Resource.String.remembertocall),
                            //"Please remember to call the clients indicated here", 
                            ToastLength.Long).Show();
                        break;
                    default:
                        //do nothing
                        Toast.MakeText(this, "Nothing to do", ToastLength.Long).Show();
                        break;
                }
            }
        }

        void showSendSmsDialog(List<PPClientSummary> clients)
        {
            new AlertDialog.Builder(this)
                .SetTitle(Resources.GetString(Resource.String.confirm_action))
                .SetMessage(string.Format(
                Resources.GetString(Resource.String.sms_confirmation),
                clients.Count, clients.Count == 1 ? "" : "s"))
                .SetPositiveButton("OK", async (senderAlert, args) =>
                {
                    await sendSms(clients);
                    await updateDay6DateAndSave(clients, DateTime.Now);
                })
                .SetNegativeButton("Cancel", (senderAlert, args) => { })
                .Create()
                .Show();
        }

        //saveClientSummary
        async Task<bool> updateDay6DateAndSave(List<PPClientSummary> clients, DateTime dateSmsd)
        {
            clients.ForEach(t => t.Day6SmsReminderDate = dateSmsd);
            new LocalDB3().DB.UpdateAll(clients);
            return true;
        }

        async Task<bool> sendSms(List<PPClientSummary> clients)
        {
            //we send           
            Toast.MakeText(this, "Please wait, Started sending messages", ToastLength.Short).Show();
            var bulkSender = new BulkSmsSender()
            {
                contactNumbers = clients,
                CurrentContext = this,
                formattedText = Resources.GetString(Resource.String.sms_msgwithname)
            };
            await bulkSender.Send();
            Toast.MakeText(this, "Completed sending messages", ToastLength.Short).Show();

            //save to db list of clients send

            return true;
        }

        void applyFilter(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            //rgroupCSOptions
            var rgroup = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            if (rgroup != null)
            {
                //we get the checked item
                var checkedButtonId = rgroup.CheckedRadioButtonId;
                switch (checkedButtonId)
                {
                    case Resource.Id.buttonCSummmCall3:
                        day3Call();break;
                    case Resource.Id.buttonCSummSms6:
                        day6Sms(); break;
                    case Resource.Id.buttonCSummSmsOrCall7:
                        day7NoShowCall(); break;
                    case Resource.Id.buttonCSummCall14:
                        day14Call(); break;
                    case Resource.Id.buttonCSummCall49:
                        day49Call(); break;
                    case Resource.Id.buttonCSummCall56:
                        day56Call(); break;
                    default:
                        //buttonCSummmAll
                        showClients4TMinus(-1);
                        break;
                }
            }
        }

        void day3Call()
        {
            //todo: complete day3call
            showClients4TMinus(3);
        }

        void showClients4TMinus(int daysPast)
        {
            //textCSummOptionsLabel   
            var textCSummOptionsLabel = FindViewById<TextView>(Resource.Id.textCSummOptionsLabel);
            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);

            //if someone clicks the button again, we show the full list
            var currentAdapter = listview.Adapter as PpxClientSummaryAdapter;
            if (daysPast == -1)
            {
                if (textCSummOptionsLabel != null)
                    textCSummOptionsLabel.Text = "All Clients";

                //Android.Widget.Toast.MakeText(this, "Showing all clients", Android.Widget.ToastLength.Short).Show();
                listview.Adapter = _defaultAdapter;
                return;
            }

            //else we filter based on choice
            var tMinus = DateTime.Now.Subtract(new TimeSpan(daysPast, 0, 0, 0));

            if (textCSummOptionsLabel != null)
                textCSummOptionsLabel.Text = "Clients for " + tMinus.ToShortDateString();

            //Android.Widget.Toast.MakeText(this, "Showing clients for " + tMinus.ToShortDateString(), Android.Widget.ToastLength.Short).Show();
            listview.Adapter = new PpxClientSummaryAdapter(this,
                listview
                , getClients4TMinus(daysPast)
                )
            { tMinus = daysPast };

            //we show the fab
            //var fab = new FloatingActionButton(this, );
        }

        List<PPClientSummary> getClients4TMinus(int daysPast)
        {
            var tMinus = DateTime.Now.Subtract(new TimeSpan(daysPast, 0, 0, 0));
            return _allPrepexClients.Where(t =>
                t.PlacementDate.Day == tMinus.Day &&
                t.PlacementDate.Month == tMinus.Month &&
                t.PlacementDate.Year == tMinus.Year
                ).ToList();
        }

        void day6Sms()
        {
            showClients4TMinus(6);
        }

        void day7NoShowCall()
        {
            showClients4TMinus(7);
        }

        void day14Call()
        {
            showClients4TMinus(14);
        }

        void day49Call()
        {
            showClients4TMinus(49);
        }

        void day56Call()
        {
            showClients4TMinus(56);
        }

        void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = _allPrepexClients[position];
            Android.Widget.Toast.MakeText(this, t.Names, Android.Widget.ToastLength.Short).Show();
        }        
    }
}