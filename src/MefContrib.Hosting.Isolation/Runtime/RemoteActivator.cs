using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using MefContrib.Hosting.Isolation.Runtime.Activation;

namespace MefContrib.Hosting.Isolation.Runtime
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
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
            var assembly = Assembly.Load(assemblyName);
            if (_assemblyInitialized.Contains(assembly) == false)
            {
                var hooks = assembly.GetTypes().Where(t => typeof (ActivationHook).IsAssignableFrom(t));
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

            var contract = (string)_initialized[type].Parts.First().ExportDefinitions.First().Metadata["ExportTypeIdentity"];
            var instance = _container.GetExports(typeof (object), null, contract).FirstOrDefault();

            var reference = new ObjectReference(description);
            _map[reference] = instance.Value;

            return reference;
        }

        public object InvokeMember(ObjectReference objectReference, string name, List<RuntimeArgument> arguments)
        {
            var obj = _map[objectReference];
            var type = obj.GetType();
            var methodInfo = type.GetMethod(name);
            var args = SerializationServices.Deserialize(arguments);
            var retVal = methodInfo.Invoke(obj, args.ToArray());

            return retVal;
        }

        public void DeactivateInstance(ObjectReference objectReference)
        {
            var obj = _map[objectReference];
            _map.Remove(objectReference);

            IDisposable disposable = obj as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}