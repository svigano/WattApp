using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Repositories;

namespace WattApp.data.Webjobs
{
    public interface ITask
    {
        void Execute();
    }
    public abstract class Task : ITask
    {
        private readonly string _taskName;
        protected readonly Logger _logger;
        protected readonly IDataRepository _dataRep;

        public Task(Logger logger, IDataRepository rep, string taskName)
        {
            _logger = logger;
            _dataRep = rep;
            _taskName = taskName;
        }
        public void Execute()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            bool taskSuccess = true;
            _logger.Info(string.Format("****{0}****", _taskName));

            try
            {
                taskSuccess = DoWork();
            }
            catch (Exception e)
            {
                taskSuccess = false;
                _logger.Error(string.Format("Unhandle Exception in task {0}", _taskName), e);
            }
            _logger.Info(string.Format("Task={0}, Success={1}, Elapsed={2} ms", _taskName, taskSuccess, stopWatch.ElapsedMilliseconds));
        }
        public abstract bool DoWork();
    }
}
