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
using WattApp.api.Models;
using WattApp.data.Models;
using NLog;
using System.Diagnostics;

namespace WattApp.api.Controllers
{
    public class DashboardController : ApiController
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private WattAppContext _db = new WattAppContext();
        private Stopwatch _stopwatch = new Stopwatch(); 

        // GET api/Dashboard
        [Route("api/customer/{customerGuid}/Dashboard")]
        //[Route("customer/{customerId}/orders")]
        public IQueryable<DashboardItemModel> GetAllDashboardItem(string customerGuid)
        {
            var dashboarditems = new List<DashboardItemModel>();
            var ditem = new DashboardItemModel();

            // TEMPORARY 
            // MOCK DATA
            if (customerGuid == "123mock123")
                dashboarditems = _mockData();
            else
            {
                var equipment = _db.Equipment.Where(e => e.Customer.Guid == customerGuid).ToList();
                foreach (var meter in equipment)
                    dashboarditems.Add(_mapEquipmentToDashboardItem(meter));
            }

            return dashboarditems.AsQueryable<DashboardItemModel>();
        }

        [Route("api/customer/{customerGuid}/Dashboard/{id}")]
        public IHttpActionResult GetDashboardItemModel(string customerGuid, int id)
        {
            var dashboarditem = new DashboardItemModel();
            // TEMPORARY 
            // MOCK DATA
            if (customerGuid == "123mock123")
                dashboarditem = _mockData().FirstOrDefault((p) => p.Id == id);
            else
            {
                var equip = _db.Equipment.First(e => e.Customer.Guid == customerGuid && e.id == id);

                if (equip == null)
                {
                    return NotFound();
                }
                dashboarditem = _mapEquipmentToDashboardItem(equip);
            }

            return Ok(dashboarditem);
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
            return new DashboardItemModel { Id = meter.id, Name = meter.Name, Location = meter.Location, Demand = meter.LastDemand, Inc = meter.DeltaDemand, lastUpdate = meter.LastUpdateTime };
        }

        private List<DashboardItemModel> _mockData()
        {
            List<DashboardItemModel> meters = new List<DashboardItemModel>();

            var m = new DashboardItemModel
            {
                Id = 1,
                Location = "507 Michigan",
                Name = "Main utility meter",
                Demand = 340,
                Inc = -2,
                lastUpdate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15))
            };
            meters.Add(m);
            m = new DashboardItemModel
            {
                Id = 2,
                Location = "5757 Corporate",
                Name = "Main meter",
                Demand = 680,
                Inc = .3,
                lastUpdate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(45))
            };
            meters.Add(m);
            m = new DashboardItemModel
            {
                Id = 3,
                Location = "Mke Hangar",
                Name = "Utility meter",
                Demand = 430,
                Inc = 5,
                lastUpdate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15))
           };
            meters.Add(m);
            m = new DashboardItemModel
            {
                Id = 4,
                Location = "Plymouth",
                Name = "Building 36 meter",
                Demand = 298,
                Inc = .2,
                lastUpdate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15))
            };
            meters.Add(m);
            m = new DashboardItemModel
            {
                Id = 5,
                Location = "York",
                Name = "CTU meter",
                Demand = 1200,
                Inc = -2,
                lastUpdate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15))
            };
            meters.Add(m);
            m = new DashboardItemModel
            {
                Id = 2,
                Location = "5757 solar",
                Name = "Roof Array meter",
                Demand = 545,
                Inc = .7,
                lastUpdate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15))
            };
            meters.Add(m);
            m = new DashboardItemModel
            {
                Id = 2,
                Location = "5757 Corporate",
                Name = "Pumps eletric meter",
                Demand = 680,
                Inc = 1.1,
                lastUpdate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30))
            };
            meters.Add(m);
            return meters;
        }
    }
}