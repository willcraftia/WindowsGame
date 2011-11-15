#region Using

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class ProcessorInfoRegistry : IProcessorInfoRegistry
    {
        Dictionary<string, IProcessorInfo> processorInfos = new Dictionary<string, IProcessorInfo>();

        public IEnumerable<IProcessorInfo> EnumerateProcessorInfos()
        {
            return processorInfos.Values;
        }

        public IEnumerable<string> EnumerateProcessorNames()
        {
            return processorInfos.Keys;
        }

        public IProcessorInfo GetProcessorInfo(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            IProcessorInfo importerInfo = null;
            processorInfos.TryGetValue(name, out importerInfo);
            return importerInfo;
        }

        public void Register(IAssemblyReference assemblyReference)
        {
            if (assemblyReference == null) throw new ArgumentNullException("assemblyReference");

            var assembly = assemblyReference.LoadAssembly();

            Tracer.TraceSource.TraceInformation("Parsing ContentProcessors from assembly '{0}'.", assembly.FullName);

            foreach (var type in assembly.GetTypes())
            {
                if (IsProcessor(type))
                {
                    var attribute = GetContentProcessorAttribute(type);
                    processorInfos[type.Name] = new AssemblyReferenceProcessorInfo(type.FullName, attribute, assemblyReference);
                    Tracer.TraceSource.TraceInformation("Registered ContentProcessor '{0}'.", type.Name);
                }
            }
        }

        public void Register(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            Tracer.TraceSource.TraceInformation("Parsing ContentProcessors from assembly '{0}'.", assembly.FullName);

            foreach (var type in assembly.GetTypes())
            {
                if (IsProcessor(type))
                {
                    var attribute = GetContentProcessorAttribute(type);
                    processorInfos[type.Name] = new AssemblyProcessorInfo(type.FullName, attribute, assembly);
                    Tracer.TraceSource.TraceInformation("Registered ContentProcessor '{0}'.", type.Name);
                }
            }
        }

        public void Unregister(IAssemblyReference assemblyReference)
        {
            if (assemblyReference == null) throw new ArgumentNullException("assemblyReference");

            var assembly = assemblyReference.LoadAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (IsProcessor(type))
                {
                    processorInfos.Remove(type.Name);
                    Tracer.TraceSource.TraceInformation("Unregistered ContentProcessor '{0}'.", type.Name);
                }
            }
        }

        public void Unregister(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            foreach (var type in assembly.GetTypes())
            {
                if (IsProcessor(type))
                {
                    processorInfos.Remove(type.Name);
                    Tracer.TraceSource.TraceInformation("Unregistered ContentProcessor '{0}'.", type.Name);
                }
            }
        }

        bool IsProcessor(Type type)
        {
            if (GetContentProcessorAttribute(type) == null) return false;

            return (type.IsClass && type.IsPublic && !type.IsAbstract && type.GetInterface(typeof(IContentProcessor).Name) != null);
        }

        ContentProcessorAttribute GetContentProcessorAttribute(Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(ContentProcessorAttribute), false) as ContentProcessorAttribute;
        }
    }
}
