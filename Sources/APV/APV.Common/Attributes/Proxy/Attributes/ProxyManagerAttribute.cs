using System;
using System.Runtime.Remoting.Proxies;
using APV.Common.Attributes.Proxy.Helpers;
using APV.Common.Attributes.Proxy.Interfaces;

namespace APV.Common.Attributes.Proxy.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProxyManagerAttribute : ProxyAttribute
    {
        private IProxyManager _proxy;

        public override MarshalByRefObject CreateInstance(Type type)
        {
            _proxy = ProxyManager.Create(type, this, true);
            return _proxy.TransparentProxy;
        }

        public IProxyManager Proxy
        {
            get { return _proxy; }
        }
    }
}