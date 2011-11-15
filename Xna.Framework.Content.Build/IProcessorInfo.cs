#region Using

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public interface IProcessorInfo
    {
        string TypeName { get; }
        string DisplayName { get; }

        IContentProcessor CreateInstance();
    }
}
