using System;
using System.Timers;
using APV.EntityFramework;
using APV.EntityFramework.Interfaces;

namespace APV.Pottle.Core.BusinessLogic.Statistics
{
    public static class StatisticsController
    {
        private static readonly object Lock = new object();
        private static bool _inited;
        private static Timer _timer;
        private static readonly IContextManager ContextManager = EntityFrameworkManager.GetContextManager();

        private static void Callback(object state, ElapsedEventArgs args)
        {
            try
            {
                lock (Lock)
                {
                    StatisticsSearchController.Update();
                }
            }
            catch (Exception ex)
            {
                //TODO: log
            }
        }

        public static void Init()
        {
            lock (Lock)
            {
                if (!_inited)
                {
                    _inited = true;
                    int statisticsUpdateTimeoutInSeconds = ContextManager.GetStatisticsUpdateTimeoutInSeconds();
                    _timer = new Timer();
                    _timer.Elapsed += Callback;
                    _timer.Interval = 1000 * statisticsUpdateTimeoutInSeconds;
                    _timer.Enabled = true;
                }
            }
        }
    }
}