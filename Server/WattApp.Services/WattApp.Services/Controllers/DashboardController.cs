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
using WattApp.Services.Models;
using WattApp.data.Models;
using NLog;
using System.Diagnostics;

namespace WattApp.Services.Controllers
{
    public class DashboardController : ApiController
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private WattAppContext _db = new WattAppContext();
        private Stopwatch _stopwatch = new Stopwatch(); 

        // GET api/Dashboard
        [Route("api/customer/{customerGuid}/Dashboard")]
        //[Route("customer/{customerId}/orders")]
        public IQueryable<DashboardItemModel> GetDashboardItemModels(string customerGuid)
        {
            var dashboarditems = new List<DashboardItemModel>();
            var ditem = new DashboardItemModel();
            var equipment = _db.Equipment.Where(e => e.Customer.Guid == customerGuid).ToList();
            foreach (var meter in equipment)
                dashboarditems.Add(_mapEquipmentToDashboardItem(meter));

            return dashboarditems.AsQueryable<DashboardItemModel>();
        }

        // GET api/Dashboard/5
        [ResponseType(typeof(DashboardItemModel))]
        [Route("api/customers/{customerGuid}/Dashboard/{itemId}")]
        public IHttpActionResult GetDashboardItemModel(string customerGuid, int id)
        {
            Equipment equip = _db.Equipment.Find(id);
            if (equip == null)
            {
                return NotFound();
            }

            return Ok(_mapEquipmentToDashboardItem(equip));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        // TO DO
        // Evaluate to use AuotMapper in the future
        private DashboardItemModel _mapEquipmentToDashboardItem(Equipment meter)
        {
            Random r = new Random();
            return new DashboardItemModel { Id = meter.id, Name = meter.Name, Location = meter.Location, Demand = Math.Round(r.NextDouble() * 100, 2), Inc = Math.Round((r.NextDouble() * 2.0 - 1.0) * 10, 2) };
        }
    }
}