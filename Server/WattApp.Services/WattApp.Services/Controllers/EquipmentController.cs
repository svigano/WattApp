using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WattApp.api.Models;
using WattApp.data.Models;
using WattApp.data.Repositories;

namespace WattApp.api.Controllers
{
    public class EquipmentController : ApiController
    {
        private IDataRepository _dataRep = null;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public EquipmentController()
        {
            _dataRep = new DataRepository(new WattAppContext());
        }

        [Route("api/customer/{customerId}/Equipment")]
        public IQueryable<EquipmentModel> GetEquipment(int customerId)
        {
            var equipmentList = new List<EquipmentModel>();
            var ditem = new DashboardItemModel();

            // TEMPORARY 
            // MOCK DATA
            if (customerId != CustomerController.kMockCustomerId)
            {
                var equipment = _dataRep.Equipment.Where(e => e.Customer.Id == customerId).ToList();
                foreach (var meter in equipment)
                    equipmentList.Add(_mapEquipmentToEquipmentModel(meter));
            }

            return equipmentList.AsQueryable<EquipmentModel>();
        }

        [Route("api/customer/{customerId}/Equipment/{id}")]
        public IHttpActionResult GetEquipment(int customerId, int id)
        {
            var equipment = _dataRep.Equipment.Find(id);
            if (customerId == CustomerController.kMockCustomerId || equipment == null)
                return NotFound();
            else
                return Ok(_mapEquipmentToEquipmentModel(equipment));
        }

        [Route("api/customer/{customerId}/Equipment")]
        public IHttpActionResult DeleteEquipment(int customerId)
        {
            if (customerId != CustomerController.kMockCustomerId)
            {
                // TMP Disable Customer
                var c = _dataRep.Customers.Find(customerId);
                if (c != null)
                {
                    c.Enabled = false;
                    _dataRep.Update(c);
                }
                var equipList = _dataRep.Equipment.Where(e => e.Customer.Id == customerId).ToList();
                if (equipList == null)
                    return NotFound();
                foreach (var item in equipList)
                    _dataRep.Delete(item);
            }

            return Ok();
        }

        private EquipmentModel _mapEquipmentToEquipmentModel(Equipment meter)
        {
            return new EquipmentModel { Id = meter.id, Name = meter.Name, Location = meter.Location, PxGuid = meter.PxGuid };
        }

    }
}
