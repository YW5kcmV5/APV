using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common.Timers;

namespace APV.Pottle.UnitTest.System
{
    [TestClass]
    public class WatchdogTimerTests
    {
        [TestMethod]
        public void ExecuteTest()
        {
            const int delay = 100;

            bool executed = false;

            Action action = () => { executed = true; };

            using (new WatchdogTimer(delay, action))
            {
                Thread.Sleep(2*delay);
                Assert.IsTrue(executed);
            }
        }

        [TestMethod]
        public void NotExecuteTest()
        {
            const int delay = 100;

            bool executed = false;

            Action action = () => { executed = true; };

            using (new WatchdogTimer(delay, action))
            {
                Thread.Sleep(delay/2);
            }

            Assert.IsFalse(executed);
        }

        [TestMethod]
        public void ResetExecuteTest()
        {
            const int delay = 100;
            const int count = 10;

            bool executed = false;

            Action action = () => { executed = true; };

            using (var timer = new WatchdogTimer(delay, action))
            {
                for (int i = 0; i < count; i++)
                {
                    Thread.Sleep(delay/2);
                    timer.Reset();
                }
            }

            Assert.IsFalse(executed);
        }
    }
}