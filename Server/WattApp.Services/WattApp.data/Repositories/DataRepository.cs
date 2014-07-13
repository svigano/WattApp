using System;
using System.Collections.Generic;
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
        private WattAppContext _ctxDb = null;

        public DataRepository()
        {
            _ctxDb = new WattAppContext();
        }
        public DataRepository(WattAppContext c)
        {
            _ctxDb = c;
        }

        public IQueryable<Point> GetAllPointsByEquipment(int equiId)
        { 
            return _ctxDb.Equipment.SelectMany(c => c.Points).Where(o => o.id == equiId);
        }

        public Sample GetLastSampleByPoint(int pointID)
        {
            var lastSample = from n in _ctxDb.Samples
                            where n.Point.id == pointID
                            group n by n.Point.id into g
                            select g.OrderByDescending(t=>t.TimeStamp).FirstOrDefault();

            return lastSample.FirstOrDefault();
        }

        public bool Insert(IEnumerable<Sample> list) 
        {
            var done = true;

            _ctxDb.Samples.AddRange(list);
            _ctxDb.SaveChanges();

            return done;
        }
    }
}
