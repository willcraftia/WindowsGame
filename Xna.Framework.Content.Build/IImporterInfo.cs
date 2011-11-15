#region Using

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public interface IImporterInfo
    {
        string TypeName { get; }
        bool CacheImportedData { get; }
        string DefaultProcessor { get; }
        string DisplayName { get; }
        IEnumerable<string> FileExtensions { get; }

        IContentImporter CreateInstance();
    }
}
