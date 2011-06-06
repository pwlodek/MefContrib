using System;
using System.Reflection;
using MefContrib.Hosting.Isolation.Runtime.Activation;

namespace MefContrib.Hosting.Isolation.Runtime.Proxies
{
    public static class ProxyServices
    {
        public static object InvokeMember(ObjectReference objectReference, MethodInfo methodInfo, object[] arguments)
        {
            if (methodInfo.DeclaringType == typeof(IObjectReferenceAware))
            {
                return objectReference;
            }

            IRemoteActivator remoteActivator = ActivationHost.GetActivator(objectReference);
            object returnValue = null;
            Exception exception = null;
            try
            {
                returnValue = remoteActivator.InvokeMember(
                    objectReference,
                    methodInfo.Name,
                    SerializationServices.Serialize(arguments));
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                // first check if we have the connectivity with the activator host
                remoteActivator = ActivationHost.GetActivator(objectReference);

                try
                {
                    remoteActivator.HeartBeat();
                }
                catch (Exception)
                {
                    // we have a serious problem - plugin host has crashed
                    PartHost.OnFailure(ActivationHost.GetActivatorHost(objectReference));
                }
            }

            RemotingServices.CloseActivator(remoteActivator);

            if (exception != null)
            {
                throw new InvokeException("Error while executing remote method.", exception);
            }

            return returnValue;
        }
    }
}