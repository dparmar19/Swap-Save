using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Amazon.S3.Transfer;
using Amazon;
using TNG.Shared.Lib.Intefaces;
using System;
using TNG.Shared.Lib.Models.BarterShop;
using TNG.Shared.Lib.Mongo.BarterShop.Master;

namespace TNG.Shared.Lib
{
    public class S3Layer : IS3Layer
    {
        private S3LayerSettings _s3LayerSetting;
        private AmazonS3Client _client;
        public S3Layer(S3LayerSettings s3LayerSettings)
        {
            this._s3LayerSetting = s3LayerSettings;
            this._client = new AmazonS3Client(s3LayerSettings.accessKeyId, s3LayerSettings.accessSecretKey, Amazon.RegionEndpoint.USEast1);

        }

        /// <summary>
        /// for uploading image
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="clientFolder"></param>
        /// <param name="storeFolder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool UploadObject(Stream stream, string contentFolder, string fileName)
        {
            string newFolderName = string.Empty;
            try
            {
                var request = new PutObjectRequest();
                request.BucketName = string.Concat(this._s3LayerSetting.bucket, @"/", contentFolder);
                request.Key = fileName;
                request.InputStream = stream;
                request.ContentType = "image/png";
                request.CannedACL = S3CannedACL.PublicRead;
                var response = this._client.PutObjectAsync(request);
                response.Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool UploadReceipt(Stream stream, string clientFolder, string storeFolder, string contentFolder, string fileName)
        {
            string newFolderName = string.Empty;
            try
            {
                var request = new PutObjectRequest();
                request.BucketName = string.Concat(this._s3LayerSetting.bucket, @"/", clientFolder, @"/", storeFolder, @"/", contentFolder);
                request.Key = fileName;
                request.InputStream = stream;
                request.ContentType = "text/html";
                request.CannedACL = S3CannedACL.PublicRead;
                var response = this._client.PutObjectAsync(request);
                response.Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UploadFile(Stream stream, string contentFolder, string fileName)
        {
            string newFolderName = string.Empty;
            try
            {
                var request = new PutObjectRequest();
                request.BucketName = string.Concat(this._s3LayerSetting.bucket, @"/", contentFolder);
                request.Key = fileName;
                request.InputStream = stream;
                request.ContentType = "application/pdf";
                request.CannedACL = S3CannedACL.PublicRead;
                var response = this._client.PutObjectAsync(request);
                response.Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UploadVideo(Stream stream, string contentFolder, string fileName)
        {
            string newFolderName = string.Empty;
            try
            {
                var request = new PutObjectRequest();
                request.BucketName = string.Concat(this._s3LayerSetting.bucket, @"/", contentFolder);
                request.Key = fileName;
                request.InputStream = stream;
                request.ContentType = "application/mp4";
                request.CannedACL = S3CannedACL.PublicRead;
                var response = this._client.PutObjectAsync(request);
                response.Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// copying image
        /// </summary>
        /// <param name="clientFolder"></param>
        /// <param name="storeFolder"></param>
        /// <param name="contentFolder"></param>
        /// <param name="sourceKey"></param>
        /// <param name="destinationKey"></param>
        /// <returns></returns>
        public bool CopyObject(string clientFolder, string storeFolder, string contentFolder, string sourceKey, string destinationKey)
        {
            string newFolderName = string.Empty;
            try
            {
                var bucketName = string.Concat(this._s3LayerSetting.bucket, @"/", clientFolder, @"/", storeFolder, @"/", contentFolder);
                CopyObjectRequest request = new CopyObjectRequest
                {
                    SourceBucket = bucketName,
                    SourceKey = sourceKey,
                    DestinationBucket = bucketName,
                    DestinationKey = destinationKey
                };
                var response = this._client.CopyObjectAsync(request);
                response.Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool DeleteObject(string clientFolder, string storeFolder, string contentFolder, string fileName)
        {
            string newFolderName = string.Empty;
            try
            {
                var bucketName = string.Concat(this._s3LayerSetting.bucket, @"/", clientFolder, @"/", storeFolder, @"/", contentFolder);
                var deleteObjectRequest = new DeleteObjectRequest()
                {
                    BucketName = bucketName,
                    Key = fileName
                };
                var response = this._client.DeleteObjectAsync(deleteObjectRequest);
                response.Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// to check the folder exist
        /// </summary>
        /// <param name="subDirectoryInBucket"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private int doesFolderExist(string subDirectoryInBucket, AmazonS3Client client)
        {
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = this._s3LayerSetting.bucket;
                request.Prefix = (subDirectoryInBucket + "/");
                request.MaxKeys = 1;
                var response = client.ListObjectsAsync(request);
                response.Wait();
                var n = response.Result.S3Objects.Count;
                return n;
            }
            catch
            {
                return 0;
            }
        }


        /// <summary>
        /// for creating new folder
        /// </summary>
        /// <param name="subDirectoryInBucket"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private bool createFolder(string subDirectoryInBucket, AmazonS3Client client)
        {
            var folderKey = subDirectoryInBucket + "/"; //end the folder name with "/"
            var request = new PutObjectRequest();
            request.BucketName = this._s3LayerSetting.bucket;
            request.StorageClass = S3StorageClass.Standard;
            request.ServerSideEncryptionMethod = ServerSideEncryptionMethod.None;
            request.CannedACL = S3CannedACL.BucketOwnerFullControl;
            request.Key = folderKey;
            request.ContentBody = string.Empty;
            var response = client.PutObjectAsync(request);
            response.Wait();
            return true;
        }

        /// <summary>
        /// for retrieving object 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="subDirectoryInBucket"></param>
        /// <returns></returns>
        public async Task<string> ReadObjectData(string keyName, string subDirectoryInBucket)
        {

            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = this._s3LayerSetting.bucket + @"" + subDirectoryInBucket,
                    Key = keyName
                };
                using (GetObjectResponse response = await this._client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string contentType = response.Headers["Content-Type"];
                    responseBody = reader.ReadToEnd();
                }
                return responseBody;
            }
            catch
            {
                return null;
            }
        }

        public bool IsFileExist(string clientFolder, string storeFolder, string contentFolder, string fileName)
        {
            try
            {
                GetObjectMetadataRequest request = new GetObjectMetadataRequest();
                request.BucketName = string.Concat(this._s3LayerSetting.bucket, @"/", clientFolder, @"/", storeFolder, @"/", contentFolder);
                request.Key = fileName;
                var response = this._client.GetObjectMetadataAsync(request);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}