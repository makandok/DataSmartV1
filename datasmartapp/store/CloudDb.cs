using System;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Datastore.v1beta3;
using Android.Content.Res;
using Google.Apis.Datastore.v1beta3.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;

namespace DataSmart.store
{
    public class CloudDb
    {
        AssetManager _assetManager { get; set; }
        public CloudDb(AssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public async void GetCloudEntities(Dictionary<string, string> assets)
        {
            var projectId = assets[Constants.ASSET_PROJECT_ID];

            // Create the service.
            var datastore = GetDatastoreService(GetDefaultCredential(assets, _assetManager), assets);

            //Retrieve entities from the server
            var res = datastore.Projects.RunQuery(
                new RunQueryRequest()
                {
                    //Query = new Query(){Limit = 10,Kind = new List<KindExpression> { new KindExpression() {Name = "jhpsystems" }, }},
                    GqlQuery = new GqlQuery() { QueryString = "select __key__, cardserial From jhpsystems" }
                    ,
                    ReadOptions = new ReadOptions() { }
                }, projectId);

            var response = await res.ExecuteAsync();// .Execute();

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
            var now = DateTime.Now;
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
                    //{"dateadded", new Value() { IntegerValue = now.toYMDInt() } },
                    {"editday", new Value() { IntegerValue = now.toYMDInt() } },
                    {"editdate", new Value() { IntegerValue = now.ToBinary() } },
                    {"datablob", new Value() {ExcludeFromIndexes=true,
                        StringValue =saveableEntity
                    .getJson()
                    .Encrypt()
                    .Value}
                    },
                    {"kindmetadata", new Value() { StringValue = saveableEntity.Entity.KindMetaData??string.Empty } }
                }
            };
            var datastore = GetDatastoreService(GetDefaultCredential(assets, _assetManager), assets);
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

        private ServiceAccountCredential GetDefaultCredential(Dictionary<string, string> assets, AssetManager assetManager)
        {
            var cert = assetManager.Open(assets[Constants.ASSET_P12KEYFILE]).toByteArray();
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
            var res = new OutDb().DB.InsertOrReplace(outEntity);
            return res;
        }

        public List<OutEntity> GetRecordsToSync()
        {
            return new OutDb().DB.Table<OutEntity>().ToList();
        }

        //public int GetRecordsToSyncCount()
        //{
        //    return new OutDb().DB.Table<OutEntity>().Count();
        //}

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
        public async Task<int> EnsureServerSync(WaitDialogHelper myDialog)
        {
            if (isRunning == 1)
                return 0;
            isRunning = 1;

            await myDialog.showDialog(doServerSync);

            isRunning = 0;
            return 0;
        }

        async Task<int> doServerSync(Action<string, ToastLength> makeToast)
        {
            var recs = GetRecordsToSync();
            if (recs.Count == 0)
            {
                makeToast("No unsynced records", ToastLength.Short);
                return 0;
            }

            var hasConnection = await checkConnection();
            if (!hasConnection)
            {
                isRunning = 0;
                if (makeToast != null)
                {
                    makeToast("No connection detected", ToastLength.Long);
                }
                return 0;
            }
            makeToast("Connected tested", ToastLength.Short);
            var cloudDb = new OutDb().DB;
            
            var recIndex = recs.Count - 1;
            while (recIndex >= 0 && hasConnection)
            {
                var outEntity = recs[recIndex];
                var ppdataset = DbSaveableEntity.fromJson<GeneralEntityDataset>(new KindItem(outEntity.DataBlob));
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
                            var deleted = cloudDb.Delete<OutEntity>(saveable.Id.Value);
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