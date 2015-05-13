using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace StorageService
{
    public class SimpleStorageService
    {
        private CloudStorageAccount _storage;
        private CloudBlobClient _client;

        public SimpleStorageService()
        {
            _storage = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["Storage"].ToString());
            _client = _storage.CreateCloudBlobClient();
        }

        public IEnumerable<string> GetFilesList(string user)
        {
            var container = _client.GetContainerReference(user.ToLower());

            if (container == null)
            {
                return new[] {""};
            }

            var items = container.ListBlobs(null, true);
            return items.Select(x => x.Uri.LocalPath);
        }

        public void PutFiles(string user, string timeTable, HttpPostedFileBase[] files)
        {
            var container = _client.GetContainerReference(user.ToLower());
            container.CreateIfNotExists();
            
            foreach (var file in files)
            {
                var blob = container.GetBlockBlobReference("orarSme/" + file.FileName);
                blob.UploadFromStream(file.InputStream);
            }
        }

        public string GetDownloadLink(string user, string blobString)
        {
            var container = _client.GetContainerReference(user.ToLower());
            Stream stream;
            if (container != null)
            {
                var index = blobString.IndexOf(container.Name) + container.Name.Length;
                var newBlobString = blobString.Substring(index+1);
                var blob = container.GetBlockBlobReference(newBlobString);
                if (blob != null)
                {
                    var policy = new SharedAccessBlobPolicy()
                    {
                        Permissions = SharedAccessBlobPermissions.Read,
                        SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1)
                    };

                    var headers = new SharedAccessBlobHeaders()
                    {
                        ContentDisposition = string.Format("attachment;filename=\"{0}\"", Path.GetFileName(blobString))
                    };

                    var sasToken = blob.GetSharedAccessSignature(policy, headers);
                    return blob.Uri.AbsoluteUri + sasToken;
                }
            }

            return string.Empty;
        }
    }
}
