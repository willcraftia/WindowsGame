#region Using

using System.Reflection;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class AssemblyProcessorInfo : ProcessorInfoBase
    {
        public Assembly Assembly { get; private set; }

        public AssemblyProcessorInfo(string typeName, ContentProcessorAttribute attribute, Assembly assembly)
            : base(typeName, attribute)
        {
            Assembly = assembly;
        }

        public override IContentProcessor CreateInstance()
        {
            return Assembly.CreateInstance(TypeName) as IContentProcessor;
        }
    }
}
