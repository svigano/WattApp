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
using WattApp.data.Repositories;

namespace WattApp.api.Controllers
{
    public class CustomerController : ApiController
    {
        private IDataRepository _dataRep = null;

        private CloudQueue _configRequestQueue;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public const int kMockCustomerId = 1000000;

        public CustomerController()
        {
            _dataRep = new DataRepository(new WattAppContext());
            _initializeStorage();
        }

        // GET api/Customer
        public IQueryable<CustomerModel> GetCustomers()
        {
            List<CustomerModel> customers = new List<CustomerModel>();

            foreach (var item in _dataRep.Customers)
                customers.Add(new CustomerModel { Id = item.Id, Name = item.Name, Guid = item.Guid, Enabled = item.Enabled});

            return customers.AsQueryable<CustomerModel>();
        }

        // GET api/Customer/5
        [ResponseType(typeof(CustomerModel))]
        public IHttpActionResult GetCustomer(int id)
        {
            Customer item = _dataRep.Customers.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(new CustomerModel { Id = item.Id, Name = item.Name, Guid = item.Guid, Enabled = item.Enabled });
        }

        [Route("api/customer/{id}/discover")]
        public IHttpActionResult DiscoverCustomer(int id)
        {
            Customer c = _dataRep.Customers.Find(id);
            if (c == null)
            {
                return NotFound();
            }
            else
            {
                // Enable Customer
                if (!c.Enabled)
                {
                    c.Enabled = true;
                    _dataRep.Update(c);
                }
                // Create a messageQueue
                var customerInfo = new CustomerDiscoverQueueInfo() { Id = c.Id, Guid = c.Guid, DiscoveryOption = DiscoveryOption.eMeterAndSamples };
                try
                {
                    var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(customerInfo));
                    _configRequestQueue.AddMessage(queueMessage);
                }
                catch (Microsoft.WindowsAzure.Storage.StorageException e)
                {
                    _logger.Error("DiscoverCustomer", e);
                }
                _logger.Debug(string.Format("Created queue message for customer {0}", customerInfo.Guid));
            }

            return Ok();
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
            return _dataRep.Customers.Count(e => e.Id == id) > 0;
        }
    }
}