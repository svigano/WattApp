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
using WattApp.data.Webjobs;

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
                var taskList = new List<ITask>();
                taskList.Add(new DailyPeaksTask(_logger, dataRep));
                taskList.Add(new DailyConsumptionTask(_logger, dataRep));

                foreach (var item in taskList)
                    item.Execute();

                //var rollupManager = new RollUpManager(_logger, dataRep);
                //var enabledEquipmentByCustomerMap = DataHelpers.FindEnabledEquipment(dataRep);
                //rollupManager.CalculateDailyPeaks(enabledEquipmentByCustomerMap, day);
                //rollupManager.CalculateDailyConsumption(enabledEquipmentByCustomerMap, day);
            }
            catch (Exception e)
            {
                _logger.Error("DailyUpdates WebJob -> Unhandle Exception ", e);
            }
            string str = string.Format("DailyUpdatesWebJob Elapsed={0} ms", stopWatch.ElapsedMilliseconds);
            _logger.Info(str);
            Trace.WriteLine(str);
            Thread.Sleep(2000);
        }
    }
}
