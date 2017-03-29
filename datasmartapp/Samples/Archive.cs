using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSmart.db;
using DataSmart.store;

namespace DataSmart
{
    public class Archive : Activity
    {
        public Dictionary<string, string> ApiAssets = null;
        string ProjectId = string.Empty;
        string DataStoreApplicationKey = string.Empty;
        Bundle BigBundle = null;
        const string ALL_VALUES = "allValues";
        string[] DATA_CONTROLs_ARRAY = new[] { "text3",
                "text4", "text1","text2","datePicker1","jumbo1"
            };
        
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (BigBundle != null)
            {
                outState.PutBundle(ALL_VALUES, BigBundle);
            }
        }
       
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //we initialise the app key for our data store
            //var a = Android.Content.Res.AssetManager.GetObject();
            ApiAssets = new Dictionary<string, string>();
            //we read the api key file

            var inputStream = Assets.Open(Constants.API_KEYFILE);
            var jsonObject = System.Json.JsonValue.Load(inputStream);

            ApiAssets[Constants.ASSET_NAME_APPNAME] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_NAME_APPNAME);
            ApiAssets[Constants.ASSET_PROJECT_ID] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_PROJECT_ID);
            ApiAssets[Constants.ASSET_NAME_SVC_ACCTEMAIL] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_NAME_SVC_ACCTEMAIL);
            ApiAssets[Constants.ASSET_DATASTORE_APPKEY] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_DATASTORE_APPKEY);
            ApiAssets[Constants.ASSET_P12KEYFILE] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_P12KEYFILE);

            ProjectId = ApiAssets[Constants.ASSET_PROJECT_ID];

            //ASSET_DATASTORE_APPKEY
            DataStoreApplicationKey = ApiAssets[Constants.ASSET_DATASTORE_APPKEY];

            if (BigBundle == null)
            {
                BigBundle = new Bundle();
            }

            if (bundle != null && bundle.ContainsKey(ALL_VALUES))
            {
                BigBundle.PutAll(bundle.GetBundle(ALL_VALUES));
                var resultsView = FindViewById<TextView>(Resource.Id.textAllValues);
                resultsView.Text = BigBundle.ToString();                
            }

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.SaveData);
            button.Click += buttonClicked;

            var button2 = FindViewById<Button>(Resource.Id.fetchData);
            button2.Click += getWebResource;

            //var showLoginButton = FindViewById<Button>(Resource.Id.showLoginForm);
            //showLoginButton.Click += LoginButton_Click;

            //var loginFormButton = FindViewById<Button>(Resource.Id.buttonLoginIn);            
            //loginFormButton.Click += doLoginIn_Click;
        }

        private void doLoginIn_Click(object sender, EventArgs e)
        {

        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.UserLoginLayout);
        }

        private void doGetData(object sender, EventArgs e)
        {
            //getWebResource();
        }

        private void buttonClicked(object sender, EventArgs e)
        {
            //new got(Assets) { AplicationKey = "" }.unwrapAriaStark(ApiAssets);

            new got(Assets) { AplicationKey = "" }.trainAriaStark(ApiAssets);

            return;
            var bundle = new Bundle();

            var asButton = sender as Button;
            if (asButton == null)
                return;

            var parent = asButton.Parent as ViewGroup;

            var jsonObject = new JsonObject();
            foreach (var controlName in DATA_CONTROLs_ARRAY)
            {
                var controlId = Resources.GetIdentifier(controlName, "id", PackageName);
                if (controlId == 0)
                    continue;

                var view = FindViewById(controlId);
                var viewType = view.GetType();

                if (viewType == typeof(EditText))
                {
                    //inputType
                    var asSpecificType = view as EditText;
                    bundle.PutString(controlName, asSpecificType.Text);
                }
                else if (viewType == typeof(DatePicker))
                {
                    var asSpecificType = view as DatePicker;
                    bundle.PutLong(controlName, asSpecificType.DateTime.ToBinary());
                }
                //else if (viewType == typeof(DatePicker))
                //{

                //}
                else
                {
                    ///we skip for now
                }
            }

            //we save to the local database
            var id = Guid.NewGuid().ToString("N");
            var myKey = LocalEntityStore.Instance.Save( new KindKey(id), new KindName(Constants.KIND_DEFAULT), new KindItem("I've been Saved"));

            //we clear the ui
            resetUi();

            updateFilterList();

            //BigBundle.Clear();
            //BigBundle.PutAll(bundle);

            //var resultsView = FindViewById<TextView>(Resource.Id.textAllValues);
            //resultsView.Text = bundle.ToString();
        }

        private void updateFilterList()
        {
            //we get the keys
            var records = LocalEntityStore.Instance.GetKeys(new KindName("default"));

            //and show them in the next grid
            var joined = string.Join("\n", records);
            var resultsView = FindViewById<TextView>(Resource.Id.textAllValues);
            resultsView.Text = joined;
        }

        private void resetUi()
        {
            foreach (var controlName in DATA_CONTROLs_ARRAY)
            {
                var controlId = Resources.GetIdentifier(controlName, "id", PackageName);
                if (controlId == 0)
                    continue;

                var view = FindViewById(controlId);
                var viewType = view.GetType();

                if (viewType == typeof(EditText))
                {
                    var asSpecificType = view as EditText;
                    asSpecificType.Text = "";
                }
                else if (viewType == typeof(DatePicker))
                {
                    var asSpecificType = view as DatePicker;
                    asSpecificType.DateTime = DateTime.MinValue;
                }
            }
        }

        public async void getWebResource(object sender, EventArgs e)
        {            
            ////http://www.w3schools.com/json/tryit.asp?filename=tryjson_http&url=myTutorials.txt
            //string url = "http://www.w3schools.com/json/tryit.asp?filename=tryjson_http&url=myTutorials.txt";
            //JsonValue json = await FetchDataAsync(url);
            //// ParseAndDisplay (json);

            //var resultsView = FindViewById<TextView>(Resource.Id.textAllValues);
            //resultsView.Text = json.ToString();
        }

        private async Task<JsonValue> FetchDataAsync(string url)
        {
            // Create an HTTP web request using the URL:
            var url1 = new Uri(url);
            var request = System.Net.WebRequest.Create(url1);
            request.ContentType = "application/json";
            request.Method = "GET";

            string result = string.Empty;
            try
            {
                using (var response = await request.GetResponseAsync())
                using (var stream = response.GetResponseStream())
                {
                    var jsonDoc = await Task.Run(() => new System.IO.StreamReader(stream).ReadToEndAsync());
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                    // Return the JSON document:
                    result = jsonDoc;
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                result = Resource.String.DEFAULT_ERRORCODE.ToString();
            }
            catch
            {
                result = Resource.String.DEFAULT_ERRORCODE.ToString();
            }
            finally
            {

            }
            return result;
        }
    }
}

