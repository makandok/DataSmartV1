using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Widget;
using System.Json;
using System.Net;
using System.Threading.Tasks;

namespace DataSmart.Utilities
{
    public class BulkSmsSender
    {
        //public List<ContactNumber> contactNumbers { get; set; }
        public List<projects.ppx.PPClientSummary> contactNumbers { get; set; }

        public string formattedText { get; set; }

        public Context CurrentContext { get; set; }

        public async Task<bool> Send()
        {
            foreach (var client in contactNumbers)
            {
                //client.Telephone = "0964260027";
                await Task.Run(() =>
                {
                    try
                    {
                        new SmsSender()
                        {
                            message = string.Format(formattedText, client.Names),
                            phoneNumber = client.Telephone
                        }.Send();
                        client.Day6SmsReminderDate = DateTime.Now;
                    }
                    catch (Java.Lang.IllegalArgumentException argex)
                    {
                        //means the number is not valid
                        //we prompt, log or fail silent
                        AppInstance.Instance.LogActionItem(string.Format("Error sending SMS. Phone number {0} for {1} is invalid", client.Telephone, client.Names));
                    }
                    catch (Exception ex)
                    {

                    }
                });

                await Task.Delay(TimeSpan.FromMilliseconds(2000));
            }
            return true;
        }
    }

    //https://developer.xamarin.com/recipes/android/networking/
    internal class SmsSender
    {
        public string message { get; set; }
        public string phoneNumber { get; set; }
        public Activity appContext { get; set; }
        public void Send()
        {
            var smsHelper = Android.Telephony.SmsManager.Default;
            //pendingIntent
            smsHelper.SendTextMessage(phoneNumber, null, message, null, null);
        }
    }
    
    internal class EmailSender
    {
        public string message { get; set; }
        public string messageSubject { get; set; }
        public List<string> receipients { get; set; }
        public Activity appContext { get; set; }
        public Android.Net.Uri attachment { get; set; }
        public void Send()
        {
            var email = new Intent(Android.Content.Intent.ActionSend)
            .SetType("message/rfc822")
            //.PutExtra(Intent.ExtraCc, new string[] { an_email })
            .PutExtra(Intent.ExtraEmail, receipients.ToArray())
            .PutExtra(Android.Content.Intent.ExtraSubject, messageSubject)
            .PutExtra(Android.Content.Intent.ExtraText, message);
            if (attachment != null)
            {
                email.PutExtra(Intent.ExtraStream, attachment);
            }

            appContext.StartActivity(email);
            Toast.MakeText(appContext, "Message will be sent by your default email app", ToastLength.Short).Show();
        }
    }

    internal class RestServiceHelper
    {
        public string baseUrl { get; set; }

        public string urlParams { get; set; }
        async void Send(Action<string> callBack)
        {
            var jsonResult = await getResource(baseUrl + urlParams);
            if (callBack != null)
            {
                callBack(jsonResult);
            }
        }

        //https://developer.xamarin.com/recipes/android/web_services/consuming_services/call_a_rest_web_service/
        async Task<JsonValue> getResource(string resourceUrl)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(resourceUrl));
            request.ContentType = "application/json";
            request.Method = "GET";
            using (var response = await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            {
                return await Task.Run(() => JsonObject.Load(stream));
            }
        }
    }
}