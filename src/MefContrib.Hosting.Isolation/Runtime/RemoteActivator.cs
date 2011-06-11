using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using MefContrib.Hosting.Isolation.Runtime.Activation;

namespace MefContrib.Hosting.Isolation.Runtime
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class RemoteActivator : IRemoteActivator
    {
        private static Dictionary<ObjectReference, object> _map = new Dictionary<ObjectReference, object>();
        private static AggregateCatalog _aggregateCatalog = new AggregateCatalog();
        private static CompositionContainer _container = new CompositionContainer(_aggregateCatalog);
        private static Dictionary<Type, TypeCatalog> _initialized = new Dictionary<Type, TypeCatalog>();
        private static HashSet<Assembly> _assemblyInitialized = new HashSet<Assembly>();

        public void HeartBeat()
        {
        }

        public ObjectReference ActivateInstance(ActivationHostDescription description, string assemblyName, string typeName)
        {
            try
            {
                var assembly = Assembly.Load(assemblyName);
                if (_assemblyInitialized.Contains(assembly) == false)
                {
                    var hooks = assembly.GetTypes().Where(t => typeof(ActivationHook).IsAssignableFrom(t));
                    foreach (var hook in hooks)
                    {
                        try
                        {
                            var hookInstance = (ActivationHook)Activator.CreateInstance(hook);
                            hookInstance.Initialize(_container, _aggregateCatalog);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    _assemblyInitialized.Add(assembly);
                }

                var type = assembly.GetTypes().Where(t => t.FullName == typeName).FirstOrDefault();
                if (_initialized.ContainsKey(type) == false)
                {
                    var catalog = new TypeCatalog(type);
                    _initialized.Add(type, catalog);
                    _aggregateCatalog.Catalogs.Add(catalog);
                }

                //var contract = (string)_initialized[type].Parts.First().ExportDefinitions.First().Metadata["ExportTypeIdentity"];
                //var instance = _container.GetExports(typeof(object), null, contract).FirstOrDefault();
                var instance = Activator.CreateInstance(type);

                var reference = new ObjectReference(description);
                _map[reference] = instance;
                //_map[reference] = instance.Value;

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
                var obj = _map[objectReference];
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
                var obj = _map[objectReference];
                _map.Remove(objectReference);

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