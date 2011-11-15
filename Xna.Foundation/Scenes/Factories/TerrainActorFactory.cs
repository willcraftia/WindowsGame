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
    /// TerrainActor の生成を担う ActorFactory です。
    /// </summary>
    /// <remarks>
    /// 同一の Terrain をシーンに複数個配置することはないので、
    /// この ActorFactory では Terrain の複製を考慮しません。
    /// </remarks>
    public class TerrainActorFactory : ActorFactory
    {
        /// <summary>
        /// ContentManager から TerrainActor をロードして返します。
        /// </summary>
        /// <param name="config">AssetActorConfig。</param>
        /// <returns>ContentManager からロードされた TerrainActor。</returns>
        protected override Actor CreateActorInstance(ActorConfig config)
        {
            var assetActorConfig = config as AssetActorConfig;
            return ActorFactoryContext.Content.Load<TerrainActor>(assetActorConfig.AssetName);
        }
    }
}
