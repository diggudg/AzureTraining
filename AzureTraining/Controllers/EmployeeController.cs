using AzureTraining.Models;
using EmpDetails;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureTraining.Controllers
{
    public class EmployeeController : Controller
    {
        EmpBlobOperations blobOperations;
        EmpTableOperations tableOperations;

        public EmployeeController()
        {
            blobOperations = new EmpBlobOperations();
            tableOperations = new EmpTableOperations();
        }
        // GET: Employee
        public ActionResult Index()
        {
            var details = tableOperations.GetEntities();
            return View(details);
        }
        public ActionResult Create()
        {
            var Details = new EmpDetailsClass();
            Details.EmpId = new Random().Next(); //Generate the Employee Id Randomly
            return View(Details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
               EmpDetailsClass obj,
          HttpPostedFileBase profileFile

)
        {

            CloudBlockBlob profileBlob = null;
            #region Upload File In Blob Storage
            //Step 1: Uploaded File in BLob Storage
            if (profileFile != null && profileFile.ContentLength != 0)
            {
                profileBlob = await blobOperations.UploadBlob(profileFile);
                obj.ProfileImage = profileBlob.Uri.ToString();
            }
            //Ends Here 
            #endregion

            //Que Storeage executio
            #region Azure web jobs
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["empstorage"]);
            // Create the queue client
            CloudQueueClient queueclinet = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to queue 
            CloudQueue queue = queueclinet.GetQueueReference("quwuw");
            //Create the Queue if it doesn't already exist.

            queue.CreateIfNotExists();
            CloudQueueMessage message = new CloudQueueMessage(profileBlob.Name);
            queue.AddMessage(message);
            #endregion


            #region Save Information in Table Storage
            //Step 2: Save the Information in the Table Storage
            //Get the Original File Size
            obj.RowKey = obj.EmpId.ToString();
            obj.PartitionKey = obj.Email;
            //Save the File in the Table
            tableOperations.CreateEntity(obj);
            //Ends Here 
            #endregion
            return RedirectToAction("Index");
        }
    }
}