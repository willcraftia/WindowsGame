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
using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Xna.Foundation.Cube.Scenes
{
    public sealed class CubeTerrainActorModel : ModelActorModel
    {
        string terrainAssetName;

        [Description("The asset name of the terrain.")]
        [DefaultValue(null)]
        public string TerrainAssetName
        {
            get { return terrainAssetName; }
            set { terrainAssetName = value; }
        }

        CubeTerrain terrain;

        [ContentSerializerIgnore]
        [Browsable(false)]
        public CubeTerrain Terrain
        {
            get { return terrain; }
        }

        public override void LoadContent()
        {
            terrain = ActorContext.Content.Load<CubeTerrain>(terrainAssetName);
            Model = terrain.Model;

            base.LoadContent();
        }

        protected override bool PreDraw(GameTime gameTime)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            return base.PreDraw(gameTime);
        }
    }
}
