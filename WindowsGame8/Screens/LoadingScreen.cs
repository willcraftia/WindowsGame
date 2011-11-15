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
using Willcraftia.Xna.Foundation;
using Willcraftia.Xna.Foundation.Screens;

namespace WindowsGame8.Screens
{
    /// <summary>
    /// ローディング画面は、メニュー システムとゲーム自体の間の移行を調整します。
    /// 通常、ある画面は、次の画面がオンに移行すると同時にオフに移行しますが、
    /// データの読み込みに長時間かかる可能性のある大規模な移行では、ゲームの読み込みを
    /// 開始する前にメニュー システムを完全に終了する必要があります。この操作を
    /// 実行するには、次の手順に従います。
    /// 
    /// - 既存のすべての画面をオフに移行するように指示します。
    /// - ローディング画面をアクティブ化します。同時にオンに移行します。
    /// - ローディング画面が前の画面の状態を監視します。
    /// - 完全にオフに移行したことを確認すると、次の実際の画面をアクティブ化します。
    /// この場合、データの読み込みに長時間かかる場合があります。この読み込みが
    /// 行われている間は、ローディング画面だけが表示されます。
    /// </summary>
    class LoadingScreen : Screen
    {
        #region Fields and Properties

        bool isSlowLoading;
        public bool IsSlowLoading
        {
            get { return isSlowLoading; }
            set { isSlowLoading = value; }
        }

        List<Screen> screens = new List<Screen>();
        public IList<Screen> Screens
        {
            get { return screens; }
        }

        #endregion

        #region Constructors

        public LoadingScreen()
        {
            IsExclusive = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!IsExclusionCompleted)
            {
                // 他の画面がまだ終了していなければ処理を終えます。
                return;
            }

            // ロード画面を終了させゲームの読み込みを開始させます。
            ScreenContext.RemoveScreen(this);

            foreach (var screen in screens)
            {
                if (screen != null)
                {
                    screen.ControllingPlayer = ControllingPlayer;

                    // ここで各画面のロードが行われるので、
                    // その間は Loding... 画面が描画されたままになります。
                    ScreenContext.AddScreen(screen);
                }
            }

            // 読み込みが完了したら、ResetElapsedTime を使用して、非常に
            // 長いフレームを完了したことと、キャッチアップしようとする
            // 必要がないことを、ゲームのタイミング メカニズムに指示します。
            ScreenContext.ResetElapsedTime();
        }

        /// <summary>
        /// ローディング画面を描画します。
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // ゲームプレイ画面は読み込みにしばらく時間がかかるので、読み込みが
            // 行われている間にローディング メッセージが表示されます。ただし、
            // メニューは非常に速やかに読み込まれるので、ゲームからメニューに戻るほんの 
            // 1 秒足らずの間にこのメッセージをちらっと見せることには意味はありません。
            // loadingSlow パラメーターは読み込みにかかる時間を示すので、true の場合は
            // ローディング メッセージを描画します。
            if (isSlowLoading)
            {
                const string message = "Loading...";

                // ビューポート内のテキストを中央揃えします。
                var viewport = GraphicsDevice.Viewport;
                var viewportSize = new Vector2(viewport.Width, viewport.Height);
                var textSize = ScreenContext.Font.MeasureString(message);
                var textPosition = (viewportSize - textSize) / 2;

                var color = Color.White * TransitionAlpha;

                // テキストを描画します。
                SpriteBatch.Begin();
                SpriteBatch.DrawString(ScreenContext.Font, message, textPosition, color);
                SpriteBatch.End();
            }
        }

        #endregion
    }
}
