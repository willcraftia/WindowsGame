#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Willcraftia.Win.Framework
{
    public sealed class ServiceContainer : IServiceProvider
    {
        Dictionary<Type, object> services = new Dictionary<Type, object>();

        public void AddService<T>(object service)
        {
            services.Add(typeof(T), service);
        }

        public object GetService(Type serviceType)
        {
            object service;
            services.TryGetValue(serviceType, out service);
            return service;
        }
    }
}
