#region Using

using Willcraftia.Content.Studio.Models;
using Willcraftia.Xna.Framework.Content.Pipeline.Xml;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Models
{
    public sealed class XmlContentLoader : IContentLoader
    {
        public object Load(string path)
        {
            return XmlContentHelper.Deserialize<object>(path);
        }

        public bool Save(string path, object content)
        {
            XmlContentHelper.Serialize<object>(content, path);
            return true;
        }
    }
}
