#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Screens
{
    /// <summary>
    /// 画面は各々の更新および描画ロジックを備える単一レイヤーであり、
    /// 他のレイヤーと組み合わせて複雑なメニュー システムを構築することができます。
    /// たとえば、メイン メニュー、オプション メニュー、"終了してもよろしいですか" 
    /// メッセージ ボックス、メインのゲームプレイ画面は、すべて GameScreen として
    /// 実装されます。
    /// </summary>
    public abstract class Screen
    {
        #region Properties

        bool isPopup = false;

        /// <summary>
        /// この画面がポップアップ画面であるかどうかを示します。
        /// <value>true (ポップアップ画面の場合)、false (それ以外の場合)。</value>
        /// <remarks>
        /// 既存の画面の上に表示される画面がポップアップ画面である場合、
        /// ポップアップ画面はレイヤーのように重ねて表示されます。
        /// よって、IsPopup が true の場合、ポップアップの下にある画面を
        /// オフに移行する必要はありません。
        /// 通常、既存の画面の上に、他の新しい画面が表示される際には、
        /// 新しい画面用の領域を確保するために、古い画面はオフの状態に移行します。
        /// </remarks>
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;

        /// <summary>
        /// 画面をアクティブ化したときに、画面がオンに移行する時間を示します。
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            set { transitionOnTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;

        /// <summary>
        /// 画面を非アクティブ化したときに、画面がオフに移行する時間を示します。
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            set { transitionOffTime = value; }
        }

        float transitionPosition = 1;

        /// <summary>
        /// 0 (完全にオンへ移行完了) から 1 (完全にオフへ移行完了) までの
        /// 画面移行の現在の程度を取得します。
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            set { transitionPosition = value; }
        }

        /// <summary>
        /// 1 (完全にオンへ移行完了) から 0 (完全にオフへ移行完了) までの
        /// 画面移行の現在のアルファ値を取得します。
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1.0f - transitionPosition; }
        }

        ScreenState screenState = ScreenState.TransitionOn;

        /// <summary>
        /// 現在の画面の移行状態を取得します。
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            set { screenState = value; }
        }

        bool isExiting = false;

        /// <summary>
        /// この画面が削除されるかどうかを示します。
        /// <value>true (削除される場合)、false (それ以外の場合)。</value>
        /// <remarks>
        /// このプロパティを true にすると画面の状態は TransitionOff に設定され、
        /// 移行が完了すると即座に画面が自動的に削除されます。
        /// </remarks>
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        bool hasFocus;

        /// <summary>
        /// この画面がフォーカスを持っているかどうかを示します。
        /// <value>true (フォーカスを持つ場合)、false (それ以外の場合)。</value>
        /// </summary>
        public bool HasFocus
        {
            get { return hasFocus; }
            set { hasFocus = value; }
        }

        bool isCovered;

        /// <summary>
        /// この画面が他の画面に覆われているかどうかを示します。
        /// <value>true (他の画面に覆われている場合)、false(それ以外の場合)。</value>
        /// <remarks>
        /// 背景画面ではない場合、このプロパティが true になると画面の状態は TransitionOff に設定され、
        /// その後、移行が完了すると画面の状態は Hidden に設定されます。
        /// </remarks>
        /// </summary>
        public bool IsCovered
        {
            get { return isCovered; }
            set { isCovered = value; }
        }

        bool isBackground;

        /// <summary>
        /// この画面が背景画面であるかどうかを示します。
        /// <value>true (背景画面である場合)、false (それ以外の場合)。</value>
        /// <remarks>背景画面は、他の画面に覆われた場合であっても ScreenState が TransitionOff に移行しません。</remarks>
        /// </summary>
        public bool IsBackground
        {
            get { return isBackground; }
            protected set { isBackground = value; }
        }

        bool isExclusive;

        /// <summary>
        /// この画面が排他的画面であるかどうかを示します。
        /// <value>true (排他的画面である場合)、false (それ以外の場合)。</value>
        /// <remarks>
        /// 排他的画面は、それが登録される際に他の全ての画面を TransitionOff に移行させます。
        /// その後、それら TransitionOff に移行させられた全画面の削除が完了するまで Update メソッドが呼び出されません。
        /// </remarks>
        /// </summary>
        public bool IsExclusive
        {
            get { return isExclusive; }
            protected set { isExclusive = value; }
        }

        bool isExclusionCompleted;
        public bool IsExclusionCompleted
        {
            get { return isExclusionCompleted; }
            set { isExclusionCompleted = value; }
        }

        IScreenContext screenContext;
        public IScreenContext ScreenContext
        {
            get { return screenContext; }
            set { screenContext = value; }
        }

        PlayerIndex? controllingPlayer;

        /// <summary>
        /// 現在この画面を制御しているプレイヤーのインデックスを取得します。
        /// 接続しているすべてのコントローラーからの入力に応答する場合は null が返されます。
        /// これは、ゲームを特定のプレイヤーのプロフィールにロックするために使用されます。
        /// メイン メニューは、接続された任意のコントローラーからの入力に応答しますが、
        /// メイン メニューから呼び出しされた以降のすべての画面は、選択操作をを実行した
        /// プレイヤーのみが制御できるようになります。
        /// このため、制御プレイヤーがメイン メニューに戻るまでは、他のコントローラー
        /// からの入力は無効になります。
        /// </summary>
        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            set { controllingPlayer = value; }
        }

        IInputDevice inputDevice;

        /// <summary>
        /// IInputDevice を取得します。
        /// </summary>
        public IInputDevice InputDevice
        {
            get { return inputDevice; }
        }

        /// <summary>
        /// IAudioService を取得するヘルパプロパティです。
        /// </summary>
        /// <remarks>
        /// IAudioService がサービスとして登録されていない場合には実行時例外が発生します。
        /// </remarks>
        public IAudioService Audio
        {
            get { return screenContext.GetRequiredService<IAudioService>(); }
        }

        /// <summary>
        /// 現在の GraphicsDevice を取得するヘルパ プロパティです。
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return screenContext.GraphicsDevice; }
        }

        /// <summary>
        /// デフォルトの SpriteBatch を取得するヘルパ プロパティです。
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return screenContext.SpriteBatch; }
        }

        /// <summary>
        /// デフォルトの ContentManager を取得するヘルパ プロパティです。
        /// </summary>
        public ContentManager Content
        {
            get { return screenContext.Content; }
        }

        ContentManager localContent;

        /// <summary>
        /// この Screen でローカルな ContentManager を取得します。
        /// </summary>
        /// <remarks>
        /// この Screen でローカルな ContentManager は、この Screen の削除と同時に破棄されます。
        /// </remarks>
        public ContentManager LocalContent
        {
            get { return localContent; }
        }

        #endregion

        #region LoadContent

        /// <summary>
        /// コンテンツを読み込みます。
        /// </summary>
        public virtual void LoadContent()
        {
            inputDevice = screenContext.GetRequiredService<IInputDeviceService>().InputDevice;
            localContent = new ContentManager(screenContext, "Content");
        }

        #endregion

        #region UnloadContent

        /// <summary>
        /// コンテンツをアンロードします。
        /// </summary>
        public virtual void UnloadContent()
        {
            localContent.Unload();
        }

        #endregion

        #region Update

        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// 画面でユーザー入力を処理できるようになります。このメソッドは Update 
        /// と異なり、画面がアクティブのときにのみ呼び出され、他の何らかの画面が
        /// フォーカスを取得しているときには呼び出されません。
        /// </summary>
        public virtual void HandleInput(GameTime gameTime)
        {
        }

        #endregion

        #region Draw

        /// <summary>
        /// 画面が自身を描画するときに呼び出されます。
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
        }

        #endregion
    }
}
