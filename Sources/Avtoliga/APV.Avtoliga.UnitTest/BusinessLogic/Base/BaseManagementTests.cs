using System.Reflection;
using APV.Avtoliga.Core.Application;
using APV.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Avtoliga.UnitTest.BusinessLogic.Base
{
    [TestClass]
    public abstract class BaseManagementTests
    {
        public static readonly object Lock = new object();

        static BaseManagementTests()
        {
            Assembly dataLayerAssembly = typeof(ContextManager).Assembly;
            EntityFrameworkManager.Register(dataLayerAssembly);
            TestManager.Login();
        }
    }
}