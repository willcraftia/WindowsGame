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
    public abstract class DebugComponent : DrawableGameComponent
    {
        #region Injected

        IDebugContext debugContext;

        /// <summary>
        /// IDebugContext を取得または設定します。
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// IDebugContext は DebugComponent を GameComponent として追加する際に、
        /// デバッグコンテナにより自動的に設定されます。
        /// </remarks>
        public IDebugContext DebugContext
        {
            get { return debugContext; }
            set { debugContext = value; }
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// IDebugContext の SpriteBatch を取得するヘルパプロパティです。
        /// </summary>
        /// <remarks>
        /// DebugComponent を GameComponent として追加する前に呼び出してはなりません。
        /// </remarks>
        protected SpriteBatch SpriteBatch
        {
            get { return debugContext.SpriteBatch; }
        }

        /// <summary>
        /// IDebugContext の Font を取得するヘルパプロパティです。
        /// </summary>
        /// <remarks>
        /// DebugComponent を GameComponent として追加する前に呼び出してはなりません。
        /// </remarks>
        protected SpriteFont Font
        {
            get { return debugContext.Font; }
        }

        /// <summary>
        /// IDebugContext の Content を取得するヘルパプロパティです。
        /// </summary>
        /// <remarks>
        /// DebugComponent を GameComponent として追加する前に呼び出してはなりません。
        /// </remarks>
        protected ContentManager Content
        {
            get { return debugContext.Content; }
        }

        /// <summary>
        /// IDebugContext の Content を取得するヘルパプロパティです。
        /// </summary>
        /// <remarks>
        /// DebugComponent を GameComponent として追加する前に呼び出してはなりません。
        /// </remarks>
        protected Texture2D FillTexture
        {
            get { return debugContext.FillTexture; }
        }

        IInputDevice inputDevice;

        /// <summary>
        /// IInputDevice を取得します。
        /// </summary>
        protected IInputDevice InputDevice
        {
            get { return inputDevice; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="game">インスタンスを登録する Game。</param>
        public DebugComponent(Game game)
            : base(game)
        {
        }

        #endregion

        #region Initialize

        public override void Initialize()
        {
            // IInputDevice を取得します。
            inputDevice = debugContext.GetRequiredService<IInputDeviceService>().InputDevice;

            base.Initialize();
        }

        #endregion
    }
}
