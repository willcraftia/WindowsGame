#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// アセット名から Model をロードする ActorModel です。
    /// </summary>
    public class AssetModelActorModel : ModelActorModel
    {
        string modelAssetName;

        /// <summary>
        /// アセット名。
        /// </summary>
        public string ModelAssetName
        {
            get { return modelAssetName; }
            set { modelAssetName = value; }
        }

        public override void LoadContent()
        {
            if (!string.IsNullOrEmpty(modelAssetName))
            {
                Model = Content.Load<Model>(modelAssetName);
            }

            base.LoadContent();
        }
    }
}
