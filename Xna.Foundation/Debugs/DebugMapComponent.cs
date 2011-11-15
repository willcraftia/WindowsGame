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

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// 描画処理で生成するテクスチャのデバッグ表示を制御する DebugComponent です。
    /// </summary>
    public sealed class DebugMapComponent : DebugComponent, IDebugMap
    {
        #region Fields and Properties

        public const int DefaultMapSize = 128;
        public const int DefaultMapOffsetX = 2;
        public const int DefaultMapOffsetY = 2;

        int mapSize = DefaultMapSize;
        
        /// <summary>
        /// テクスチャ描画領域のサイズを取得または設定します。
        /// </summary>
        public int MapSize
        {
            get { return mapSize; }
            set { mapSize = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="game">インスタンスを登録する Game。</param>
        public DebugMapComponent(Game game)
            : base(game)
        {
            game.Components.ComponentRemoved += new EventHandler<GameComponentCollectionEventArgs>(OnComponentRemoved);
            game.Components.ComponentAdded += new EventHandler<GameComponentCollectionEventArgs>(OnComponentAdded);

            // 描画処理のみを行い、更新処理は行いません。
            Enabled = false;
        }

        /// <summary>
        /// 自分自身が GameComponent として登録される時に、自分自身を現在有効な IDebugMap として設定します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データ。</param>
        void OnComponentAdded(object sender, GameComponentCollectionEventArgs e)
        {
            if (e.GameComponent == this)
            {
                DebugMap.Instance = this;
            }
        }

        /// <summary>
        /// 自分自身が GameComponent としての登録から解除される時に、
        /// 自分自身が現在有効な IDebugMap ではなくなるように設定します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データ。</param>
        void OnComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            if (e.GameComponent == this)
            {
                DebugMap.Instance = null;
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// 登録されているテクスチャを描画します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        public override void Draw(GameTime gameTime)
        {
            int offsetX = DefaultMapOffsetX;
            int offsetY = DefaultMapOffsetY;
            int width = MapSize;
            int height = MapSize;
            var rect = new Rectangle(offsetX, offsetY, width, height);

            foreach (var map in maps)
            {
                SamplerState samplerState = null;

                if (map.Format == SurfaceFormat.Single ||
                    map.Format == SurfaceFormat.HalfSingle ||
                    map.Format == SurfaceFormat.Vector2 ||
                    map.Format == SurfaceFormat.Vector4)
                {
                    samplerState = SamplerState.PointClamp;
                }

                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, samplerState, null, null);
                SpriteBatch.Draw(map, rect, Color.White);
                SpriteBatch.End();

                rect.X += width + offsetX;
                if (GraphicsDevice.Viewport.Width < rect.X + width)
                {
                    rect.X = offsetX;
                    rect.Y += height + offsetY;
                }
            }

            maps.Clear();
        }

        #endregion

        #region IDebugMap

        List<Texture2D> maps = new List<Texture2D>();
        public IList<Texture2D> Maps
        {
            get { return maps; }
        }

        #endregion
    }
}
