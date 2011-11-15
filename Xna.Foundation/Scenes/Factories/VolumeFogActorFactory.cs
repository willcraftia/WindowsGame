#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Factories
{
    /// <summary>
    /// VolumeFogActor の生成を担う ActoryFactory です。
    /// </summary>
    public class VolumeFogActorFactory : PrototypeActorFactory
    {
        /// <summary>
        /// ContentManager から VolumeFogActor をロードして返します。
        /// </summary>
        /// <param name="config">AssetActorConfig。</param>
        /// <returns>ContentManager からロードされた VolumeFogActor。</returns>
        protected override Actor CreatePrototypeActor(AssetActorConfig config)
        {
            return ActorFactoryContext.Content.Load<VolumeFogActor>(config.AssetName);
        }
    }
}
