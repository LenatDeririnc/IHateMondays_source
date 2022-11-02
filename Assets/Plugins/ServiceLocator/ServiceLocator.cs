using System;
using System.Collections.Generic;
using Plugins.ServiceLocator.Interfaces;

namespace Plugins.ServiceLocator
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, IService> _services = new();

        public static void Set<T>(T service) where T : class, IService
        {
            _services[typeof(T)] = service;
        }

        public static void Set(Type type, IService service)
        {
            _services[type] = service;
        }

        public static T Get<T>() where T : class, IService
	    {
            if (_services.ContainsKey(typeof(T)))
		        return (T) _services[typeof(T)];
            return null;
        }
    }
}