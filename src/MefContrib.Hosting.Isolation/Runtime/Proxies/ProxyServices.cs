using System;
using System.Reflection;
using MefContrib.Hosting.Isolation.Runtime.Activation;
using MefContrib.Hosting.Isolation.Runtime.Activation.Hosts;

namespace MefContrib.Hosting.Isolation.Runtime.Proxies
{
    public static class ProxyServices
    {
        public static object InvokeMember(ObjectReference objectReference, MethodInfo methodInfo, object[] arguments)
        {
            if (objectReference == null)
            {
                throw new ArgumentNullException("objectReference");
            }

            if (objectReference.Faulted)
            {
                throw new InvokeException("Given object is faulted.");
            }

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

                RemotingServices.CloseActivator(remoteActivator);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            HandleHostException(exception, objectReference.Description, objectReference);

            return returnValue;
        }

        public static void HandleHostException(Exception exception, ActivationHostDescription description, ObjectReference reference = null)
        {
            if (exception != null)
            {
                // first check if we have the connectivity with the activator host
                var remoteActivator = ActivationHost.GetActivationHost(description).GetActivator();

                try
                {
                    remoteActivator.HeartBeat();
                    RemotingServices.CloseActivator(remoteActivator);
                }
                catch (Exception)
                {
                    // mark object as faulted
                    if (reference != null)
                    {
                        reference.Faulted = true;
                    }
                    
                    var host = ActivationHost.GetActivationHost(description);
                    var activationHostBase = host as PartActivationHostBase;
                    if (activationHostBase != null)
                    {
                        // we have a serious problem - plugin host has crashed
                        if (activationHostBase.Faulted == false)
                        {
                            activationHostBase.Faulted = true;
                            PartHost.OnFailure(host);
                        }
                    }
                    else
                    {
                        PartHost.OnFailure(host);
                    }
                }

                throw new InvokeException("Error while executing remote method.", exception);
            }
        }
    }
}