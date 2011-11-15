#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Willcraftia.Content.Studio.PluginFramework
{
    public sealed class PluginActivatorRegistry
    {
        Dictionary<Assembly, IPluginActivator> activators = new Dictionary<Assembly, IPluginActivator>();

        public ICollection<IPluginActivator> Activators
        {
            get { return activators.Values; }
        }

        public void Register(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(IPluginActivator).IsAssignableFrom(type)) continue;

                activators[assembly] = Activator.CreateInstance(type) as IPluginActivator;

                Tracer.TraceSource.TraceInformation(
                    "Registered PluginActivator '{0}' for '{1}'.", type.Name, assembly.FullName);
            }
        }

        public void Unregister(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            activators.Remove(assembly);

            Tracer.TraceSource.TraceInformation("Unregistered PluginActivator for '{0}'.", assembly.FullName);
        }
    }
}
