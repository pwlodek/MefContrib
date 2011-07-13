namespace MefContrib.Hosting.Isolation.Runtime.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using MefContrib.Hosting.Isolation.Runtime.Activation;

    /// <summary>
    /// Default implementation of the <see cref="IRemoteActivator"/> interface.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class RemoteActivator : IRemoteActivator
    {
        private static readonly Dictionary<ObjectReference, object> ObjectMap = new Dictionary<ObjectReference, object>();
        private static readonly HashSet<Assembly> InitializedAssemblies = new HashSet<Assembly>();

        public void HeartBeat()
        {
        }

        public ObjectReference ActivateInstance(ActivationHostDescription description, string assemblyName, string typeName)
        {
            try
            {
                var assembly = Assembly.Load(assemblyName);
                if (InitializedAssemblies.Contains(assembly) == false)
                {
                    var hooks = assembly.GetTypes().Where(t => typeof(ActivationHook).IsAssignableFrom(t));
                    foreach (var hook in hooks)
                    {
                        try
                        {
                            var hookInstance = (ActivationHook)Activator.CreateInstance(hook);
                            hookInstance.Initialize();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    InitializedAssemblies.Add(assembly);
                }

                var type = assembly.GetTypes().Where(t => t.FullName == typeName).FirstOrDefault();
                var instance = Activator.CreateInstance(type);

                var reference = new ObjectReference(description);
                reference.IsDisposable = (instance as IDisposable) != null;
                ObjectMap[reference] = instance;

                return reference;
            }
            catch (Exception exception)
            {
                var message = string.Format("Error while activating part {0} from assembly {1}.", typeName, assemblyName);
                throw new FaultException<RemoteActivatorExceptionDetail>(
                    new RemoteActivatorExceptionDetail(exception) { Description = description },
                    new FaultReason(message));
            }
        }

        public InvokeReturnValue InvokeMember(ObjectReference objectReference, string name, List<InvokeArgument> arguments)
        {
            try
            {
                var obj = ObjectMap[objectReference];
                var type = obj.GetType();
                var methodInfo = type.GetMethod(name);
                var args = SerializationServices.Deserialize(arguments);
                var retVal = methodInfo.Invoke(obj, args.ToArray());

                var returnValue = SerializationServices.Serialize(retVal);
                return returnValue;
            }
            catch (Exception exception)
            {
                var message = string.Format("Error while invoking member {0}.", name);
                throw new FaultException<RemoteActivatorExceptionDetail>(
                    new RemoteActivatorExceptionDetail(exception) { Description = objectReference.Description },
                    new FaultReason(message));
            }
        }

        public void DeactivateInstance(ObjectReference objectReference)
        {
            try
            {
                var obj = ObjectMap[objectReference];
                ObjectMap.Remove(objectReference);

                var disposable = obj as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            catch (Exception exception)
            {
                var message = string.Format("Error while deactivating instance {0}.", objectReference);
                throw new FaultException<RemoteActivatorExceptionDetail>(
                    new RemoteActivatorExceptionDetail(exception) { Description = objectReference.Description },
                    new FaultReason(message));
            }
        }
    }
}