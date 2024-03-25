using System;
using System.Reflection;
using APV.EntityFramework;
using APV.Common.Attributes.Proxy;
using APV.Common.Attributes.Proxy.Interfaces;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.DatabaseConsoleTest
{
    public abstract class BaseMarshalInstance<T> : BaseMarshalProxy
    {
        void CheckAccess(MethodBase method)
        {
            Console.WriteLine($"{nameof(BaseMarshalInstance<T>)}.{nameof(CheckAccess)}.{method.Name}");
            if (method.Name == "MethodWithException")
            {
                throw new InvalidOperationException("MethodWithException");
            }
        }

        protected override void OnMethodInvoke(IProxyManager manager, MethodBase methodBase)
        {
            Console.WriteLine($"{nameof(BaseMarshalInstance<T>)}.{nameof(OnMethodInvoke)}.{methodBase.Name}");
            CheckAccess(methodBase);
        }
    }

    public abstract class BaseContextBoundInstance : BaseContextBoundProxy
    {
        void CheckAccess(MethodBase method)
        {
            Console.WriteLine($"{nameof(BaseContextBoundInstance)}.{nameof(CheckAccess)}.{method.Name}");
            if (method.Name == "MethodWithException")
            {
                throw new InvalidOperationException("MethodWithException");
            }
        }

        protected override void OnMethodInvoke(IProxyManager manager, MethodBase methodBase)
        {
            Console.WriteLine($"{nameof(BaseContextBoundInstance)}.{nameof(OnMethodInvoke)}.{methodBase.Name}");
            CheckAccess(methodBase);
        }
    }

    public class TestMarshalInstance : BaseMarshalInstance<int>
    {
        public virtual int Id { get; internal set; }

        public virtual void Method()
        {
        }

        public virtual void MethodWithException()
        {
        }
    }

    public class TestContextBoundInstance : BaseContextBoundInstance
    {
        public virtual int Id { get; internal set; }

        public virtual void Method()
        {
        }

        public virtual void MethodWithException()
        {
        }
    }

    public static class ProxyTests
    {
        private static void ManagementTest()
        {
            var management = (UserManagement)EntityFrameworkManager.GetManagement<UserEntity>();
            management.Login("1", "2");
        }

        private static void MarshalProxyTest()
        {
            //var instance0 = InternalProxy.Create<TestInstance>();
            //int id0 = instance0.Id;
            //instance0.Method();

            var instance = BaseMarshalProxy.Create<TestMarshalInstance>();
            //var instance = new TestMarshalInstance();
            instance.Id = 11;
            int id = instance.Id;
            instance.Method();
            //instance.MethodWithException();
        }

        private static void ContextBoundProxyTest()
        {
            //var instance0 = InternalProxy.Create<TestInstance>();
            //int id0 = instance0.Id;
            //instance0.Method();

            var instance = new TestContextBoundInstance();
            instance.Id = 11;
            int id = instance.Id;
            instance.Method();

            //instance.MethodWithException();
        }

        public static void Execute()
        {
            try
            {
                MarshalProxyTest();
                ContextBoundProxyTest();
                ManagementTest();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}