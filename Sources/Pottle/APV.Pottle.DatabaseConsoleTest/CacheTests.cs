using System;
using System.Reflection;
using System.Threading;
using APV.Common.Attributes.Proxy.Attributes;
using APV.Common.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.DatabaseConsoleTest
{
    public class CacheValue
    {
        public CacheValue()
        {
            Thread.Sleep(1);
        }

        public int Id
        {
            get
            {
                return 1;
            }
        }
    }

    public interface ICacheEntity
    {
        CacheValue Value { get; }
    }

    public class CacheStandartEntity : ICacheEntity
    {
        private CacheValue _value;

        public CacheValue Value
        {
            get { return _value ?? (_value = new CacheValue()); }
        }
    }

    public class CacheManualEntity : ICacheEntity
    {
        private PropertyCacheManager _cache;

        protected PropertyCacheManager Cache
        {
            get { return _cache ?? (_cache = new PropertyCacheManager()); }
        }

        public CacheValue Value
        {
            get { return (CacheValue)Cache.GetValue("Value", () => new CacheValue()); }
        }
    }

    public class CacheGeneratedEntityAttribute : BaseWraperAttribute
    {
        public override string GetPropertyGetCode(PropertyInfo property)
        {
            return string.Format("return (CacheValue) Cache.GetValue(\"{0}\", () => new CacheValue());", property.Name);
        }
    }

    [ProxyManager]
    public class CacheGeneratedEntity : ContextBoundObject, ICacheEntity
    {
        private PropertyCacheManager _cache;

        protected PropertyCacheManager Cache
        {
            get { return _cache ?? (_cache = new PropertyCacheManager()); }
        }

        [CacheGeneratedEntity]
        public virtual CacheValue Value
        {
            get { return null; }
        }
    }

    public static class CacheTests
    {
        private static void MainPerformanceTest(Type type)
        {
            const int count = 1000;
            DateTime begin = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                var entity = (ICacheEntity)Activator.CreateInstance(type);
                for (int j = 0; j < count; j++)
                {
                    CacheValue value = entity.Value;
                    Assert.IsNotNull(value);
                    int id = value.Id;
                    Assert.IsNotNull(id);
                }
            }
            var processing = (int)(DateTime.Now - begin).TotalMilliseconds;
            Console.WriteLine("MainPerformanceTest. {0}. {1} mlsec.", type.Name, processing);
        }

        public static void Execute()
        {
            try
            {
                MainPerformanceTest(typeof(CacheStandartEntity));
                MainPerformanceTest(typeof(CacheManualEntity));
                MainPerformanceTest(typeof(CacheGeneratedEntity));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
