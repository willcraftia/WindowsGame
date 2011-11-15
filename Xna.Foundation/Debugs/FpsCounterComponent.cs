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
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// FPS を計測して表示する DebugComponent。
    /// </summary>
    public sealed class FpsCounterComponent : DebugComponent
    {
        #region Fields and Properties

        Color windowColor = new Color(0, 0, 0, 128);
        
        /// <summary>
        /// ウィンドウ背景色を取得または設定します。
        /// </summary>
        public Color WindowColor
        {
            get { return windowColor; }
            set { windowColor = value; }
        }

        Color fontColor = Color.White;
        
        /// <summary>
        /// 文字色を取得または設定します。
        /// </summary>
        public Color FontColor
        {
            get { return fontColor; }
            set { fontColor = value; }
        }

        Color runningSlowlyFontColor = Color.Red;
        
        /// <summary>
        /// 実行速度低下時の文字色を取得または設定します。
        /// </summary>
        public Color RunningSlowlyFontColor
        {
            get { return runningSlowlyFontColor; }
            set { runningSlowlyFontColor = value; }
        }

        float fps;
        int sampleFrames;
        TimeSpan sampleSpan = TimeSpan.FromSeconds(1);
        Stopwatch stopwatch = Stopwatch.StartNew();
        StringBuilder stringBuilder = new StringBuilder(16);

        #endregion

        #region Constructors

        public FpsCounterComponent(Game game)
            : base(game)
        {
            stringBuilder.Length = 0;
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            if (stopwatch.Elapsed > sampleSpan)
            {
                // FPS の更新と次の測定期間の開始
                fps = (float) sampleFrames / (float) stopwatch.Elapsed.TotalSeconds;

                stopwatch.Reset();
                stopwatch.Start();
                sampleFrames = 0;

                // 表示文字列の更新
                stringBuilder.Length = 0;
                stringBuilder.Append("FPS: ");
                stringBuilder.AppendNumber(fps);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            sampleFrames++;

            // FPS 表示の周りに半透明の黒い矩形のサイズ計算と配置
            var size = Font.MeasureString("X");
            var rect = new Rectangle(0, 0, (int) (size.X * 14f), (int) (size.Y * 1.3f));

            var vp = GraphicsDevice.Viewport;
            var layout = new Layout(new Rectangle(vp.X, vp.Y, vp.Width, vp.Height), vp.TitleSafeArea);
            rect = layout.Place(rect, 0.01f, 0.01f, LayoutAlignment.TopLeft);

            // FPS 表示を矩形の中で配置
            size = Font.MeasureString(stringBuilder);
            layout.ClientArea = rect;
            var position = layout.Place(size, 0.0f, 0.1f, LayoutAlignment.Center);

            var fColor = !gameTime.IsRunningSlowly ? fontColor : runningSlowlyFontColor;

            SpriteBatch.Begin();
            SpriteBatch.Draw(FillTexture, rect, windowColor);
            SpriteBatch.DrawString(Font, stringBuilder, position, fColor);
            SpriteBatch.End();
        }
    }
}
