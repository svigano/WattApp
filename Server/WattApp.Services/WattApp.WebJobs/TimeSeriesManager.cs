using BuildingApi;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;
using WattApp.data.Repositories;
using WattApp.WebJobs.API;

namespace WattApp.WebJobs
{
    internal class TimeSeriesManager
    {
        private const string kEletricMeter = "EletricMeter";
        private const int DEFAULT_BACKFILL_TIME_HOURS = 48;

        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private string _baseAPIRoot = string.Empty;
        private ITokenProvider _tokenProvider;
        private EquipmentClient _equipmentClient;
        private SampleClient _sampleClient;
        private WattAppContext _wattAppDB = new WattAppContext();
        private IDataRepository _dataRep = null;

        public TimeSeriesManager(ITokenProvider provider, string baseRootAPI)
        {
            _tokenProvider = provider;
            _baseAPIRoot = baseRootAPI;
            _equipmentClient = new EquipmentClient(_tokenProvider, _baseAPIRoot);
            _sampleClient = new SampleClient(_tokenProvider, _baseAPIRoot);
            _dataRep = new DataRepository(_wattAppDB);
        }

        public void PullTimeSeriesTask()
        {
            try
            {
                var map = _findEnabledEquipment();
                if (map.Keys.Count == 0)
                    _logger.Info("No Customer/Equipment enabled");
                foreach (var key in map.Keys)
                {
                    _logger.Debug("CustomerGuid {0} # of Equipment {1}", key, map[key].Count);
                    var currentCompany = new Company { Id = key };
                    _pullTimeSeriesDataByCustomer(currentCompany, map[key]);
                }
            }
            catch (Exception e)
            {
                _logger.Error("PullTimeSeriesTask->Unhandle Exception ", e);
            }

        }

        private void _pullTimeSeriesDataByCustomer(Company c, IList<data.Models.Equipment> eqList)
        { 
            var ptIds = new List<string>();

            foreach (var equip in eqList)
            {
                _logger.Debug("Equipment {0} # of Points {1}", equip.Name, equip.Points.Count());
                foreach (var item in equip.Points)
                {
                    DateTime startTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(DEFAULT_BACKFILL_TIME_HOURS));
                    DateTime endTime = DateTime.UtcNow;

                    // Check the latest sample available in wattapp system
                    var wlatestSample = _dataRep.GetLastSampleByPoint(item.id);
                    // New point, let's bring back the default backfill data
                    if (wlatestSample == null)
                    {
                        // Store data in WattApp
                        // TO DO error handling
                        var pxsamples = _sampleClient.GetSamples(item.PxGuid, startTime, endTime, c);
                        var wsamples = _mapTowSample(item, pxsamples);
                        _dataRep.Insert(wsamples);
                        _logger.Info(string.Format("Initial BackFilled stored # {0} samples for the equipment {1} pt: {2}", wsamples.Count(), equip.Name, item.PxGuid));
                    }
                    else 
                    {
                        // Store data in WattApp
                        // TO DO error handling
                        startTime = wlatestSample.TimeStamp;
                        var pxsamples = _sampleClient.GetSamples(item.PxGuid, startTime, endTime, c);
                        var pxLatestBatchSample = pxsamples.OrderByDescending(t => t.Timestamp).FirstOrDefault();
                        if (pxLatestBatchSample != null && pxLatestBatchSample.Timestamp > wlatestSample.TimeStamp)
                        {
                            var sampleTobeInserted = new List<BuildingApi.Sample>(pxsamples);
                            sampleTobeInserted.RemoveAll(x => x.Timestamp <= wlatestSample.TimeStamp);
                            var wsamples = _mapTowSample(item, sampleTobeInserted);
                            _dataRep.Insert(wsamples);
                            _logger.Info(string.Format("Stored # {0} samples for the equipment {1} pt: {2}", wsamples.Count(), equip.Name, item.PxGuid));
                        }
                    }
                }
            }
        }

        private IEnumerable<WattApp.data.Models.Sample> _mapTowSample(data.Models.Point pt, IEnumerable<BuildingApi.Sample> pxsamples)
        {
            var wsamples = new List<WattApp.data.Models.Sample>();

            foreach (var item in pxsamples)
                wsamples.Add(new data.Models.Sample { Point = pt, Value = Math.Round(item.Value,2), TimeStamp = item.Timestamp });   

            return wsamples;
        }

        // TO DO
        // To be refactor in the ?? DataRepository ??
        private Dictionary<string, List<data.Models.Equipment>> _findEnabledEquipment()
        {
            List<data.Models.Equipment> equipList = new List<data.Models.Equipment>();
            var map = new Dictionary<string, List<WattApp.data.Models.Equipment>>();

            var q =
                    from c in _wattAppDB.Equipment
                    join p in _wattAppDB.Customers on c.Customer equals p
                    where c.Customer.Enabled == true
                    select new { Equipment = c, CustomerGuid = p.Guid };


            foreach (var item in q)
            {
                if (!map.ContainsKey(item.CustomerGuid))
                {
                    var l = new List<data.Models.Equipment>();
                    l.Add(item.Equipment);
                    map.Add(item.CustomerGuid, l);
                }
                else
                    map[item.CustomerGuid].Add(item.Equipment);
            }

            //var activeCustomers = _wattAppDB.Customers.Where(c => c.Enabled == true).ToList();
            //foreach (var item in activeCustomers)
            //{
            //    var l = new List<data.Models.Equipment>();
            //    var equip = (_wattAppDB.Equipment.Where(e => e.Customer.Id == item.Id) as ObjectQuery<WattApp.data.Models.Equipment>).Include("Points");
            //    l.AddRange(equip);
            //    map.Add(item.Guid, l);
            //}
            return map;
        }
    }
}
