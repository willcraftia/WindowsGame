#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Screens
{
    /// <summary>
    /// ScreenContainer は Screen を管理する GameComponent です。
    /// 画面のスタックを管理し、それぞれの画面の Update および Draw メソッドを適切なタイミングで呼び出し、
    /// 最上位のアクティブな画面に入力を自動的にルーティングします。
    /// </summary>
    public sealed class ScreenContainer : DrawableGameComponent, IScreenContext
    {
        #region Fields and Properties

        List<Screen> screens = new List<Screen>();
        Stack<Screen> screensToUpdate = new Stack<Screen>();

        Texture2D blankTexture;

        bool isInitialized;

        bool traceEnabled;

        /// <summary>
        /// true の場合、ScreenService は更新されるたびにすべての画面の
        /// リストを出力します。これは、すべての画面が適切な時点で
        /// 追加/削除されていることを確認するのに便利です。
        /// </summary>
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 新しい ScreenService コンポーネントを構築します。
        /// </summary>
        /// <param name="game"></param>
        public ScreenContainer(Game game)
            : base(game)
        {
        }

        #endregion

        #region Initialize

        /// <summary>
        /// ScreenService コンポーネントを初期化します。
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }

        #endregion

        #region LoadContent

        /// <summary>
        /// コンテンツを読み込みます。
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("Screens/Fonts/Default");
            blankTexture = Game.Content.Load<Texture2D>("Screens/Textures/Blank");

            // 各画面インスタンスがそれぞれのコンテンツを読み込むように指示します。
            foreach (var screen in screens)
            {
                screen.LoadContent();
            }
        }

        #endregion

        #region UnloadContent

        /// <summary>
        /// グラフィック コンテンツをアンロードします。
        /// </summary>
        protected override void UnloadContent()
        {
            // 各画面がそれぞれのコンテンツをアンロードするように指示します。
            foreach (var screen in screens)
            {
                screen.UnloadContent();
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// 各画面でロジックの更新を実行できるようにします。
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // ある画面の更新の過程で別の画面を追加または削除する場合の混乱を
            // 防ぐため、マスター画面リストのコピーを作成します。
            screensToUpdate.Clear();

            foreach (var screen in screens)
            {
                screensToUpdate.Push(screen);
            }

            var otherScreenHasFocus = !Game.IsActive;
            var coveredByOtherScreen = false;

            // 更新を待機する画面が存在する限りループします。
            while (screensToUpdate.Count > 0)
            {
                // 最上位の画面を待機リストからポップ オフします。
                var screen = screensToUpdate.Pop();

                // 他の画面に覆われているかどうかを設定します
                screen.IsCovered = coveredByOtherScreen;

                // フォーカスを持てるかどうかを判定して設定します
                screen.HasFocus = false;
                if (!coveredByOtherScreen &&
                    (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active))
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HasFocus = true;
                        otherScreenHasFocus = true;
                    }
                }

                // 排他的画面ならば排他が完了しているかどうかを判定して設定します。
                screen.IsExclusionCompleted = false;
                if (screen.IsExclusive && (screens.Count == 1 && screen.ScreenState == ScreenState.Active))
                {
                    screen.IsExclusionCompleted = true;
                }

                // 画面の状態遷移を更新します。
                UpdateTransition(gameTime, screen);

                // 画面を更新します。
                screen.Update(gameTime);

                // フォーカスを持つ場合は入力を処理します
                if (screen.HasFocus)
                {
                    screen.HandleInput(gameTime);
                }

                // この画面がアクティブな非ポップアップ画面の場合、
                // 以降の画面がこの画面に覆われるように設定します。
                if (!screen.IsPopup &&
                    (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active))
                {
                    coveredByOtherScreen = true;
                }
            }

            // デバッグ出力で画面のリストを出力するかを確認する。
            if (traceEnabled)
            {
                TraceScreens();
            }
        }

        void UpdateTransition(GameTime gameTime, Screen screen)
        {
            if (screen.IsExiting)
            {
                // 画面が完全に消去される場合は、まず「オフへ移行中」に相当する
                // TransitionOff の状態に設定されます。
                screen.ScreenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, screen, screen.TransitionOffTime, 1))
                {
                    // 移行が完了すると、画面が ScreenService から削除されます。
                    RemoveScreen(screen);
                }
            }
            else if (!screen.IsBackground && screen.IsCovered)
            {
                // isBackground = true ならば、画面が他の画面に覆われると TransitionOff の状態にします。
                // isBackground = false は常に他の画面に覆われる前提の画面です。
                if (UpdateTransition(gameTime, screen, screen.TransitionOffTime, 1))
                {
                    // まだオフへの移行中です。
                    screen.ScreenState = ScreenState.TransitionOff;
                }
                else
                {
                    // 移行が完了し、画面が隠蔽されます。
                    screen.ScreenState = ScreenState.Hidden;
                }
            }
            else
            {
                // 最後が「オフへ移行中」でない場合は「オンへ移行中」の状態になり
                // 移行が完了した時点でアクティブ画面となります。
                if (UpdateTransition(gameTime, screen, screen.TransitionOnTime, -1))
                {
                    // まだオンへの移行中です。
                    screen.ScreenState = ScreenState.TransitionOn;
                }
                else
                {
                    // 移行が完了し、画面がアクティブになります。
                    screen.ScreenState = ScreenState.Active;
                }
            }
        }

        /// <summary>
        /// 画面の移行位置を更新するためのヘルパー。
        /// </summary>
        bool UpdateTransition(GameTime gameTime, Screen screen, TimeSpan time, int direction)
        {
            // どの程度移動する必要がありますか?
            float transitionDelta = 1;
            if (time != TimeSpan.Zero)
            {
                transitionDelta = (float) (gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);
            }

            // 移行位置を更新します。
            screen.TransitionPosition += transitionDelta * direction;

            // 移行の最後に到達しましたか?
            if (((direction < 0) && (screen.TransitionPosition <= 0)) ||
                ((direction > 0) && (screen.TransitionPosition >= 1)))
            {
                screen.TransitionPosition = MathHelper.Clamp(screen.TransitionPosition, 0, 1);
                return false;
            }

            // transitionPosition に到達していない場合、まだ移行が行われています。
            return true;
        }

        /// <summary>
        /// すべての画面のリストをデバッグ用に出力します。
        /// </summary>
        void TraceScreens()
        {
            var screenNames = new List<string>();

            foreach (var screen in screens)
            {
                screenNames.Add(screen.GetType().Name);
            }

            Trace.WriteLine(string.Join(", ", screenNames.ToArray()));
        }

        #endregion

        #region Draw

        /// <summary>
        /// 各画面が自身を描画するように指示します。
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            foreach (var screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                {
                    continue;
                }

                screen.Draw(gameTime);
            }
        }

        #endregion

        #region IScreenContext

        public ContentManager Content
        {
            get { return Game.Content; }
        }

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

        Color backgroundColor = Color.Black;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        public void AddScreen(Screen screen)
        {
            screen.IsExiting = false;

            // IScreenContext を設定します。
            screen.ScreenContext = this;

            // 排他的画面が登録されたら他の全ての画面を TransitionOff に移行させます。
            if (screen.IsExclusive)
            {
                foreach (var otherScreen in screens)
                {
                    ExitScreen(otherScreen);
                }
            }

            // グラフィック デバイスを備えている場合、画面がコンテンツを
            // 読み込むように指示します。
            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            screens.Remove(screen);

            // グラフィック デバイスを備えている場合、画面がコンテンツを
            // アンロードするように指示します。
            if (isInitialized)
            {
                screen.UnloadContent();
            }
        }

        public void ExitScreen(Screen screen)
        {
            if (screen.TransitionOffTime == TimeSpan.Zero)
            {
                // 画面の移行時間がゼロに設定されている場合は、すぐに削除されます。
                RemoveScreen(screen);
            }
            else
            {
                // それ以外の場合、オフに移行してから終了するようにフラグを設定します。
                screen.IsExiting = true;
            }
        }

        public void FadeBackBuffer(float alpha)
        {
            FadeBackBuffer(alpha, backgroundColor);
        }

        public void FadeBackBuffer(float alpha, Color color)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(blankTexture, GraphicsDevice.Viewport.Bounds, color * alpha);
            SpriteBatch.End();
        }

        public void Exit()
        {
            Game.Exit();
        }

        #endregion

        #region IGameContext

        public void ResetElapsedTime()
        {
            Game.ResetElapsedTime();
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
