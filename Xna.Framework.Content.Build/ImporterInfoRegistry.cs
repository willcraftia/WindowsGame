#region Using

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class ImporterInfoRegistry : IImporterInfoRegistry
    {
        Dictionary<string, IImporterInfo> importerInfos = new Dictionary<string, IImporterInfo>();

        public IEnumerable<IImporterInfo> EnumerateImporterInfos()
        {
            return importerInfos.Values;
        }

        public IEnumerable<string> EnumerateImporterNames()
        {
            return importerInfos.Keys;
        }

        public IImporterInfo GetImporterInfo(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            IImporterInfo importerInfo = null;
            importerInfos.TryGetValue(name, out importerInfo);
            return importerInfo;
        }

        public void Register(IAssemblyReference assemblyReference)
        {
            if (assemblyReference == null) throw new ArgumentNullException("assemblyReference");

            var assembly = assemblyReference.LoadAssembly();
            
            Tracer.TraceSource.TraceInformation("Parsing ContentImporters from assembly '{0}'.", assembly.FullName);

            foreach (var type in assembly.GetTypes())
            {
                if (IsImporter(type))
                {
                    var attribute = GetContentImporterAttribute(type);
                    importerInfos[type.Name] = new AssemblyReferenceImporterInfo(type.FullName, attribute, assemblyReference);
                    Tracer.TraceSource.TraceInformation("Registered ContentImporter '{0}'.", type.Name);
                }
            }
        }

        public void Register(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            Tracer.TraceSource.TraceInformation("Parsing ContentImporters from assembly '{0}'.", assembly.FullName);

            foreach (var type in assembly.GetTypes())
            {
                if (IsImporter(type))
                {
                    var attribute = GetContentImporterAttribute(type);
                    importerInfos[type.Name] = new AssemblyImporterInfo(type.FullName, attribute, assembly);
                    Tracer.TraceSource.TraceInformation("Registered ContentImporter '{0}'.", type.Name);
                }
            }
        }

        public void Unregister(IAssemblyReference assemblyReference)
        {
            if (assemblyReference == null) throw new ArgumentNullException("assemblyReference");

            var assembly = assemblyReference.LoadAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (IsImporter(type))
                {
                    importerInfos.Remove(type.Name);
                    Tracer.TraceSource.TraceInformation("Unregistered ContentImporter '{0}'.", type.Name);
                }
            }
        }

        public void Unregister(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            foreach (var type in assembly.GetTypes())
            {
                if (IsImporter(type))
                {
                    importerInfos.Remove(type.Name);
                    Tracer.TraceSource.TraceInformation("Unregistered ContentImporter '{0}'.", type.Name);
                }
            }
        }

        bool IsImporter(Type type)
        {
            if (GetContentImporterAttribute(type) == null) return false;

            return (type.IsClass && type.IsPublic && !type.IsAbstract && type.GetInterface(typeof(IContentImporter).Name) != null);
        }

        ContentImporterAttribute GetContentImporterAttribute(Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(ContentImporterAttribute), false) as ContentImporterAttribute;
        }
    }
}
