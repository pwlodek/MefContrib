using System;
using System.Collections.Generic;

namespace MefContrib.Hosting.Isolation.Runtime.Proxies
{
    public class LinFuProxyFactory : IProxyFactory
    {
        private static readonly LinFu.DynamicProxy.ProxyFactory Factory = new LinFu.DynamicProxy.ProxyFactory();

        public static readonly IProxyFactory Default = new LinFuProxyFactory();

        public object CreateProxy(ObjectReference objectReference, Type contractType, Type[] additionalInterfacesInterfaces)
        {
            var proxy = new LinFuProxy(objectReference);
            var interfaces = new List<Type>(additionalInterfacesInterfaces);
            interfaces.Add(typeof(IObjectReferenceAware));

            return Factory.CreateProxy(contractType, proxy, interfaces.ToArray());
        }
    }
}