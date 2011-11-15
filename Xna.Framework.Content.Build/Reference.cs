#region Using

using System;
using System.IO;
using System.Reflection;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class Reference : IAssemblyReference
    {
        public ContentProject ContentProject { get; private set; }

        public string AssemblyName { get; private set; }
        public string HintPath { get; private set; }

        public Reference(ContentProject contentProject, string assemblyName, string hintPath)
        {
            if (contentProject == null) throw new ArgumentNullException("contentProject");
            if (string.IsNullOrEmpty(assemblyName)) throw new ArgumentNullException("assemblyName");

            ContentProject = contentProject;
            AssemblyName = assemblyName;
            HintPath = hintPath;
        }

        public Assembly LoadAssembly()
        {
            if (!string.IsNullOrEmpty(HintPath))
            {
                return Assembly.LoadFrom(HintPath);
            }

            var assemblyName = new AssemblyName(AssemblyName);
            var assemblyPath = ResolveAssemblyPath(assemblyName);
            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }

            return Assembly.Load(assemblyName);
        }

        string ResolveAssemblyPath(AssemblyName assemblyName)
        {
            var assemblyFileName = assemblyName.Name + ".dll";
            return Path.Combine(ContentProject.XnaAssemblyFolder, assemblyFileName);
        }
    }
}
