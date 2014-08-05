using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;

namespace WattApp.data.Repositories
{
    public interface IDataRepository
    {
        // Customers
        IDbSet<Customer> Customers { get; }
        void Insert(IEnumerable<Customer> list);
        void Update(Customer c);

        // Equipment
        IDbSet<Equipment> Equipment { get; }
        IQueryable<Point> GetAllPointsByEquipment(int equiId);
        void Insert(IEnumerable<Equipment> list);
        void Update(Equipment e);
        
        // Points
        IDbSet<Point> Points { get; }

        // Samples
         IDbSet<Sample> Samples { get; }
        Sample GetLastSampleByPoint(int pointID, SampleType t);
        Sample GetSampleByClosestTimeStamp(int pointID, DateTime startTime, SampleType t);
        Sample GetSampleByTimeStamp(int pointID, DateTime time, SampleType t);
        void Insert(IEnumerable<Sample> list);
        void Insert(Sample s);
        void Update(Sample s);

    }
}
