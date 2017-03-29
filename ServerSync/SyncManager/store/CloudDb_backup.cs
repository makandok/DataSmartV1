using System;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Datastore.v1;
using Android.Content.Res;
using Google.Apis.Datastore.v1.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using JhpDataSystem.model;
using System.Net;
using Android.Widget;
using SyncManager.store;
using SyncManager.model;

namespace JhpDataSystem.store
{
    public class CloudDb_graveyard
    {
        const string DATEADDED = "dateadded";
        public List<string> getAllKindNames()
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

        public DateTime getMostRecentDate(KindName kindName)
        {
            return DateTime.MinValue;
        }

        public void addToProcessingQueue(List<CloudEntity> cloudEntities)
        {
            
        }
        public async Task<int> doServerSync(KindName kindName, string projectId, DatastoreService datastore)
        {
            var offset = 0;
            var skip = 500;
            var lastDateForKind = getMostRecentDate(kindName);
            bool hasData;
            do
            {
                var cloudEntities = await GetCloudEntities1(datastore,
                    projectId,
                    kindName,
                    new Offset(offset),
                    new ResultsLimit(skip),
                    lastDateForKind
                    );
                hasData = cloudEntities.Count > 0;

                //we save this result
                addToProcessingQueue(cloudEntities);
                offset += skip;
            } while (hasData);
            return 0;
        }

        public async Task<List<CloudEntity>> beginServerSync()
        {
            var kindNames = new List<KindName>();
            var assets = ApiAssets;
            var projectId = assets[Constants.ASSET_PROJECT_ID];
            var datastore = GetDatastoreService(GetDefaultCredential(assets), assets);

            foreach(var kind in kindNames)
            {
                await doServerSync(kind, projectId, datastore);
            }

            return null;
        }

        public RunQueryRequest getQueryRequest(DateTime dateAdded, KindName kindName = null)
        {
            var kindExpression = kindName == null ?
                new List<KindExpression>() :
                new List<KindExpression> { new KindExpression() { Name = kindName.Value } };

            return new RunQueryRequest()
            {

                Query = new Query()
                {
                    Limit = 10,
                    Offset = 0,
                    Filter = new Google.Apis.Datastore.v1.Data.Filter()
                    {
                        PropertyFilter = new PropertyFilter()
                        {
                            Property = new PropertyReference() { Name = DATEADDED },
                            Value = new Value() { TimestampValue = dateAdded },
                            Op = ">="
                        }
                    },
                    Order = new List<PropertyOrder>() {
                        new PropertyOrder() { Direction="asc",
                            Property =new PropertyReference() {Name= DATEADDED } }
                    },
                    Kind = kindExpression
                },
                //GqlQuery = new GqlQuery() { QueryString = "select __key__, cardserial From jhpsystems" }
                ReadOptions = new ReadOptions() { }
            };
        }

        public async Task<List<CloudEntity>> GetCloudEntities1(
            DatastoreService dataStore, string projectId,
            KindName kindName, Offset offset, ResultsLimit skip, DateTime dateAdded)
        {
            var res = dataStore.Projects.RunQuery(
                getQueryRequest(dateAdded, kindName), projectId);

            var response = await res.ExecuteAsync();
            var entityResults = response.Batch.EntityResults;

            var toReturn = new List<CloudEntity>();
            foreach(var entityResult in response.Batch.EntityResults)
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

                    //EditDay = Convert.ToInt32( entity.Properties["editday"].IntegerValue),
                    //EditDate = Convert.ToInt64(entity.Properties["editdate"].IntegerValue),

                    //{ "editday", new Value() { IntegerValue = now.toYMDInt() } },
                    //{ "editdate", new Value() { IntegerValue = now.ToBinary() } },

                    //DateAdded = Convert.ToDateTime(
                    //    entity.Properties["dateadded"].TimestampValue)
                };

                if (entity.Properties.ContainsKey("editdate")){
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

        public async void GetCloudEntities(Dictionary<string, string> assets)
        {
            var projectId = assets[Constants.ASSET_PROJECT_ID];

            // Create the service.
            var datastore = GetDatastoreService(GetDefaultCredential(assets), assets);

            //Retrieve entities from the server
            var res = datastore.Projects.RunQuery(
                new RunQueryRequest()
                {
                    //Query = new Query(){Limit = 10,Kind = new List<KindExpression> { new KindExpression() {Name = "jhpsystems" }, }},
                    GqlQuery = new GqlQuery() { QueryString = "select __key__, cardserial From jhpsystems" }
                    ,
                    ReadOptions = new ReadOptions() { }
                }, projectId);

            var response = await res.ExecuteAsync();

            var y = (
            from entityResult in response.Batch.EntityResults
            let entity = entityResult.Entity
            select new { entity.Key.Path, entity.Properties.Values }).ToList();

            var batch = response.Batch.EntityResults;
        }

        public async Task<Key> Save(DbSaveableEntity saveableEntity)
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

        public int AddToOutQueue(DbSaveableEntity saveableEntity)
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

        public List<OutEntity> GetRecordsToSync()
        {
            //return new OutDb().DB.Table<OutEntity>().ToList();
            return new List<OutEntity>();
        }

        public async Task<bool> checkConnection()
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

        public async Task<int> EnsureServerSync(WaitDialogHelper myDialog)
        {
            if (isRunning == 1)
                return 0;
            isRunning = 1;

            await myDialog.showDialog(doServerSync);

            isRunning = 0;
            return 0;
        }

        public async Task<int> doServerSync(Action<string, ToastLength> makeToast)
        {
            makeToast = (string a, ToastLength b) => { };
            //var recs = GetRecordsToSync();
            //if (recs.Count == 0)
            //{
            //    makeToast("No unsynced records", ToastLength.Short);
            //    return 0;
            //}

            var hasConnection = await checkConnection();
            if (!hasConnection)
            {
                isRunning = 0;
                makeToast?.Invoke("No connection detected", ToastLength.Long);
                return 0;
            }
            makeToast("Connected tested", ToastLength.Short);
            var recs = new List<OutEntity>();
                
                //await GetCloudEntities1("appusers");


            return recs.Count;

            var cloudDb = new OutDb().DB;
            
            var recIndex = recs.Count - 1;
            while (recIndex >= 0 && hasConnection)
            {
                var outEntity = recs[recIndex];
                var ppdataset = DbSaveableEntity.fromJson<GeneralEntityDataset>(
                    new KindItem(outEntity.DataBlob));
                var saveable = new DbSaveableEntity(ppdataset)
                {
                    kindName = new KindName(ppdataset.FormName)
                };

                var saved = false;
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        await Save(saveable);
                        //we remove this key from the database
                        saved = true;
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
                        //unknown exception
                    }
                    finally { }
                    if (saved)
                    {
                        try
                        {
                            //we delete the record from downloads db
                            //var deleted = cloudDb.Delete<OutEntity>(saveable.Id.Value);
                        }
                        catch
                        {

                        }
                        break;
                    }
                    else
                    {
                        //lets add a 2 second delay in case it failed the first time
                        await Task.Delay(TimeSpan.FromMilliseconds(2000));
                    }
                }
                recIndex--;
                hasConnection = await checkConnection();              
            }            
            return 0;
        }
    }
}