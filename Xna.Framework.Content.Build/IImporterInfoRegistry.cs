#region Using

using System.Collections.Generic;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public interface IImporterInfoRegistry
    {
        IEnumerable<IImporterInfo> EnumerateImporterInfos();

        IEnumerable<string> EnumerateImporterNames();

        IImporterInfo GetImporterInfo(string name);
    }
}
