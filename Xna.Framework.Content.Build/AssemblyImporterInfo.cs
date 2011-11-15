#region Using

using System.Reflection;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class AssemblyImporterInfo : ImporterInfoBase
    {
        public Assembly Assembly { get; private set; }

        public AssemblyImporterInfo(string typeName, ContentImporterAttribute attribute, Assembly assembly)
            : base(typeName, attribute)
        {
            Assembly = assembly;
        }

        public override IContentImporter CreateInstance()
        {
            return Assembly.CreateInstance(TypeName) as IContentImporter;
        }
    }
}
