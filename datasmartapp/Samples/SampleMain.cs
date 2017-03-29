using System;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Datastore.v1beta3;
using Android.Content.Res;
using Google.Apis.Datastore.v1beta3.Data;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using JhpDataSystem;

namespace DataSmart.db
{
    public class got
    {
       AssetManager _assetManager { get; set; }        
       public string AplicationKey { get; internal set; }
        public got(AssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public DatastoreService CreateDataStoreClient(Dictionary<string, string> assets)
        {
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync().Result;
            if (credentials.IsCreateScopedRequired)
            {
                credentials = credentials.CreateScoped(new[] {
                    DatastoreService.Scope.Datastore,
                    //DatastoreService.Scope.CloudPlatform
                });
            }

            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = assets[Constants.ASSET_NAME_APPNAME],
                HttpClientInitializer = credentials
            };

            return new DatastoreService(serviceInitializer);
        }

        Key getKey(string kind)
        {
            return new Key() { Path = new[] { new PathElement() { Kind = kind } } };
        }

        List<Key> allocateIds(DatastoreService datasstore, List<Key> keys, string projectId)
        {
            var alloc = new AllocateIdsRequest() {Keys = keys };
            var worker = datasstore.Projects.AllocateIds(alloc, projectId);
            var res = worker.Execute();
            return keys;
        }


        public async void unwrapAriaStark(Dictionary<string, string> assets)
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
        
        public async Task<Key> trainAriaStark()
        {
            var assets = AppInstance.Instance.ApiAssets;
            var projectId = assets[Constants.ASSET_PROJECT_ID];
            var datastore = GetDatastoreService(GetDefaultCredential(assets, _assetManager), assets);
            return await trainAriaStark(datastore, projectId);
        }

        private async Task<Key> trainAriaStark(DatastoreService datastore, string projectId)
        {
            var trxbody = new BeginTransactionRequest();
            var beginTrxRequest = datastore.Projects.BeginTransaction(trxbody, projectId);
            var res = await beginTrxRequest.ExecuteAsync();

            var trxid = res.Transaction;
            var entity = new Entity()
            {

                Key = new Key()
                {
                    PartitionId = new PartitionId() { NamespaceId = "", ProjectId = projectId },
                    Path = new List<PathElement>() { new PathElement() { Kind = "jhpsystems",
                        Name = "A8CFCA9D-0B3C-43F7-B7AD-DA21AE79C92F" } }
                },
                Properties = new Dictionary<string, Value>()
                {

                    {"cardserial", new Value() { IntegerValue = 2022 } },
                    {"datablob", new Value() { StringValue = "I was once a blob" } },
                    {"registerno", new Value() { IntegerValue = 666 } }
                }
            };

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

        private ServiceAccountCredential GetDefaultCredential(Dictionary<string,string> assets, AssetManager assetManager)
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

        internal DatastoreService GetDatastoreService(ServiceAccountCredential credential, Dictionary<string,string> assets)
        {
            return new DatastoreService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = assets[Constants.ASSET_NAME_APPNAME],
            });
        }

        public void manyFacedGod(Dictionary<string,string> assets)
        {
            var projectId = assets[Constants.ASSET_PROJECT_ID];
            var credential = GetDefaultCredential(assets, _assetManager);

            // Create the service.
            var datastore = GetDatastoreService(credential, assets);


            //create the keys
            //assign ids
            var keys = allocateIds(datastore,
                new List<Key> { getKey("jhpsystems"), getKey("jhpsystems"), getKey("jhpsystems") },
                projectId
                );
            //select __key__, cardserial, registerno From jhpsystems
            //assign to entities
            var entities = new List<Mutation>();
            var x = 1;
            foreach (var key in keys)
            {
                x++;
                var entity = new Entity() { Key = key };
                entity.Properties = new Dictionary<string, Value>() {
                    {"cardserial", new Value() { IntegerValue = 20*x }},
                    {"datablob", new Value() { StringValue = "This is a blob "+ x }},
                    {"registerno", new Value() { IntegerValue = 330 + x }},
                    {"dateadded", new Value() { TimestampValue = DateTime.Now }}
                };
                entities.Add(new Mutation() { Upsert = entity });
            }

            //we figure out how to save
            var tr = new BeginTransactionRequest() { };
            var trid = datastore.Projects.BeginTransaction(tr, projectId);

            var mut = new Mutation() { Upsert = new Entity() };
            var cr = new CommitRequest() { Mutations = entities };

            datastore.Projects.Commit(cr, projectId);

            //perhaps we save to the local database

            //alert that we are done saving
            var t = 0;
        }
    }
}