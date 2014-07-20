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


        public IQueryable<Point> GetAllPointsByEquipment(int equiId)
        { 
            return _ctxDb.Equipment.SelectMany(c => c.PointsList).Where(o => o.id == equiId);
        }
        public IDbSet<Point> Points { get { return _ctxDb.Points; } }

        public IDbSet<Sample> Samples { get { return _ctxDb.Samples; } }
        public Sample GetLastSampleByPoint(int pointID)
        {
            var lastSample = from n in _ctxDb.Samples
                            where n.Point.id == pointID
                            group n by n.Point.id into g
                            select g.OrderByDescending(t=>t.TimeStamp).FirstOrDefault();

            return lastSample.FirstOrDefault();
        }
        public Sample GetSamplesByPoint(int pointID, DateTime startTime)
        {
            //var lastSample = from n in _ctxDb.Samples
            //                 where n.Point.id == pointID && n.TimeStamp >= startTime && n.TimeStamp <= endTime
            //                 select n;

            var selected = _ctxDb.Samples.Where(n => n.Point.id == pointID)
                           .OrderBy(n => Math.Abs(System.Data.Entity.DbFunctions.DiffSeconds(startTime, n.TimeStamp).Value));

            return selected.FirstOrDefault();
        }

        public bool Insert(IEnumerable<Sample> list) 
        {
            var done = true;
            var mylist = new List<Sample>(list);
            _ctxDb.Samples.AddRange(list);
            _ctxDb.SaveChanges();

            return done;
        }
    }
}
