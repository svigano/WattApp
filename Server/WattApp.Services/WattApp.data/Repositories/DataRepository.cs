using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;

namespace WattApp.data.Repositories
{
    public class DataRepository : IDataRepository
    {
        // TO DO
        // Add logentries
        public WattAppContext _ctxDb = null;

        public DataRepository()
        {
            _ctxDb = new WattAppContext();
        }
        public DataRepository(WattAppContext c)
        {
            _ctxDb = c;
        }

        public WattAppContext ctxDb { get { return _ctxDb; } }

        public IDbSet<Customer> Customers { get { return _ctxDb.Customers; } }

        public void Update(Customer c)
        {
            _ctxDb.Entry(c).State = EntityState.Modified;
            _ctxDb.SaveChanges();
        }

        public void Insert(IEnumerable<Customer> list)
        {
            _ctxDb.Customers.AddRange(list);
            _ctxDb.SaveChanges();
        }

        public IDbSet<Equipment> Equipment { get { return _ctxDb.Equipment; } }

        public void Insert(IEnumerable<Equipment> list)
        {
            _ctxDb.Equipment.AddRange(list);
            _ctxDb.SaveChanges();
        }

        public void Update(Equipment e)
        { 
            _ctxDb.Entry(e).State = EntityState.Modified;
            _ctxDb.SaveChanges();
        }

        public void Delete(Equipment e)
        {
            var entityToBeDeleted = _ctxDb.Equipment.Find(e.id);
            if (entityToBeDeleted != null)
            {
                _ctxDb.Equipment.Remove(entityToBeDeleted);
                _ctxDb.SaveChanges();
            }
        }


        public IQueryable<Point> GetAllPointsByEquipment(int equiId)
        { 
            return _ctxDb.Equipment.SelectMany(c => c.PointsList).Where(o => o.id == equiId);
        }
        public IDbSet<Point> Points { get { return _ctxDb.Points; } }

        public IDbSet<Sample> Samples { get { return _ctxDb.Samples; } }
        public Sample GetLastSampleByPoint(int pointID, SampleType type)
        {
            var lastSample = from n in _ctxDb.Samples
                            where n.Point.id == pointID && n.SampleType == type
                            group n by n.Point.id into g
                            select g.OrderByDescending(t=>t.TimeStamp).FirstOrDefault();

            return lastSample.FirstOrDefault();
        }
        public Sample GetSampleByClosestTimeStamp(int pointID, DateTime startTime, SampleType type)
        {

            var selected = _ctxDb.Samples.Where(n => n.Point.id == pointID && n.SampleType == type)
                           .OrderBy(n => Math.Abs(System.Data.Entity.DbFunctions.DiffSeconds(startTime, n.TimeStamp).Value));

            return selected.FirstOrDefault();
        }

        public Sample GetSampleByTimeStamp(int pointID, DateTime timeStamp, SampleType type)
        {

            var selected = _ctxDb.Samples.Where(n => n.Point.id == pointID && n.SampleType == type && n.TimeStamp == timeStamp);
            return selected.FirstOrDefault();
        }

        public void Insert(IEnumerable<Sample> list) 
        {
            var mylist = new List<Sample>(list);
            _ctxDb.Samples.AddRange(list);
            _ctxDb.SaveChanges();
        }

        public void Insert(Sample sample)
        {
            var tSample = GetSampleByTimeStamp(sample.PointId, sample.TimeStamp, sample.SampleType);
            if (tSample != null)
            {
                tSample.Value = sample.Value; 
                Update(tSample);
            }
            else
            {
                _ctxDb.Samples.Add(sample);
                _ctxDb.SaveChanges();
            }
        }
        public void Update(Sample sample)
        {
            _ctxDb.Entry(sample).State = EntityState.Modified;
            _ctxDb.SaveChanges();
        }

    }
}
