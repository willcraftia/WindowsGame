#region Using

using System;
using System.IO;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Xna.Framework.Content.Pipeline.Xml;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Models
{
    public sealed class XmlContentTypeResolver : IContentTypeResolver
    {
        public Type ResolveContentType(FileInfo file)
        {
            try
            {
                var content = XmlContentHelper.Deserialize<object>(file.FullName);
                return content.GetType();
            }
            catch
            {
                return null;
            }
        }
    }
}
