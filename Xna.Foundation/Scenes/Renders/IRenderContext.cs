#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public interface IRenderContext : IGameContext
    {
        /// <summary>
        /// シーン描画機能で使える ContentManager を取得します。
        /// </summary>
        ContentManager Content { get; }

        /// <summary>
        /// シーン描画機能で使えるデフォルトの SpriteBatch を取得します。
        /// </summary>
        SpriteBatch SpriteBatch { get; }

        IRenderableScene Scene { get; }

        IEnumerable<CharacterActor> VisibleCharacters { get; }
        IEnumerable<StaticMeshActor> VisibleStaticMeshes { get; }
        IEnumerable<TerrainActor> VisibleTerrains { get; }
        IEnumerable<FluidSurfaceActor> VisibleFluidSurfaces { get; }
        IEnumerable<SkyDomeActor> VisibleSkyDomes { get; }
        IEnumerable<VolumeFogActor> VisibleVolumeFogs { get; }
    }
}
