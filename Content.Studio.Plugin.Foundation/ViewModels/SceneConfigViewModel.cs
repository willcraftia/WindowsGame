#region Using

using System;
using System.IO;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Content.Studio.Plugin.Foundation.Models;
using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.ViewModels
{
    public sealed class SceneConfigViewModel : ContentViewModelBase
    {
        public Asset Asset { get; private set; }

        public SceneConfigEdit SceneConfigEdit { get; private set; }

        public SceneConfigViewModel(FileInfo file)
            : base(file)
        {
            Asset = Workspace.Current.Project.GetAsset(file);
            SceneConfigEdit = new SceneConfigEdit(Model as SceneConfig);
        }

        public void LoadContent(ISceneContext sceneContext)
        {
            if (sceneContext == null) throw new ArgumentNullException("sceneContext");

            SceneConfigEdit.SceneContext = sceneContext;
            SceneConfigEdit.LoadContent();
        }

        public void UnloadContent()
        {
            SceneConfigEdit.UnloadContent();
        }
    }
}
