#region Using

using System;
using System.IO;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public interface IContentTypeResolver
    {
        Type ResolveContentType(FileInfo file);
    }
}
