using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WattApp.data.Models;
using WattApp.api.Models;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using NLog;
using WattApp.data.Webjobs;
using Newtonsoft.Json;

namespace WattApp.api.Controllers
{
    public class CustomerController : ApiController
    {
        private WattAppContext db = new WattAppContext();
        private CloudQueue _configRequestQueue;
        private static Logger _logger = LogManager.GetCurrentClassLogger();


        public CustomerController()
        {
            _initializeStorage();
        }

        // GET api/Customer
        public IQueryable<CustomerModel> GetCustomers()
        {
            var r = db.Customers;
            List<CustomerModel> customers = new List<CustomerModel>();

            foreach (var item in r)
                customers.Add(new CustomerModel { Id = item.Id, Name = item.Name, Guid = item.Guid, Enabled = item.Enabled});

            Console.WriteLine(r.Count());
            return customers.AsQueryable<CustomerModel>();
        }

        // GET api/Customer/5
        [ResponseType(typeof(CustomerModel))]
        public IHttpActionResult GetCustomer(int id)
        {
            Customer item = db.Customers.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(new CustomerModel { Id = item.Id, Name = item.Name, Guid = item.Guid, Enabled = item.Enabled });
        }

        public void PutCustomer(int id, [FromBody]CustomerModel c)
        {
            if (c != null)
            {
                Trace.WriteLine(string.Format("-----------> id: {0}, name: {1}, guid: {2}, enable: {3}", c.Id, c.Name, c.Guid, c.Enabled));
                var customerInfo = new CustomerQueueInfo() { Id = c.Id, Name = c.Name, Guid = c.Guid, Enabled = c.Enabled};
                var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(customerInfo));
                try
                {
                    _configRequestQueue.AddMessage(queueMessage);
                }
                catch (Microsoft.WindowsAzure.Storage.StorageException e)
                {
                    Trace.TraceError(e.Message);
                }
                _logger.Debug(string.Format("Created queue message for customer {0}", customerInfo.Guid));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void _initializeStorage()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureJobsStorage"].ToString());

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Get a reference to the queue.
            _configRequestQueue = queueClient.GetQueueReference("wattappconfigrequest");
            _configRequestQueue.CreateIfNotExists();
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.Id == id) > 0;
        }
    }
}