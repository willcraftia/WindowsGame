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
using Willcraftia.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// DebugComponent の状態を制御する GameComponent です。
    /// </summary>
    /// <remarks>
    /// DebugContainer は DebugComponent の追加を監視するために、
    /// あらゆる DebugComponent よりも先に登録する必要があります。
    /// また、デバッグ用のグラフィックス コンテンツの生成と管理するために DrawableGameComponent を継承していますが、
    /// 更新や描画は行いません。
    /// </remarks>
    public sealed class DebugContainer : DrawableGameComponent, IDebugContext
    {
        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="game">インスタンスを登録する Game。</param>
        public DebugContainer(Game game)
            : base(game)
        {
            // DebugComponent の追加と削除を監視できるように EventHandler を登録します。
            game.Components.ComponentAdded += new EventHandler<GameComponentCollectionEventArgs>(OnComponentAdded);
            game.Components.ComponentRemoved += new EventHandler<GameComponentCollectionEventArgs>(OnComponentRemoved);

            // DebugContainer は DebugComponent に対する DI コンテナとして振る舞うだけであり、
            // 更新も描画も行いません。
            Enabled = false;
            Visible = false;
        }

        /// <summary>
        /// 追加された GameComponent が DebugComponent ならば DebugContext プロパティを設定します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データ。</param>
        void OnComponentAdded(object sender, GameComponentCollectionEventArgs e)
        {
            var debugComponent = e.GameComponent as DebugComponent;
            if (debugComponent != null)
            {
                debugComponent.DebugContext = this;
            }
        }

        /// <summary>
        /// 削除された GameComponent が DebugComponent ならば DebugContext プロパティをクリアします。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データ。</param>
        void OnComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            var debugComponent = e.GameComponent as DebugComponent;
            if (debugComponent != null)
            {
                debugComponent.DebugContext = null;
            }
        }

        #endregion

        #region LoadContent

        /// <summary>
        /// デバッグ用のコンテンツをロードします。
        /// </summary>
        protected override void LoadContent()
        {
            content = new ContentManager(Game.Services, "Content/Debugs");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("Fonts/Debug");
            fillTexture = new Texture2D(GraphicsDevice, 1, 1);
            fillTexture.SetData<Color>(new Color[] { Color.White });

            base.LoadContent();
        }

        #endregion

        #region UnloadContent

        /// <summary>
        /// デバッグ用のコンテンツをアンロードします。
        /// </summary>
        protected override void UnloadContent()
        {
            if (content != null)
            {
                content.Unload();
            }
            if (spriteBatch != null)
            {
                spriteBatch.Dispose();
            }
            if (fillTexture != null)
            {
                fillTexture.Dispose();
            }

            base.UnloadContent();
        }

        #endregion

        #region IDebugContext

        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        SpriteFont font;
        public SpriteFont Font
        {
            get { return font; }
        }

        ContentManager content;
        public ContentManager Content
        {
            get { return content; }
        }

        Texture2D fillTexture;
        public Texture2D FillTexture
        {
            get { return fillTexture; }
        }

        #endregion

        #region IServiceProvider

        public object GetService(Type serviceType)
        {
            return Game.Services.GetService(serviceType);
        }

        #endregion
    }
}
