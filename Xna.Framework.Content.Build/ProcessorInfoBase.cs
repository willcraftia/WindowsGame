#region Using

using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public abstract class ProcessorInfoBase : IProcessorInfo
    {
        public string TypeName { get; private set; }
        public string DisplayName { get; private set; }

        protected ProcessorInfoBase(string typeName, ContentProcessorAttribute attribute)
        {
            TypeName = typeName;
            DisplayName = attribute.DisplayName;
        }

        public abstract IContentProcessor CreateInstance();
    }
}
