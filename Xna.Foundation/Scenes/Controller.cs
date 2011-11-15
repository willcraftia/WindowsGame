#region Using

using System;
using System.Collections.Generic;
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

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// シーン内の要素を制御するクラスです。
    /// </summary>
    public abstract class Controller
    {
        #region Events

        /// <summary>
        /// Enabled プロパティが変更されると発生します。
        /// </summary>
        public event EventHandler EnabledChanged;

        #endregion

        #region Fields and Properties

        string name;

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        [ContentSerializer(Optional = true)]
        [DefaultValue(null)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        bool enabled = true;

        /// <summary>
        /// 制御を行うかどうかを示します。
        /// </summary>
        /// <value>
        /// true (制御を行う場合)、false (それ以外の場合)。
        /// </value>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (EnabledChanged != null)
                    {
                        EnabledChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        bool contentLoaded;

        /// <summary>
        /// コンテンツのロードが行われたかどうかを示します。
        /// </summary>
        /// <value>
        /// true (コンテンツのロードが行われた場合)、false (それ以外の場合)。
        /// </value>
        [Browsable(false)]
        [ContentSerializerIgnore]
        public bool ContentLoaded
        {
            get { return contentLoaded; }
        }

        IControllerContext controllerContext;

        /// <summary>
        /// IControllerContext を取得または設定します。
        /// </summary>
        /// <remarks>
        /// IControllerContext は、LoadContent メソッドの呼び出し前に、
        /// Controller を管理するクラスにより設定されます。
        /// </remarks>
        [Browsable(false)]
        [ContentSerializerIgnore]
        public IControllerContext ControllerContext
        {
            get { return controllerContext; }
            set { controllerContext = value; }
        }

        /// <summary>
        /// Scene を取得します。
        /// </summary>
        [Browsable(false)]
        [ContentSerializerIgnore]
        public Scene Scene
        {
            get { return controllerContext.Scene; }
        }

        /// <summary>
        /// IInputDevice を取得します。
        /// </summary>
        [Browsable(false)]
        [ContentSerializerIgnore]
        public IInputDevice InputDevice
        {
            get { return controllerContext.InputDevice; }
        }

        PlayerIndex playerIndex;

        [Browsable(false)]
        [ContentSerializerIgnore]
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
            protected set { playerIndex = value; }
        }

        #endregion

        #region LoadContent

        /// <summary>
        /// コンテンツをロードします。
        /// </summary>
        /// <remarks>
        /// このメソッドの実行後に ContentLoaded プロパティが true に設定されます。
        /// </remarks>
        public virtual void LoadContent()
        {
            playerIndex = Scene.Player.ControllingPlayer;

            contentLoaded = true;
        }

        #endregion

        #region UnloadContent

        /// <summary>
        /// コンテンツをアンロードします。
        /// </summary>
        /// <remarks>
        /// このメソッドの実行後に ContentLoaded プロパティが false に設定されます。
        /// </remarks>
        public virtual void UnloadContent()
        {
            contentLoaded = false;
        }

        #endregion

        #region Update

        /// <summary>
        /// 制御を更新します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        public abstract void Update(GameTime gameTime);

        #endregion
    }
}
