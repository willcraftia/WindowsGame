#region Using

using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Content.Studio.PluginFramework;
using Willcraftia.Content.Studio.Plugin.Foundation.ViewModels;
using Willcraftia.Content.Studio.Plugin.Foundation.Views;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Content.Studio.Views;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation
{
    public sealed class PluginActivator : IPluginActivator
    {
        public void Load()
        {
            ContentViewModelRegistry.Instance.Register<SceneConfig, SceneConfigViewModel>();
            ContentViewModelRegistry.Instance.Register<Actor, ActorViewModel>();
            ContentControlRegistry.Instance.Register<SceneConfigViewModel, SceneConfigControl>();
            ContentControlRegistry.Instance.Register<ActorViewModel, ActorControl>();

            ActorModelViewModelRegistry.Register<AssetModelActorModel, AssetModelActorModelViewModel>();
            ActorModelControlRegistry.Register<AssetModelActorModelViewModel, AssetModelActorModelControl>();

            RuntimeContentControlRegistry.Instance.Register<Actor, RuntimeActorControl>();
        }

        public void Unload()
        {
            ContentViewModelRegistry.Instance.Unregister<SceneConfig>();
            ContentViewModelRegistry.Instance.Unregister<Actor>();
            ContentControlRegistry.Instance.Unregister<SceneConfigViewModel>();
            ContentControlRegistry.Instance.Unregister<ActorViewModel>();

            ActorModelViewModelRegistry.Unregister<AssetModelActorModel>();
            ActorModelControlRegistry.Unregister<AssetModelActorModelViewModel>();

            RuntimeContentControlRegistry.Instance.Unregister<Actor>();
        }
    }
}
