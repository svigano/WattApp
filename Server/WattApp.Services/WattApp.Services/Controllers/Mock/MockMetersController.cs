using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WattApp.api.Controllers
{
    public class Meter 
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public double Demand { get; set; }
        public double Inc { get; set; }
    }

    public class MockMetersController : ApiController
    {
        private List<Meter> _meters = new List<Meter>();
        public MockMetersController()
        {
            var m = new Meter { 
                Id  = 1,
                Location = "507 Michigan",
                Name = "Main utility meter",
                Demand = 340,
                Inc = -2
            };
            _meters.Add(m);
            m = new Meter { 
                Id  = 2,
                Location = "5757 Corporate",
                Name = "Main meter",
                Demand = 680,
                Inc = .3
            };
            _meters.Add(m);
            m = new Meter
            {
                Id = 3,
                Location = "Mke Hangar",
                Name = "Utility meter",
                Demand = 430,
                Inc = 5
            };
            _meters.Add(m);
            m = new Meter
            {
                Id = 4,
                Location = "Plymouth",
                Name = "Building 36 meter",
                Demand = 298,
                Inc = .2
            };
            _meters.Add(m);
            m = new Meter
            {
                Id = 5,
                Location = "York",
                Name = "CTU meter",
                Demand = 1200,
                Inc = -2
            };
            _meters.Add(m);
            m = new Meter
            {
                Id = 2,
                Location = "5757 solar",
                Name = "Roof Array meter",
                Demand = 545,
                Inc = .7
            };
            _meters.Add(m);
            m = new Meter
            {
                Id = 2,
                Location = "5757 Corporate",
                Name = "Pumps eletric meter",
                Demand = 680,
                Inc = 1.1
            };
            _meters.Add(m);
        }

        public IEnumerable<Meter> GetAllMeters()
        {
            return _meters;
        }

        public IHttpActionResult GetMeter(int id)
        {
            var meter = _meters.FirstOrDefault((p) => p.Id == id);
            if (meter == null)
            {
                return NotFound();
            }
            return Ok(meter);
        } 
    }
}
