using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AzureTraining.Models
{
    public class EmpBlobOperations
    {
        private static CloudBlobContainer profileBlobContainer;
        // Initialize BLOB and Queue Here
        public EmpBlobOperations()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["empstorage"].ToString());
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Get the blob container reference.
            profileBlobContainer = blobClient.GetContainerReference("employees");
            //Create Blob Container if not exist
            profileBlobContainer.CreateIfNotExists();
        }

        // Method to Upload the BLOB
        public async Task<CloudBlockBlob> UploadBlob(HttpPostedFileBase profileImage)
        {
            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
            // GET a blob reference. 
            CloudBlockBlob profileBlob = profileBlobContainer.GetBlockBlobReference(blobName);
            // Uploading a local file and Create the blob.
            using (var fs = profileImage.InputStream)
            {
                await profileBlob.UploadFromStreamAsync(fs);
            }
            return profileBlob;
        }
    }
}