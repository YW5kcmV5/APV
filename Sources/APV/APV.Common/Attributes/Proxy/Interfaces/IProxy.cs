using System;
using System.Reflection;

namespace APV.Common.Attributes.Proxy.Interfaces
{
    public interface IProxy
    {
        void OnMethodInvoke(IProxyManager manager, MethodBase methodBase);

        void OnMethodError(IProxyManager manager, MethodBase methodBase, Exception exception);
    }
}