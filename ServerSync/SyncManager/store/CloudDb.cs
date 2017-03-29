using System;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Datastore.v1;
using Google.Apis.Datastore.v1.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using JhpDataSystem.model;
using System.Net;
using SyncManager.store;
using SyncManager.model;

namespace JhpDataSystem.store
{
    public class FetchCloudDataWorker
    {
        RunQueryRequest query;
        ProjectsResource.RunQueryRequest res;

        DatastoreService _dataStore;
        string _projectId;
        KindName _kindName;
        ResultsLimit _skip;
        long _editDate;
        bool _fetchOldData;

        const string DATEADDED = "dateadded";
        const string EDITDATE = "editdate";

        public FetchCloudDataWorker(DatastoreService dataStore, string projectId, KindName kindName, ResultsLimit skip, long editDate, bool fetchOldData)
        {
            query = getQueryRequest1(skip, editDate, fetchOldData, kindName);
            res = dataStore.Projects.RunQuery(query
                , projectId);

            _dataStore = dataStore;
            _projectId = projectId;
            _kindName = kindName;
            _skip = skip;
            _editDate = editDate;
            _fetchOldData = fetchOldData;
        }

        private static async Task<RunQueryResponse> execQuery(ProjectsResource.RunQueryRequest res, int numAttempts = 4)
        {
            RunQueryResponse response = null;
            var fetched = false;
            for (int i = 0; i < numAttempts; i++)
            {
                try
                {
                    response = await res.ExecuteAsync();
                    fetched = true;
                }
                catch (Google.GoogleApiException gex)
                {
                    //todo: mark this record as bad to prevent it blocking for life
                    //cloudDb.InsertOrReplace(new OutEntityUnsynced().load(outEntity));
                    //cloudDb.Delete<OutEntity>(saveable.Id.Value);
                    //break;
                }
                catch (System.Net.WebException wex)
                {
                    //perhaps lost connection
                    //we alllow it to spin for now
                }
                catch (Exception ex)
                {
                    //ex.Message
                    //"A task was canceled."

                    //unknown exception
                }
                if (fetched)
                {
                    break;
                }
                else
                {
                    //lets add a 2 second delay in case it failed the first time
                    //lets log that we had to wait, try x
                    await Task.Delay(TimeSpan.FromMilliseconds(2000));
                }
            }
            return response;
        }

        public async Task<List<CloudEntity>> beginFetchCloudData()
        {
            var toReturn = new List<CloudEntity>();
            var response = await execQuery(res);
            if (response == null)
            {
                //means we failed to get something, so we quit and perhaps try later
                return null;
            }
            if (response.Batch.EntityResults == null)
            {
                //means we got something except its no results
                return toReturn;
            }

            toReturn.AddRange(toCloudEntity(response.Batch.EntityResults));
            while (response.Batch.EntityResults != null && response.Batch.MoreResults != "NO_MORE_RESULTS")
            {
                //means we have some more results
                //var newQuery = getQueryRequest(_skip, _editDate, _fetchOldData, _kindName);
                //newQuery.Query.StartCursor = response.Batch.EndCursor;
                query.Query.StartCursor = response.Batch.EndCursor;
                //res = _dataStore.Projects.RunQuery(newQuery, _projectId);
                
                response = await execQuery(res);
                toReturn.AddRange(toCloudEntity(response.Batch.EntityResults));
            }            
            return toReturn;
        }

        //private async Task<List<CloudEntity>> fetchCloudData1()
        //{
        //    var toReturn = new List<CloudEntity>();
        //    var repeatDownload = true;
        //    while (repeatDownload)
        //    {

        //        var response = await res.ExecuteAsync();
        //        var entityResults = response.Batch.EntityResults;
        //        //Debug.
        //        if (entityResults == null)
        //        {
        //            repeatDownload = false;
        //            break;
        //        }

        //        query.Query.StartCursor = response.Batch.EndCursor;
        //        foreach (var entityResult in response.Batch.EntityResults)
        //        {
        //            var entity = entityResult.Entity;
        //            var path = entity.Key.Path.FirstOrDefault();
        //            var cloudEntity = new CloudEntity()
        //            {
        //                FormName = path.Kind,
        //                Id = path.Name,
        //                EntityId = entity.Properties["entityid"].StringValue,
        //                DataBlob = entity.Properties["datablob"].StringValue,
        //                KindMetaData = entity.Properties["kindmetadata"].StringValue,
        //                //EditDate = Convert.ToInt64(
        //                //    entity.Properties["editdate"].IntegerValue),
        //                //EditDay = Convert.ToInt32(
        //                //    entity.Properties["editday"].IntegerValue)
        //            };
        //            if (entity.Properties.ContainsKey("editdate"))
        //            {
        //                var editDate = entity.Properties["editdate"].IntegerValue;
        //                cloudEntity.EditDate = Convert.ToInt64(editDate);

        //                var editDay = entity.Properties["editday"].IntegerValue;
        //                cloudEntity.EditDay = Convert.ToInt32(editDay);
        //            }
        //            else
        //            {
        //                //use field date added 
        //                var entityDate = Convert.ToDateTime(
        //                    entity.Properties["dateadded"].TimestampValue);
        //                var editday = entityDate.toYMDInt();
        //                cloudEntity.EditDay = editday;
        //                var editdate = entityDate.ToBinary();
        //                cloudEntity.EditDate = editdate;
        //            }
        //            toReturn.Add(cloudEntity);
        //        }

        //        //moreResultsAfterLimit
        //        if (response.Batch.MoreResults == "NO_MORE_RESULTS")
        //        {
        //            repeatDownload = false;
        //            break;
        //        }
        //        else
        //        {
        //            repeatDownload = true;
        //        }
        //    }
        //    return toReturn;
        //}

        private static List<CloudEntity> toCloudEntity(IList<EntityResult> results)
        {
            var toReturn = new List<CloudEntity>();
            foreach (var entityResult in results)
            {
                var entity = entityResult.Entity;
                var path = entity.Key.Path.FirstOrDefault();
                var cloudEntity = new CloudEntity()
                {
                    FormName = path.Kind,
                    Id = path.Name,
                    EntityId = entity.Properties["entityid"].StringValue,
                    DataBlob = entity.Properties["datablob"].StringValue,
                    KindMetaData = entity.Properties["kindmetadata"].StringValue,
                    //EditDate = Convert.ToInt64(
                    //    entity.Properties["editdate"].IntegerValue),
                    //EditDay = Convert.ToInt32(
                    //    entity.Properties["editday"].IntegerValue)
                };
                if (entity.Properties.ContainsKey("editdate"))
                {
                    var editDate = entity.Properties["editdate"].IntegerValue;
                    cloudEntity.EditDate = Convert.ToInt64(editDate);

                    var editDay = entity.Properties["editday"].IntegerValue;
                    cloudEntity.EditDay = Convert.ToInt32(editDay);
                }
                else
                {
                    //use field date added 
                    var entityDate = Convert.ToDateTime(
                        entity.Properties["dateadded"].TimestampValue);                  

                    var editday = entityDate.toYMDInt();
                    cloudEntity.EditDay = editday;

                    var local = entityDate.ToLocalTime();
                    var editdate = local.ToBinary();
                    cloudEntity.EditDate = editdate;
                }
                toReturn.Add(cloudEntity);
            }
            return toReturn;
        }

        private RunQueryRequest getQueryRequest(ResultsLimit skip, long editDate, bool fetchOldData, KindName kindName = null)
        {
            List<GqlQueryParameter> queryParams = null;
            var queryString = string.Empty;
            if (fetchOldData)
            {
                queryString = string.Format(
                    "select * from {0} where dateadded > @1 order by dateadded ASC limit @2",
                    kindName.Value,
                    skip.Value
                    );
                queryParams = new List<GqlQueryParameter>() {
                            new GqlQueryParameter() { Value =
                            new Value() { TimestampValue = 
                            DateTime.FromBinary(editDate)
                            } },
                    new GqlQueryParameter() { Value =
                            new Value() { IntegerValue = skip.Value } } };
            }
            else
            {
                queryString = string.Format(
    "select * from {0} where editdate > @1 order by editdate ASC limit @2",
    kindName.Value,
    skip.Value
    );
                queryParams = new List<GqlQueryParameter>() {
                            new GqlQueryParameter() { Value =
                            new Value() { IntegerValue = editDate } },
                    new GqlQueryParameter() { Value =
                            new Value() { IntegerValue = skip.Value } } };
            }

            return new RunQueryRequest()
            {
                GqlQuery = new GqlQuery()
                {
                    QueryString = queryString,
                    //AllowLiterals = true,
                    PositionalBindings = queryParams,
                   
                },
                ReadOptions = new ReadOptions() { }
            };
        }

        private RunQueryRequest getQueryRequest1(ResultsLimit skip, long editDate, bool fetchOldData, KindName kindName)
        {
            PropertyFilter filter = null;// dateFilter = 
            string datePropertyName = string.Empty;
            if (fetchOldData)
            {
                datePropertyName = DATEADDED;
                filter = new PropertyFilter()
                {
                    Property = new PropertyReference() { Name = DATEADDED },
                    Value = new Value() { TimestampValue = DateTime.FromBinary(editDate) },
                    Op = "GREATER_THAN"
                };
            }
            else
            {
                datePropertyName = EDITDATE;
                filter = new PropertyFilter()
                {
                    Property = new PropertyReference() { Name = EDITDATE },
                    Value = new Value() { IntegerValue = editDate },
                    //Op = "GREATER_THAN",
                    //Op = "GREATER_THAN_OR_EQUAL",
                    Op = "GREATER_THAN"
                };
            }

            return new RunQueryRequest()
            {
                Query = new Query()
                {
                    Filter = new Filter()
                    {
                        PropertyFilter = filter
                    },
                    Order = new List<PropertyOrder>() {
                    new PropertyOrder() { Direction="ASCENDING",
                        Property =new PropertyReference() {Name= datePropertyName } }
                },
                    Kind = new List<KindExpression> {
                        new KindExpression() { Name = kindName.Value } },
                    Limit = skip.Value,
                }
               ,
                //GqlQuery = new GqlQuery()
                //{
                //    QueryString = queryString,
                //    AllowLiterals = true,
                //    PositionalBindings = queryParams
                //},
                ReadOptions = new ReadOptions() { }
            };
        }
    }

    public class KindDataProcessor
    {
        public bool FetchOldData { get; internal set; }

        private long getLastSyncedDateForKind(KindName kindName)
        {
            var db = new CloudLocalStore(kindName);
            var entity = db.GetLatestEntity();
            if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            {
                return new DateTime(2016, 07, 08, 0, 0, 0, 1, DateTimeKind.Local).ToBinary();
            }
            return entity.EditDate;
        }

        public async Task<int> fetchRecordsForKind(KindName kindName, string projectId, DatastoreService datastore)
        {
            var skip = 50;
            var lastDateForKind = getLastSyncedDateForKind(kindName);

            var cloudEntities = await new FetchCloudDataWorker(datastore,
                projectId,
                kindName,
                new ResultsLimit(skip),
                lastDateForKind, this.FetchOldData)
            .beginFetchCloudData();
            if (cloudEntities.Count > 0)
            {
                var savedToLocal = await addToProcessingQueue(kindName, cloudEntities);
            }
            return 0;
        }

        public async Task<bool> addToProcessingQueue(KindName kindName, List<CloudEntity> cloudEntities)
        {
            var kindStore = new CloudLocalStore(kindName);
            foreach (var entity in cloudEntities)
            {
                kindStore.Update(entity);
            }
            return true;
        }
    }

    public class CloudDb
    {
        const string DATEADDED = "dateadded";
        public static List<string> getAllKindNames()
        {
            var allClientKinds = new List<string>();
            allClientKinds.AddRange(Constants.PPX_KIND_DISPLAYNAMES.Keys);
            allClientKinds.AddRange(Constants.VMMC_KIND_DISPLAYNAMES.Keys);
            allClientKinds.Add(Constants.KIND_APPUSERS);
            return allClientKinds;
        }

        public static List<string> getAllKindFields()
        {
            var allClientKinds = new List<string>();
            allClientKinds.AddRange(Constants.PPX_KIND_DISPLAYNAMES.Keys);
            allClientKinds.AddRange(Constants.VMMC_KIND_DISPLAYNAMES.Keys);
            allClientKinds.Add(Constants.KIND_APPUSERS);
            return allClientKinds;
        }
        //select __key__,kindmetadata From appusers
        //select __key__, kindmetadata From `vmmc_regandproc` where dateadded
        //select __key__, dateadded From `appusers` where dateadded >= datetime('2016-08-01T00:00:01Z')
        //select * from appusers where dateadded > datetime('2016-08-16T00:00:00.1Z')
        //select __key__, dateadded From `appusers` where dateadded >=datetime('2016-08-16T00:00:00.01Z') limit 50 offset 0 order by dateadded asc 
        //https://console.cloud.google.com/datastore/entities/query/gql?project=jhpzmb-vmmc-odk&ns=&kind=_GAE_MR_TaskPayload&gql=select%20*
        //Key(`__Stat_Total__`, 'total_entity_usage')
        //https://console.cloud.google.com/datastore/entities/edit?key=0%2F%7C19%2Fpp_client_devicerem%7C37%2Fname:5643a2ae27144cdaa1fe0d0e43b3864b&project=jhpzmb-vmmc-odk&ns=&kind=_GAE_MR_TaskPayload&gql=select%20*
        //https://console.cloud.google.com/datastore/entities/query/gql?project=jhpzmb-vmmc-odk&ns=&kind=_GAE_MR_TaskPayload&gql=select%20*%20from%20appusers
        //https://console.cloud.google.com/datastore/entities/edit?key=0%2F%7C8%2Fappusers%7C37%2Fname:524d91cffb024c7085148004cc47854c&project=jhpzmb-vmmc-odk&ns=&kind=_GAE_MR_TaskPayload&gql=select%20*%20from%20appusers
        //https://console.cloud.google.com/datastore/entities/query/gql?project=jhpzmb-vmmc-odk&ns=&kind=_AE_Backup_Information&gql=select%20*%20from%20appusers%20order%20by%20dateadded%20asc%20limit%20500%20offset%200%20where%20dateadded%20%3E%3D%20datetime(%272016-08-16T00:00:00.01Z%27)

        private async Task<Key> Save(DbSaveableEntity saveableEntity)
        {
            var assets = AppInstance.Instance.ApiAssets;
            var projectId = assets[Constants.ASSET_PROJECT_ID];
            var entity = new Entity()
            {
                Key = new Key()
                {
                    PartitionId = new PartitionId() { NamespaceId = "", ProjectId = projectId },
                    Path = new List<PathElement>() { new PathElement() {
                        Kind = saveableEntity.kindName.Value,
                        Name = saveableEntity.Id.Value}
                    }
                },
                Properties = new Dictionary<string, Value>()
                {
                    {"id", new Value() { StringValue = saveableEntity.Id.Value  } },
                    {"entityid", new Value() { StringValue = saveableEntity.EntityId.Value } },
                    {"dateadded", new Value() { TimestampValue = DateTime.Now } },
                    {"datablob", new Value() {ExcludeFromIndexes=true,
                        StringValue =saveableEntity
                    .getJson()
                    .Encrypt()
                    .Value}
                    },
                    {"kindmetadata", new Value() { StringValue = saveableEntity.Entity.KindMetaData??string.Empty } }
                }
            };
            var datastore = GetDatastoreService(GetDefaultCredential(assets), assets);
            return await SaveToCloud(datastore, projectId, entity);
        }

        private async Task<Key> SaveToCloud(DatastoreService datastore, string projectId, Entity entity)
        {
            var trxbody = new BeginTransactionRequest();
            var beginTrxRequest = datastore.Projects.BeginTransaction(trxbody, projectId);
            var res = await beginTrxRequest.ExecuteAsync();
            var trxid = res.Transaction;

            var commitRequest = new CommitRequest();
            commitRequest.Mutations = new List<Mutation>() {
                new Mutation() {   Upsert = entity }
            };
            commitRequest.Transaction = trxid;
            var commmitReq = datastore.Projects.Commit(commitRequest, projectId);

            var commitExec = await commmitReq.ExecuteAsync();
            var res1 = commitExec.MutationResults.FirstOrDefault();
            return res1.Key;
        }

        private ServiceAccountCredential GetDefaultCredential(Dictionary<string, string> assets)
        {
            //System.IO.Stream mstream = null;
            //var googleCredential = Google.Apis.Auth.OAuth2.GoogleCredential.FromStream(mstream);
            var cert =
                SyncManager.Properties.Resources.key;
                //assetManager.Open(assets[Constants.ASSET_P12KEYFILE]).toByteArray();
            var serviceAccountEmail = assets[Constants.ASSET_NAME_SVC_ACCTEMAIL];
            var certificate = new X509Certificate2(cert, "notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(
              new ServiceAccountCredential.Initializer(serviceAccountEmail)
              {
                  Scopes = new[] { DatastoreService.Scope.Datastore }
              }.FromCertificate(certificate));
            return credential;
        }

        private DatastoreService GetDatastoreService(ServiceAccountCredential credential, Dictionary<string, string> assets)
        {
            return new DatastoreService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = assets[Constants.ASSET_NAME_APPNAME],
            });
        }

        private int AddToOutQueue(DbSaveableEntity saveableEntity)
        {
            var asString = saveableEntity.getJson();
            var outEntity = new OutEntity()
            {
                Id = saveableEntity.Id.Value,
                DataBlob = asString
            };
            var res = 0;
                //new OutDb().DB.InsertOrReplace(outEntity);
            return res;
        }

        public static async Task<bool> checkConnection()
        {
            var googleUrl = "https://google.co.zm";
            var toReturn = false;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(googleUrl);
                request.Timeout = 5000;
                WebResponse response;
                response = await request.GetResponseAsync();
                response.Close();
                return true;
            }
            catch (WebException ex)
            {
                toReturn = false;
            }
            catch (Exception e)
            {
                toReturn = false;
            }
            return toReturn;
        }

        int isRunning = 0;

        public Dictionary<string, string> ApiAssets { get; set; }

        private async Task<int> doServerSync(Action<int> updateProgress, bool syncOldData)
        {
            var currProgress = 0;
            var hasConnection = await checkConnection();
            if (!hasConnection)
            {
                isRunning = 0;
                updateProgress(100);
                return 0;
            }

            var assets = ApiAssets;
            var projectId = assets[Constants.ASSET_PROJECT_ID];
            var datastore = GetDatastoreService(GetDefaultCredential(assets), assets);

            var kindNames = getAllKindNames();

            updateProgress(currProgress += 5);
            var progressStep = 70 / kindNames.Count;
            var checkConnStep = 25 / kindNames.Count;

            foreach (var kind in kindNames)
            {
                var worker = new KindDataProcessor() { FetchOldData = syncOldData };
                var res = await worker.fetchRecordsForKind(kind.toKind(), projectId, datastore);
                            
                updateProgress(currProgress += progressStep);
                
                //check if we have a connection
                hasConnection = await checkConnection();
                updateProgress(currProgress += checkConnStep);
            }
            updateProgress(100);
            return 0;
        }

        public async Task<int> EnsureServerSync(Action<int> setProgress, bool syncOldData)
        {
            var progress = new System.Progress<int>();
            progress.ProgressChanged += (sender, e) => { setProgress(e); };
            var asIProgress = progress as IProgress<int>;

            if (isRunning == 1)
                return 0;

            isRunning = 1;
            await Task.Run(async () => await doServerSync(asIProgress.Report, syncOldData)
            );

            isRunning = 0;
            return 0;
        }

        internal void RefreshLocalEntities(Action<int> setProgress)
        {
            var cloudTables = getAllKindNames();

            //we get the keys and editdates
            foreach (var table in cloudTables)
            {
                var localCloudData = new CloudLocalStore<CloudEntity>(table.toKind());
                var entities = localCloudData.GetUnsyncedLocalEntities(
                    cloudTable:table.toKind(),
                    localTable:getLocalTableName(table).toKind()
                    );
                if (entities.Count == 0)
                    continue;

                var entityConverter = new EntityConverter();
                var localStore = new CloudLocalStore<LocalEntity>(getLocalTableName(table).toKind());
                var flatStore = new FieldValueStore(getTableFieldValueName(table)) {
                    batchSize = 50 };

                foreach (var entity in entities)
                {
                    //we decrypt
                    var localEntity = entityConverter.toLocalEntity(entity);
                    localEntity.FormName = table;
                    var saved = localStore.Update(localEntity);
                    if (saved == null)
                    {
                        //means we couldn't save, so we do what?
                        //throw exception??
                        log(string.Format("Couldn't save record for ", table, entity.Id));
                        continue;
                    }

                    //and deidentify
                    var deid = entityConverter.toDeidEntity(localEntity);

                    var ged = DbSaveableEntity.fromJson<GeneralEntityDataset>(
                        new KindItem(deid.DataBlob)
                        );

                    //todo: modify so we sync from CloudLocalStore separately than when downloading
                    //and save to localTables
                    flatStore.Save(ged, localEntity, saved.recordId);
                }

                //call finalise
                flatStore.finalise();
            }

            //check if we have these in the local tables

            //fetch and process records missing

            //or do a sql comparison

        }

        void log(string message)
        {

        }

        public static string getLocalTableName(string table)
        {
            return table + "_local";
        }

        public static string getTableFieldValueName(string table)
        {
            return table + "_local_fvs";
        }        
    }
}