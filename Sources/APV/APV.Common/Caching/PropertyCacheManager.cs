using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace APV.Common.Caching
{
    public sealed class PropertyCacheManager : IDisposable
    {
        private class CacheInfo : IDisposable
        {
            public readonly object Lock = new object();
            public readonly object AssignmentLock = new object();
            public object Value;
            public bool DisposeWhenClear;
            public DateTime LastClearOperation = DateTime.Now;
            public DateTime SetOperation = DateTime.Now;
            public bool Inited;
            public bool InProcess;

            public object LastValue;
            public bool UseLastValue;
            public bool LastValueInited;

            private bool _disposed;

            public void Clear()
            {
                object valueToDispose = null;
                bool disposeWhenClear = false;

                lock (AssignmentLock)
                {
                    LastClearOperation = DateTime.Now;
                    if (Inited)
                    {
                        if (UseLastValue)
                        {
                            valueToDispose = LastValue;
                            LastValue = Value;
                            LastValueInited = true;
                        }
                        else
                        {
                            valueToDispose = Value;
                        }
                        disposeWhenClear = DisposeWhenClear;
                        Value = null;
                        Inited = false;
                    }
                }

                if ((disposeWhenClear) && (!ReferenceEquals(valueToDispose, null)))
                {
                    var disposable = valueToDispose as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                    else
                    {
                        CacheableUtility.Clear(valueToDispose);
                    }
                }
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    Clear();
                }
                GC.SuppressFinalize(this);
            }
        }

        private readonly object _lock = new object();
        private readonly SortedList<int, CacheInfo> _cache = new SortedList<int, CacheInfo>();
        private bool _disposed;

        private CacheInfo GetCacheInfo(int propertyHash, bool disposeWhenClear, bool useLastValue = false)
        {
            lock (_lock)
            {
                int index = _cache.IndexOfKey(propertyHash);
                CacheInfo cache;
                if (index != -1)
                {
                    cache = _cache.Values[index];
                }
                else
                {
                    cache = new CacheInfo { DisposeWhenClear = disposeWhenClear, UseLastValue = useLastValue };
                    _cache.Add(propertyHash, cache);
                }
                return cache;
            }
        }

        private CacheInfo GetCacheIfExists(int propertyHash)
        {
            lock (_lock)
            {
                int index = _cache.IndexOfKey(propertyHash);
                return (index != -1) ? _cache.Values[index] : null;
            }
        }

        public delegate object Calc();

        [DebuggerStepThrough]
        public object GetValue(int propertyNameHash, bool disposeWhenClear, int liveTimeInSeconds, bool clone, bool useLastValue, Calc @delegate)
        {
            if (_disposed)
            {
                Clear(propertyNameHash);
                //throw new InvalidOperationException("The object already disposed.");
            }

            CacheInfo cache = GetCacheInfo(propertyNameHash, disposeWhenClear, useLastValue);

            object value = null;
            bool haveToUseValue = true;

            lock (cache.AssignmentLock)
            {
                if ((cache.InProcess) && (cache.UseLastValue) && (cache.LastValueInited))
                {
                    value = cache.LastValue;
                    haveToUseValue = false;
                }
            }

            if (haveToUseValue)
            {
                lock (cache.Lock)
                {
                    value = cache.Value;
                    if ((cache.Inited) && (liveTimeInSeconds > 0))
                    {
                        DateTime now = DateTime.Now;
                        if ((now - cache.SetOperation).TotalSeconds > liveTimeInSeconds)
                        {
                            cache.Clear();
                            value = null;
                        }
                    }
                    if (!cache.Inited)
                    {
                        lock (cache.AssignmentLock)
                        {
                            cache.InProcess = true;
                        }

                        DateTime now;
                        do
                        {
                            now = DateTime.Now;
                            try
                            {
                                value = @delegate.Invoke();
                            }
                            catch
                            {
                                lock (cache.AssignmentLock)
                                {
                                    cache.InProcess = false;
                                }
                                throw;
                            }
                        } while ((now < cache.LastClearOperation) && (!_disposed));

                        lock (cache.AssignmentLock)
                        {
                            cache.Value = value;
                            cache.SetOperation = now;
                            cache.Inited = true;
                            cache.InProcess = false;
                        }
                    }
                    if (_disposed)
                    {
                        cache.Clear();
                    }
                }
            }

            if (clone)
            {
                lock (cache.AssignmentLock)
                {
                    value = CacheableUtility.Clone(value);
                }
            }

            return value;
        }

        public object GetValue(string propertyName, Calc @delegate)
        {
            return GetValue(propertyName.GetHashCode(), true, 0, false, false, @delegate);
        }

        public object GetValue(string propertyName, bool disposeWhenClear, bool clone, Calc @delegate)
        {
            return GetValue(propertyName.GetHashCode(), disposeWhenClear, 0, clone, false, @delegate);
        }

        public object GetValue(string propertyName, bool disposeWhenClear, bool clone, bool useLastValue, Calc @delegate)
        {
            return GetValue(propertyName.GetHashCode(), disposeWhenClear, 0, clone, useLastValue, @delegate);
        }

        public object GetValue(string propertyName, bool disposeWhenClear, Calc @delegate)
        {
            return GetValue(propertyName.GetHashCode(), disposeWhenClear, 0, false, false, @delegate);
        }

        public object GetValue(string propertyName, bool disposeWhenClear, int liveTimeInSeconds, Calc @delegate)
        {
            return GetValue(propertyName.GetHashCode(), disposeWhenClear, liveTimeInSeconds, false, false, @delegate);
        }

        public object GetValue(string propertyName, int liveTimeInSeconds, Calc @delegate)
        {
            return GetValue(propertyName.GetHashCode(), true, liveTimeInSeconds, false, false, @delegate);
        }

        public object GetValue(string propertyName, bool disposeWhenClear, int liveTimeInSeconds, bool clone, Calc @delegate)
        {
            return GetValue(propertyName.GetHashCode(), disposeWhenClear, liveTimeInSeconds, clone, false, @delegate);
        }

        public T GetValue<T>(Expression<Func<T>> expression, Func<T> method)
        {
            string propertyName = expression.ExtractName();
            return (T) GetValue(propertyName.GetHashCode(), true, 0, false, false, () => (object) method());
        }

        private static void ProcessClear(IEnumerable<CacheInfo> items)
        {
            foreach (CacheInfo item in items)
            {
                item.Clear();
            }
        }

        public void Clear()
        {
            CacheInfo[] items;
            lock (_lock)
            {
                items = _cache.Values.ToArray();
            }
            ProcessClear(items);
        }

        public void Clear(string propertyName)
        {
            Clear(propertyName.GetHashCode());
        }

        public void Clear(int propertyNameHash)
        {
            CacheInfo cache = GetCacheIfExists(propertyNameHash);
            cache?.Clear();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                CacheInfo[] items;
                lock (_lock)
                {
                    items = _cache.Values.ToArray();
                    _cache.Clear();
                }
                ProcessClear(items);
            }
            GC.SuppressFinalize(this);
        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            var body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }
    }
}
