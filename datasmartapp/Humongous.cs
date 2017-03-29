using Android.App;
using Android.Content;
using Android.Content.Res;
using JhpDataSystem.model;
using DataSmart.store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JhpDataSystem;

namespace DataSmart
{
    public class AppInstance
    {
        static AppInstance _instance;
        public static AppInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppInstance()
                    {
                        AppVersion = "1.2"
                    };                        
                }
                return _instance;
            }
        }

        AssetManager _assetManager { get; set; }
        Activity _mainContext;
        public LocalEntityStore LocalEntityStoreInstance { get; private set; }

        public Dictionary<int, List<FieldValuePair>> TemporalViewData = null;

        Android.Net.Uri writeErrorLog(Exception exception)
        {
            //https://developer.xamarin.com/api/property/Android.OS.Environment.ExternalStorageDirectory/
            var filename = "a"+Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt";
            var downloadsFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            var defaultDirectory = Path.Combine(downloadsFolder.Path, "jhp");
            if (!Directory.Exists(defaultDirectory))
            {
                //System.Security.AccessControl.DirectorySecurity.
                //var f = new System.Security.AccessControl.DirectorySecurity() {                    
                //};f.
                Directory.CreateDirectory(defaultDirectory);                
            }
            var filePath = Path.Combine(downloadsFolder.Path,"jhp", filename);
            using (var streamWriter = File.CreateText(filePath))
            {
                streamWriter.Write(exception.Message ?? "No exception message");
                streamWriter.WriteLine(exception.StackTrace ?? "No Stacktrace");
            }

            var file = new Java.IO.File(filePath);
            file.SetReadable(true, false);
            return Android.Net.Uri.FromFile(file);
        }

        public Dictionary<string, string> ApiAssets = null;
        protected void uncaughtExceptionHandler(object sender, 
            //Exception exception
            UnhandledExceptionEventArgs args
            )
        {
            var exception = args.ExceptionObject as Exception;
            if (exception == null)
                return;

            Android.Widget.Toast.MakeText(_mainContext, 
                "An error occurred. "+ (exception.Message ?? "No exception message"), 
                Android.Widget.ToastLength.Long).Show();

            //can't do Log.Debug as process is being destroyed
            //we do our own
            var logUri = writeErrorLog(exception);
            Android.Widget.Toast.MakeText(_mainContext, "Preparing to send email with error", Android.Widget.ToastLength.Long).Show();
            new Utilities.EmailSender()
            {
                appContext = _mainContext,
                receipients = new List<string>() {
                            "makando.kabila@jhpiego.org"},
                messageSubject = "Crash Report",
                attachment= logUri,
                message = "See attached log"
            }.Send();
            if (!args.IsTerminating)
            {
                _mainContext.FinishAffinity();
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }
        }

        public void InitialiseAppResources(AssetManager assetManager, Activity context)
        {
            AppDomain.CurrentDomain.UnhandledException += uncaughtExceptionHandler;
            //ContextManager = null;
            ModuleContext = null;
            _assetManager = assetManager;
            _mainContext = context;
            TemporalViewData = new Dictionary<int, List<FieldValuePair>>();
            ApiAssets = new Dictionary<string, string>();
            //we read the api key file
            var inputStream = assetManager.Open(Constants.API_KEYFILE);
            var jsonObject = System.Json.JsonValue.Load(inputStream);

            foreach (var assetName in Constants.ASSET_LIST)
            {
                ApiAssets[assetName] =
                    jsonObject.decryptAndGetApiSetting(assetName);
            }

            //we need to have this class initialised
            LocalEntityStoreInstance = new LocalEntityStore();
            LocalEntityStoreInstance.buildTables(false);
            
            CloudDbInstance = new CloudDb(_assetManager);

            //Android.OS.Build.Serial
            var configuration = new LocalDB3().DB.Table<DeviceConfiguration>().FirstOrDefault();
            Configuration = configuration;
        }

        public projects.ContextLocalEntityStore ModuleContext { get; set; }

        internal void SetProjectContext(projects.BaseContextManager ctxt)
        {
            ModuleContext = new projects.ContextLocalEntityStore(ctxt);
        }

        List<FieldItem> readFields(string fieldsAssetName, AssetManager assetManager, Activity context)
        {
            //we load the fields
            var fieldStream = assetManager.Open(fieldsAssetName);

            var asString = fieldStream.toText();

            var fields = Newtonsoft.Json.JsonConvert.
                DeserializeObject<List<FieldItem>>(asString);

            var viewPages = fields.Select(t => t.pageName).Distinct().ToList();

            var dictionary = new Dictionary<string, int>();
            foreach (var page in viewPages)
            {
                var id = context.Resources.GetIdentifier(page, "layout", context.PackageName);
                if (id == 0) throw new ArgumentOutOfRangeException("Could not determine Id for layout " + page);
                dictionary[page] = id;
            }

            foreach (var field in fields)
            {
                field.PageId = dictionary[field.pageName];
            }

            return fields;
        }

        internal void LogActionItem(string v)
        {
            //todo: implement LogActionItem in AppInstance
            //throw new NotImplementedException();
        }

        public UserSession CurrentUser { get; internal set; }

        internal void SetTempDataForView(int viewId, List<FieldValuePair> valueFields)
        {
            throw new NotImplementedException();
        }

        public CloudDb CloudDbInstance
        {
            get;private set;
        }
        public DeviceConfiguration Configuration { get; internal set; }
        public string AppVersion { get; internal set; }
    }
}