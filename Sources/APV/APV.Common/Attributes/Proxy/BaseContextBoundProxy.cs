using System;
using System.Reflection;
using APV.Common.Attributes.Proxy.Attributes;
using APV.Common.Attributes.Proxy.Interfaces;

namespace APV.Common.Attributes.Proxy
{
    /// <summary>
    /// Does not support generic types (templates)
    /// </summary>
    [ProxyManager]
    public abstract class BaseContextBoundProxy : ContextBoundObject, IProxy
    {
        protected virtual void OnMethodInvoke(IProxyManager manager, MethodBase methodBase)
        {
        }

        public void OnMethodError(IProxyManager manager, MethodBase methodBase, Exception exception)
        {
        }

        #region IProxy

        void IProxy.OnMethodInvoke(IProxyManager manager, MethodBase methodBase)
        {
            OnMethodInvoke(manager, methodBase);
        }

        void IProxy.OnMethodError(IProxyManager manager, MethodBase methodBase, Exception exception)
        {
            OnMethodError(manager, methodBase, exception);
        }

        #endregion
    }
}