#region Using

using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Content.Studio.PluginFramework;
using Willcraftia.Content.Studio.Plugin.Models;
using Willcraftia.Content.Studio.Plugin.Views;
using Willcraftia.Content.Studio.Views;

#endregion

namespace Willcraftia.Content.Studio.Plugin
{
    public sealed class PluginActivator : IPluginActivator
    {
        public void Load()
        {
            ContentLoaderRegistry.Instance.Register(".xml", new XmlContentLoader());
            ContentTypeResolverRegistry.Instance.Register(".xml", new XmlContentTypeResolver());

            RuntimeContentControlRegistry.Instance.Register<Model, ModelControl>();
            RuntimeContentControlRegistry.Instance.Register<SpriteFont, SpriteFontControl>();
            RuntimeContentControlRegistry.Instance.Register<Texture, TextureControl>();
        }

        public void Unload()
        {
            ContentLoaderRegistry.Instance.Unregister(".xml");
            ContentTypeResolverRegistry.Instance.Unregister(".xml");

            RuntimeContentControlRegistry.Instance.Unregister<Model>();
            RuntimeContentControlRegistry.Instance.Unregister<SpriteFont>();
            RuntimeContentControlRegistry.Instance.Unregister<Texture>();
        }
    }
}
