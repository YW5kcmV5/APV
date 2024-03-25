using System;
using System.Diagnostics;
using System.Reflection;
using APV.Common.Attributes.Proxy.Attributes;
using APV.Common.Attributes.Proxy.Helpers;
using APV.Common.Attributes.Proxy.Interfaces;

namespace APV.Common.Attributes.Proxy
{
    [ProxyManager]
    public abstract class BaseMarshalProxy : MarshalByRefObject, IProxy
    {
        protected virtual void OnMethodInvoke(IProxyManager manager, MethodBase methodBase)
        {
        }

        protected virtual void OnMethodError(IProxyManager manager, MethodBase methodBase, Exception exception)
        {
        }

        [DebuggerStepThrough]
        void IProxy.OnMethodInvoke(IProxyManager manager, MethodBase methodBase)
        {
            OnMethodInvoke(manager, methodBase);
        }

        [DebuggerStepThrough]
        void IProxy.OnMethodError(IProxyManager manager, MethodBase methodBase, Exception exception)
        {
            OnMethodError(manager, methodBase, exception);
        }

        public static BaseMarshalProxy Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var attribute = type.GetCustomAttribute<ProxyManagerAttribute>();
            IProxyManager proxy = ProxyManager.Create(type, attribute, false);
            return (BaseMarshalProxy)proxy.TransparentProxy;
        }

        public static T Create<T>() where T : BaseMarshalProxy
        {
            return (T) Create(typeof (T));
        }
    }
}