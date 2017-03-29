using System;
using System.Text;

using Android.App;
/*
 * Copyright (c) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * Sample code to issue several basic Google Cloud Store (GCS) operations
 * using the Google Client Libraries.
 * For more information, see documentation for Compute Storage .NET client
 * https://cloud.google.com/storage/docs/json_api/v1/json-api-dotnet-samples
 */

using System.IO;

using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Apis.Download;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Datastore.v1beta3;
using Google.Apis.Datastore.v1beta3.Data;

namespace DataSmart.db
{
    public class StorageSample
    {
        public void DoNothing()
        {
            var serviceAccountEmail = "***.apps.googleusercontent.com";

            var certificate = new X509Certificate2(
                @"***-privatekey.p12", "notasecret", X509KeyStorageFlags.Exportable);


            //var t = Google.Apis.Auth.OAuth2.ServiceAccountCredential

            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = new[] {
           Google.Apis.Datastore.v1beta3.DatastoreService.Scope.Datastore.ToString().ToLower()
                }
            }.FromCertificate(certificate));

            // Create the service.
            var service = new DatastoreService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "***",
            });

            var request = new Google.Apis.Datastore.v1beta3.Data
                .BeginTransactionRequest();
            // GoogleData.BlindWriteRequest();
            var entity = new Entity();
            entity.Key = new Google.Apis.Datastore.v1beta3.Data.Key();
            entity.Key.Path = new List<Google.Apis.Datastore.v1beta3.Data.PathElement>();
            entity.Key.Path.Add(new Google.Apis.Datastore.v1beta3.Data.PathElement() { Kind = "KindTest", Name = "name" });
            var firstName = new Google.Apis.Datastore.v1beta3.Data.PropertyReference();//.Property();
            //firstName.
            //firstName.Values = new List<Google.Apis.Datastore.v1beta3.Data.Value>();
            //firstName. .Values.Add(new Google.Apis.Datastore.v1beta3.Data.Value { StringValue = "FName" });
            entity.Properties = new Dictionary<string, Google.Apis.Datastore.v1beta3.Data.Value>();
            entity.Properties["firstName"] = new 
                Google.Apis.Datastore.v1beta3.Data.Value { StringValue = "FName" };

            entity.Properties.Add("FirstName", new
                Google.Apis.Datastore.v1beta3.Data.Value
            { StringValue = "FName" });

            //request.Mutation = new Mutation();
            //request.Mutation.Upsert = new List<Entity>();
            //request.Mutation.Upsert.Add(entity);

            //var response = service.Datasets.BlindWrite(request, "***").Execute();
        }

        public StorageService CreateStorageClient()
        {
            var tokenServerUrl = "";
            var credentials = new Google.Apis.Auth.OAuth2.ComputeCredential();
            var initializer = new Google.Apis.Auth.OAuth2.ServiceCredential.Initializer(tokenServerUrl);


            //if (credentials.IsCreateScopedRequired)
            //{
            //    credentials = credentials.CreateScoped(new[] { StorageService.Scope.DevstorageFullControl });
            //}

            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = "Storage Sample",
                HttpClientInitializer = credentials
            };

            return new StorageService(serviceInitializer);
        }
        // [END create_storage_client]

        // [START list_buckets]
        public void ListBuckets(string projectId)
        {
            StorageService storage = CreateStorageClient();

            var buckets = storage.Buckets.List(projectId).Execute();

            if (buckets.Items != null)
            {
                foreach (var bucket in buckets.Items)
                {
                    Console.WriteLine($"Bucket: {bucket.Name}");
                }
            }
        }
        // [END list_buckets]

        // [START list_objects]
        public void ListObjects(string bucketName)
        {
            StorageService storage = CreateStorageClient();

            var objects = storage.Objects.List(bucketName).Execute();

            if (objects.Items != null)
            {
                foreach (var obj in objects.Items)
                {
                    Console.WriteLine($"Object: {obj.Name}");
                }
            }
        }
        // [END list_objects]

        // [START upload_stream]
        public void UploadStream(string bucketName)
        {
            StorageService storage = CreateStorageClient();

            var content = "My text object content";
            var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            storage.Objects.Insert(
                bucket: bucketName,
                stream: uploadStream,
                contentType: "text/plain",
                body: new Google.Apis.Storage.v1.Data.Object() { Name = "my-file.txt" }
            ).Upload();

            Console.WriteLine("Uploaded my-file.txt");
        }
        // [END upload_stream]

        // [START download_stream]
        public void DownloadStream(string bucketName)
        {
            StorageService storage = CreateStorageClient();

            using (var stream = new MemoryStream())
            {
                storage.Objects.Get(bucketName, "my-file.txt").Download(stream);

                var content = Encoding.UTF8.GetString(stream.GetBuffer());

                Console.WriteLine($"Downloaded my-file.txt with content: {content}");
            }
        }
        // [END download_stream]

        // [START delete_object]
        public void DeleteObject(string bucketName)
        {
            StorageService storage = CreateStorageClient();

            storage.Objects.Delete(bucketName, "my-file.txt").Execute();

            Console.WriteLine("Deleted my-file.txt");
        }
        // [END delete_object]

        // [START media_downloader]
        public void DownloadToFile(string bucketName)
        {
            StorageService storage = CreateStorageClient();

            var objectToDownload = storage.Objects.Get(bucketName, "my-file.txt").Execute();

            var downloader = new MediaDownloader(storage);

            downloader.ProgressChanged += progress =>
            {
                Console.WriteLine($"{progress.Status} {progress.BytesDownloaded} bytes");
            };

            using (var fileStream = new FileStream("downloaded-file.txt", FileMode.Create))
            {
                var progress = downloader.Download(objectToDownload.MediaLink, fileStream);

                if (progress.Status == Google.Apis.Download.DownloadStatus.Completed)
                {
                    Console.WriteLine("Downloaded my-file.txt to downloaded-file.txt");
                }
            }
        }
        // [END media_downloader]
    }
}