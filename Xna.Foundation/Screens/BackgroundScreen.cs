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

#endregion

namespace Willcraftia.Xna.Foundation.Screens
{
    /// <summary>
    /// 背景画面描画のための Screen です。
    /// </summary>
    /// <remarks>
    /// BackgroundScreen は、プロパティで指定されるテクスチャをそのまま背景へ描画します。
    /// </remarks>
    public class BackgroundScreen : Screen
    {
        #region Fields

        string backgroundTextureAssetName = "Screens/Textures/DefaultBackground";
        public string BackgroundTextureAssetName
        {
            get { return backgroundTextureAssetName; }
            set { backgroundTextureAssetName = value; }
        }

        Texture2D backgroundTexture;

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        public BackgroundScreen()
        {
            // 背景フラグを ON にします。
            IsBackground = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            base.LoadContent();

            // 背景テクスチャはサイズが大きいので、
            // Screen でローカルな ContentManager でロードし、
            // Screen の削除と同時にアンロードされるようにしておきます。
            backgroundTexture = LocalContent.Load<Texture2D>(backgroundTextureAssetName);
        }
        
        #endregion

        #region Draw

        /// <summary>
        /// 背景画面を描画します。
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            var whiteVector = Vector3.One;
            var backgroundVector = ScreenContext.BackgroundColor.ToVector3();

            // 背景色から白の間で線形補間を行い、最終的な色を決定します。
            Vector3 resultVector;
            Vector3.Lerp(ref backgroundVector, ref whiteVector, TransitionAlpha, out resultVector);

            var color = new Color(resultVector) * TransitionAlpha;

            SpriteBatch.Begin();
            SpriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, color);
            SpriteBatch.End();
        }

        #endregion
    }
}
