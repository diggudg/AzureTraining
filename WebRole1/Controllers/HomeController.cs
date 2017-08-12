using EmpDetails;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebRole1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            EmpDetailsInfo emp = new EmpDetailsInfo();
            return View(emp);
        }

        [HttpPost]
        public ActionResult Create(EmpDetailsInfo emp)
        {
            const string QueueName = "empQueue";
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var nameSpaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!nameSpaceManager.QueueExists(QueueName))
            {
                nameSpaceManager.CreateQueue(QueueName);
            }

            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(connectionString);
            QueueClient myQ = factory.CreateQueueClient(QueueName);

            BrokeredMessage bm = new BrokeredMessage(emp);

            myQ.Send(bm);

            ViewBag.Status = "Message Send";


            return View();
        }

        public ActionResult List()
        {
            return View();
        }
    }
}