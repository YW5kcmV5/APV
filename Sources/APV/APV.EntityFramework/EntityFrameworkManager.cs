using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Common.Attributes.Proxy;

namespace APV.EntityFramework
{
    public static class EntityFrameworkManager
    {
        private enum ProviderType
        {
            ContextManager,

            DataLayerManager,

            Management
        }

        private static readonly SortedList<int, object> Providers = new SortedList<int, object>();

        static EntityFrameworkManager()
        {
            Register();
        }

        private static int GetKey(ProviderType providerType, Type entityType = null)
        {
            return (entityType != null)
                       ? (providerType.ToString() + entityType.FullName).GetHashCode()
                       : providerType.ToString().GetHashCode();
        }

        private static void Register(ProviderType providerType, Type provider, Type entityType = null)
        {
            int key = GetKey(providerType, entityType);
            Register(key, provider);
        }

        private static void Register(int key, object value)
        {
            lock (Providers)
            {
                if (!Providers.ContainsKey(key))
                {
                    Providers.Add(key, value);
                }
                else
                {
                    Providers[key] = value;
                }
            }
        }

        private static object Get(int key)
        {
            lock (Providers)
            {
                int index = Providers.IndexOfKey(key);
                if (index != -1)
                {
                    object manager = Providers.Values[index];
                    if (manager is Type)
                    {
                        var managerType = (Type) manager;
                        manager = (managerType.BasedOn(typeof (BaseMarshalProxy)))
                                      ? BaseMarshalProxy.Create(managerType)
                                      : Activator.CreateInstance((Type) manager, true);
                        Providers[key] = manager;
                    }
                    return manager;
                }
                return null;
            }
        }

        public static void Register()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var systemAssembliesNames = new[] {"System", "mscorlib", "Microsoft", "vshost32"};
            foreach (Assembly assembly in assemblies)
            {
                string fullName = assembly.FullName;
                if (!systemAssembliesNames.Any(fullName.StartsWith))
                {
                    Register(assembly);
                }
            }
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                AssemblyName[] references = entryAssembly.GetReferencedAssemblies();
                foreach (AssemblyName assemblyName in references)
                {
                    if ((!assemblyName.FullName.StartsWith("System")) && (assemblies.All(assembly => assembly.FullName != assemblyName.FullName)))
                    {
                        Assembly assembly = Assembly.Load(assemblyName);
                        Register(assembly);
                    }
                }
            }
        }

        public static void Register(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            Type[] allTypes = assembly.GetTypes().Where(type => (type.IsClass) && (!type.IsAbstract) && (!type.ContainsGenericParameters)).ToArray();

            lock (Providers)
            {
                //Register context managers
                Type contextManager = allTypes.LastOrDefault(type => type.BasedOn(typeof (IContextManager)));
                if (contextManager != null)
                {
                    Register(ProviderType.ContextManager, contextManager);
                }

                //Register data layer managers
                Type[] providers = allTypes.Where(type => type.BasedOn(typeof(IDataLayerManager))).ToArray();
                foreach (Type provider in providers)
                {
                    Type baseInterface = provider.GetInterface(typeof(IDataLayerManager<>).FullName);
                    if (baseInterface != null)
                    {
                        Type[] genericTypes = baseInterface.GenericTypeArguments;
                        Type entityType = genericTypes.SingleOrDefault(item => item.BasedOn(typeof(IEntity)));
                        if (entityType != null)
                        {
                            Register(ProviderType.DataLayerManager, provider, entityType);
                        }
                    }
                }

                //Register managements
                providers = allTypes.Where(type => type.BasedOn(typeof(IManagement))).ToArray();
                foreach (Type provider in providers)
                {
                    Type baseInterface = provider.GetInterface(typeof(IManagement<>).FullName);
                    if (baseInterface != null)
                    {
                        Type[] genericTypes = baseInterface.GenericTypeArguments;
                        Type entityType = genericTypes.SingleOrDefault(item => item.BasedOn(typeof(IEntity)));
                        if (entityType != null)
                        {
                            Register(ProviderType.Management, provider, entityType);
                        }
                    }
                }
            }
        }

        public static IContextManager GetContextManager()
        {
            int key = GetKey(ProviderType.ContextManager);
            lock (Providers)
            {
                object manager = Get(key);

                if (manager == null)
                    throw new InvalidOperationException("Context manager is not registered.");

                return (IContextManager) manager;
            }
        }

        public static IDataLayerManager GetManager(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            Type type = entityType;
            int key = GetKey(ProviderType.DataLayerManager, entityType);
            lock (Providers)
            {
                object manager = Get(key);

                if (manager == null)
                    throw new InvalidOperationException(string.Format("Data layer manager for type \"{0}\" is not registered.", type.FullName));

                return (IDataLayerManager) manager;
            }
        }

        public static IDataLayerManager<TEntity> GetManager<TEntity>() where TEntity : IEntity
        {
            Type entityType = typeof(TEntity);
            return (IDataLayerManager<TEntity>) GetManager(entityType);
        }

        public static IManagement GetManagement(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            Type type = entityType;
            int key = GetKey(ProviderType.Management, entityType);
            lock (Providers)
            {
                object management = Get(key);

                if (management == null)
                    throw new InvalidOperationException(string.Format("Management for type \"{0}\" is not registered.", type.FullName));

                return (IManagement) management;
            }
        }

        public static IManagement<TEntity> GetManagement<TEntity>() where TEntity : IEntity
        {
            Type entityType = typeof(TEntity);
            return (IManagement<TEntity>)GetManagement(entityType);
        }
    }
}