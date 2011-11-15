#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace Willcraftia.Content.Studio.PluginFramework
{
    public sealed class PluginManager
    {
        PluginActivatorRegistry pluginActivatorRegistry = new PluginActivatorRegistry();

        public void Register(string assemblyFile)
        {
            var assembly = Assembly.LoadFrom(assemblyFile);
            var activator = GetPluginActivator(assembly);
            if (activator != null)
            {
                activator.Load();
                Tracer.TraceSource.TraceInformation("Loaded plug-in '{0}'", assemblyFile);
            }
        }

        public void Unregister(string assemblyFile)
        {
            var assembly = Assembly.LoadFrom(assemblyFile);
            var activator = GetPluginActivator(assembly);
            if (activator != null)
            {
                activator.Unload();
                Tracer.TraceSource.TraceInformation("Unloaded plug-in '{0}'", assemblyFile);
            }
        }

        IPluginActivator GetPluginActivator(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(IPluginActivator).IsAssignableFrom(type)) continue;

                return assembly.CreateInstance(type.FullName) as IPluginActivator;
            }
            // TODO: 例外を投げたい
            return null;
        }
    }
}
