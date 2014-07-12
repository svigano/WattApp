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
        // Equipment
        IQueryable<Point> GetAllPointsByEquipment(int equiId);

        // Samples
        bool Insert(IEnumerable<Sample> list);
    }
}
