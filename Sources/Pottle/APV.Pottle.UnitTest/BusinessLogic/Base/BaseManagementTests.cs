using System.Reflection;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic.Base
{
    [TestClass]
    public abstract class BaseManagementTests
    {
        public static readonly object Lock = new object();

        static BaseManagementTests()
        {
            Assembly dataLayerAssembly = typeof(BaseDataLayerManager<,>).Assembly;
            ApplicationManager.Register(dataLayerAssembly);

            TestManager.Login();
        }
    }
}