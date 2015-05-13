using System.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace StorageService
{
    public class StorageService
    {
        private CloudStorageAccount _storage;

        public StorageService()
        {
            _storage = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["Storage"].ToString());
        }
    }
}
