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
    public interface IScreenContext : IGameContext
    {
        /// <summary>
        /// デフォルトの ContentManager を取得します。
        /// </summary>
        /// <remarks>
        /// デフォルトの ContentManager はゲーム全体で共有されます。
        /// したがって、この ContentManager でコンテンツをロードすると、
        /// 以降、同じコンテンツをロードしようとするとロード済みコンテンツに対する参照が返されます。
        /// </remarks>
        ContentManager Content { get; }

        /// <summary>
        /// デフォルトの SpriteBatch を取得します。
        /// </summary>
        SpriteBatch SpriteBatch { get; }

        /// <summary>
        /// デフォルトの SpriteFont を取得します。
        /// </summary>
        SpriteFont Font { get; }

        /// <summary>
        /// 背景色を取得します。
        /// </summary>
        Color BackgroundColor { get; }

        /// <summary>
        /// Screeen を追加します。
        /// </summary>
        /// <param name="screen">Screen。</param>
        void AddScreen(Screen screen);

        /// <summary>
        /// Screen を削除します。
        /// </summary>
        /// <param name="screen">Screen。</param>
        void RemoveScreen(Screen screen);

        /// <summary>
        /// Screen を削除するように指示します。
        /// </summary>
        /// <param name="screen"></param>
        /// <remarks>
        /// このメソッドでは Screen の描画をフェード オフさせながら Screen を削除します。
        /// </remarks>
        void ExitScreen(Screen screen);

        /// <summary>
        /// 指定された色を基準にした半透明全画面スプライトを描画します。
        /// </summary>
        /// <param name="alpha">アルファ値。</param>
        /// <param name="color">半透明化の基準とする色。</param>
        void FadeBackBuffer(float alpha, Color color);

        /// <summary>
        /// Game を終了させます。
        /// </summary>
        void Exit();
    }
}
