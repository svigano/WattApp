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
using WattApp.Services.Models;

namespace WattApp.Services.Controllers
{
    public class CustomerController : ApiController
    {
        private WattAppContext db = new WattAppContext();

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.Id == id) > 0;
        }
    }
}