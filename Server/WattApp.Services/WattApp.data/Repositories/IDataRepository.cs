using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;

namespace WattApp.data.Repositories
{
    public interface IDataRepository
    {
        // Customers

        // Equipment
        IQueryable<Point> GetAllPointsByEquipment(int equiId);

        // Samples
        Sample GetLastSampleByPoint(int pointID);

        bool Insert(IEnumerable<Sample> list);
    }
}
