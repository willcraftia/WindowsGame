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
using Willcraftia.Xna.Foundation;
using Willcraftia.Xna.Foundation.Screens;

#endregion

namespace WindowsGame8.Screens
{
    /// <summary>
    /// このヘルパー クラスは MenuScreen 内の単一エントリを表します。既定では、
    /// エントリのテキスト文字列を描画するだけですが、メニュー エントリをさまざまな方法で
    /// 表示するようにカスタマイズできます。メニュー エントリを選択したときに
    /// 発生するイベントも提供します。
    /// </summary>
    class MenuEntry
    {
        #region Fields and Properties

        /// <summary>
        /// このエントリに対してレンダリングされるテキスト。
        /// </summary>
        string text;

        /// <summary>
        /// カーソルの選択項目から外れ、項目が視覚的にフェードアウトするエフェクトを制御する。
        /// </summary>
        /// <remarks>
        /// 選択が解除された場合、エントリは選択効果から移行します。
        /// </remarks>
        float selectionFade;

        /// <summary>
        /// このメニュー エントリのテキストを取得または設定します。
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        MenuScreen screen;

        /// <summary>
        /// 現在の GraphicsDevice を取得するヘルパ プロパティです。
        /// </summary>
        protected GraphicsDevice GraphicsDevice
        {
            get { return screen.ScreenContext.GraphicsDevice; }
        }

        /// <summary>
        /// デフォルトの SpriteBatch を取得するヘルパ プロパティです。
        /// </summary>
        protected SpriteBatch SpriteBatch
        {
            get { return screen.ScreenContext.SpriteBatch; }
        }

        #endregion

        #region Events

        /// <summary>
        /// メニュー エントリが選択された場合に発生するイベント。
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        /// <summary>
        /// Selected イベントを発生させるメソッド。
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
            {
                Selected(this, new PlayerIndexEventArgs(playerIndex));
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 指定されたテキストを持つ新しいメニュー エントリを構築します。
        /// </summary>
        public MenuEntry(MenuScreen screen, string text)
        {
            this.screen = screen;
            this.text = text;
        }

        #endregion

        #region Update

        /// <summary>
        /// メニュー エントリを更新します。
        /// </summary>
        public virtual void Update(GameTime gameTime, bool isSelected)
        {
            // メニューの選択が変更されると、エントリは新しい状態に直ちに
            // 変更するのではなく、選択した外観と選択を解除した外観の間で徐々に
            // フェード イン/フェード アウトします。
            var fadeSpeed = (float) gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
            {
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            }
            else
            {
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// メニュー エントリを描画します。このメソッドは、オーバーライドにより外観をカスタマイズできます。
        /// </summary>
        public virtual void Draw(GameTime gameTime, bool isSelected, Vector2 position)
        {
            // 選択したエントリを黄色で描画し、それ以外の場合は白で描画します。
            var color = isSelected ? Color.Yellow : Color.White;

            // 選択したメニュー エントリのサイズを鼓動させます。
            var time = gameTime.TotalGameTime.TotalSeconds;

            var pulsate = (float) Math.Sin(time * 6) + 1;

            var scale = 1 + pulsate * 0.05f * selectionFade;

            // 移行時にテキストをフェード アウトさせるためにアルファ値を徐々に減らします。
            color *= screen.TransitionAlpha;

            // テキストを描画し、各行の中央に位置揃えします。
            var origin = new Vector2(0, screen.ScreenContext.Font.LineSpacing / 2);

            SpriteBatch.DrawString(screen.ScreenContext.Font, text, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// このメニュー エントリに必要なスペースの大きさを照会します。
        /// </summary>
        public virtual int GetHeight()
        {
            return screen.ScreenContext.Font.LineSpacing;
        }

        #endregion
    }
}
