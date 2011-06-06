namespace MefContrib.Hosting.Isolation.Runtime.Proxies
{
    using System;

    public interface IProxyFactory
    {
        object CreateProxy(ObjectReference objectReference, Type contractType, Type[] additionalInterfaces);
    }
}