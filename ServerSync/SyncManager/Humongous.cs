using Android.App;
using Android.Content.Res;
using JhpDataSystem.model;
using JhpDataSystem.projects;
using JhpDataSystem.store;
using Newtonsoft.Json.Linq;
using SyncManager.store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JhpDataSystem
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
                        AppVersion = "1.0"
                    }
                        ;
                }
                return _instance;
            }
        }

        AssetManager _assetManager { get; set; }
        Activity _mainContext;

        public Dictionary<int, List<FieldValuePair>> TemporalViewData = null;

        public void updateFieldDictionary()
        {
            var currentContexts = new List<BaseContextManager>();
            var allContexts = new List<BaseContextManager>() {
                //new LspContextManager(null,null),
                new VmmcContextManager(null,null),
                new PpxContextManager(null,null)
            };

            foreach (var projContext in allContexts)
            {
                if (projContext.FieldItems != null)
                { currentContexts.Add(projContext); }
            }

            foreach (var currCtxt in currentContexts)
            {
                var views = currCtxt.FieldItems.Select(t => t.pageName).Distinct().ToList();
                foreach (var kind in currCtxt.KindDisplayNames.Keys)
                {
                    var fieldDictionaryStore = new
                        FieldDictionaryStore()
                    { DatasetName = kind };
                    var kindSearchName = kind;
                    if (currCtxt.KindToFieldTablename != null
                        && currCtxt.KindToFieldTablename.ContainsKey(kind))
                    {
                        kindSearchName = currCtxt.KindToFieldTablename[kind];
                    }
                    fieldDictionaryStore.getFields(currCtxt.Name,
                        currCtxt.FieldItems.Where(t => t.pageName.Contains(kindSearchName)).ToList());
                    fieldDictionaryStore.saveToDb();
                }
            }
        }

        public Dictionary<string, string> ApiAssets = null;
        public void InitialiseAppResources(AssetManager assetManager, Activity context)
        {
            //ContextManager = null;
            ModuleContext = null;
            _assetManager = assetManager;
            _mainContext = context;
            TemporalViewData = new Dictionary<int, List<FieldValuePair>>();
            ApiAssets = new Dictionary<string, string>();
            //we read the api key file
            var bytes = SyncManager.Properties.Resources.api_keys;
            var inputStream = new MemoryStream(bytes).toText();

            //var keys = new Dictionary<string, string>();
            var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject(inputStream)
                as Newtonsoft.Json.Linq.JContainer;
            var tokens = jObject.ToList<JToken>();
            foreach (var token in tokens)
            {
                var name = token.First();
                var assetName = name.Path;
                var assetValue = name.Value<string>();

                if (Constants.ASSET_LIST.Contains(assetName.ToLowerInvariant()))
                {
                    ApiAssets[assetName] = assetValue;
                }                
            }

            var saveDictionaryToDb = false;
            if (saveDictionaryToDb)
            {
                updateFieldDictionary();
            }

            CloudDbInstance = new CloudDb() { ApiAssets = ApiAssets,  };

            //var allKindNames = new List<string>();
            //foreach (var ctxt in currentContexts)
            //    foreach (var kind in ctxt.KindDisplayNames.Keys)
            //        allKindNames.Add(kind);

            //this creates a table used to store a decrypted set of similar data
            var dropAndRecreate = false;
            var allTables = CloudDb.getAllKindNames();
            foreach (var table in allTables)
            {
                new CloudLocalStore(table.toKind()).build();
                new CloudLocalStore(CloudDb.getLocalTableName(table).toKind())
                    .build(dropAndRecreate);
                new FieldValueStore(CloudDb.getTableFieldValueName(table))
                    .build(dropAndRecreate);
            }
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