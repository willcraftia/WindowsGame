#region Using

using System;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class AssemblyReferenceProcessorInfo : ProcessorInfoBase
    {
        public IAssemblyReference AssemblyReference { get; private set; }

        public AssemblyReferenceProcessorInfo(string typeName, ContentProcessorAttribute attribute, IAssemblyReference assemblyReference)
            : base(typeName, attribute)
        {
            AssemblyReference = assemblyReference;
        }

        public override IContentProcessor CreateInstance()
        {
            var assembly = AssemblyReference.LoadAssembly();
            return assembly.CreateInstance(TypeName) as IContentProcessor;
        }
    }
}
