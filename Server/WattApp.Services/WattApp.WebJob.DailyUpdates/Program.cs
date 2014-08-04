using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WattApp.data.Models;
using WattApp.data.Repositories;

namespace WattApp.WebJob.DailyUpdates
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            IDataRepository dataRep = new DataRepository(new WattAppContext());

            try
            {
                var rollupManager = new RollUpManager(_logger, dataRep);
                var enabledEquipmentByCustomerMap = DataHelpers.FindEnabledEquipment(dataRep);

                var day = new DateTime(2014, 8, 2);
                Console.WriteLine(day);

                rollupManager.CalculateDailyPeaks(enabledEquipmentByCustomerMap, day);

                rollupManager.CalculateDailyConsumption(enabledEquipmentByCustomerMap, day);

            }
            catch (Exception e)
            {
                _logger.Error("WebJob -> Unhandle Exception ", e);
                Trace.WriteLine("WebJob -> Unhandle Exception " + e.Message);
            }
            string str = string.Format("DailyUpdates WebJob executed in (ms) {0}", stopWatch.ElapsedMilliseconds);
            _logger.Info(str);
            Trace.WriteLine(str);
            Thread.Sleep(2000);
            Console.ReadKey();
        }
    }
}
