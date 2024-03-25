using System;
using APV.Common.Attributes.Proxy.Attributes;

namespace APV.Common.Attributes.Proxy.Interfaces
{
    public interface IProxyManager
    {
        Type SourceType { get; }

        Type WrappedType { get; }

        ProxyManagerAttribute ProxyAttribute { get; }

        MarshalByRefObject Instance { get; }

        MarshalByRefObject TransparentProxy { get; }
    }
}