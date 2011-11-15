#region Using

using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class AssemblyReferenceImporterInfo : ImporterInfoBase
    {
        public IAssemblyReference AssemblyReference { get; private set; }

        public AssemblyReferenceImporterInfo(string typeName, ContentImporterAttribute attribute, IAssemblyReference assemblyReference)
            : base(typeName, attribute)
        {
            AssemblyReference = assemblyReference;
        }

        public override IContentImporter CreateInstance()
        {
            var assembly = AssemblyReference.LoadAssembly();
            return assembly.CreateInstance(TypeName) as IContentImporter;
        }
    }
}
