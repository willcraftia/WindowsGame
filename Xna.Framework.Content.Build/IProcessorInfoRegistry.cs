#region Using

using System.Collections.Generic;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public interface IProcessorInfoRegistry
    {
        IEnumerable<IProcessorInfo> EnumerateProcessorInfos();

        IEnumerable<string> EnumerateProcessorNames();

        IProcessorInfo GetProcessorInfo(string name);
    }
}
