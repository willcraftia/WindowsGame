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
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Framework.Input;
using Willcraftia.Xna.Foundation.Screens;

using WindowsGame8.Inputs;

#endregion

namespace WindowsGame8.Screens
{
    /// <summary>
    /// オプションのメニューを含む画面の基本クラス。ユーザーが上下に移動してエントリを
    /// 選択したり、キャンセルして画面からバック アウトしたりできます。
    /// </summary>
    abstract class MenuScreen : Screen
    {
        #region Fields and Properties

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;

        /// <summary>
        /// メニュー エントリのリストを取得し、派生クラスがメニュー コンテンツを
        /// 追加したり、変更したりできるようにします。
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region Update

        /// <summary>
        /// メニューを更新します。
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // ネストされた各 MenuEntry オブジェクトを更新します。
            for (int i = 0; i < menuEntries.Count; i++)
            {
                var isSelected = (i == selectedEntry);

                menuEntries[i].Update(gameTime, isSelected);
            }

            base.Update(gameTime);
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// ユーザー入力に応答し、選択したエントリの変更、およびメニューの受け入れ
        /// または取り消しを行います。
        /// </summary>
        public override void HandleInput(GameTime gameTime)
        {
            PlayerIndex playerIndex;
            // 前のメニュー エントリに移動しますか?
            if (InputDevice.IsMenuUp(ControllingPlayer, out playerIndex))
            {
                Audio.SharedAudioManager.CreateSound(SoundConstants.CursorSound).Play();

                selectedEntry--;

                if (selectedEntry < 0)
                {
                    selectedEntry = menuEntries.Count - 1;
                }
            }

            // 次のメニュー エントリに移動しますか?
            if (InputDevice.IsMenuDown(ControllingPlayer, out playerIndex))
            {
                Audio.SharedAudioManager.CreateSound(SoundConstants.CursorSound).Play();

                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                {
                    selectedEntry = 0;
                }
            }

            // 選択しているメニュー エントリを決定、もしくはキャンセルさせる。
            // ControllingPlayer 引数に null を渡した場合は、すべてのプレイヤーからの入力を
            // 受け付け、特定のインデックスを渡した場合はそのインデックスに割り当てられている
            // プレイヤーのみから入力を受け付けます。
            // null を渡した場合であっても、InputState ヘルパーは実際に入力を行った
            // プレイヤーを返します。これを OnSelectEntry および OnCancel に渡します。
            // これにより、どのプレイヤーが決定およびキャンセルのアクションをトリガーしたかを
            // 判別することが可能になります。
            if (InputDevice.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (InputDevice.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }

        /// <summary>
        /// ユーザーがメニュー エントリを選択した場合のハンドラー。
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// ユーザーがメニュー エントリをキャンセル場合のハンドラー。
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ScreenContext.ExitScreen(this);
        }

        /// <summary>
        /// このオーバーロードは、MenuEntry イベント ハンドラーとしての OnCancel を
        /// 使用しやすくします。
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        #endregion

        #region Draw

        /// <summary>
        /// メニューを描画します。
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            var position = new Vector2(100, 150);

            // 移行時にメニューを所定の位置にスライドさせ、累乗曲線を使用して、
            // よりおもしろく見えるようにします (これにより、終点に近づくに
            // つれて移動の速度が低下します)。
            var transitionOffset = (float) Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
            {
                position.X -= transitionOffset * 256;
            }
            else if (ScreenState == ScreenState.TransitionOff)
            {
                position.X += transitionOffset * 512;
            }

            SpriteBatch.Begin();

            // 各メニュー エントリを順々に描画します。
            for (int i = 0; i < menuEntries.Count; i++)
            {
                var menuEntry = menuEntries[i];

                var isSelected = (i == selectedEntry);

                menuEntry.Draw(gameTime, isSelected, position);

                position.Y += menuEntry.GetHeight();
            }

            // メニュー タイトルを描画します。
            var titlePosition = new Vector2(426, 80);
            var titleOrigin = ScreenContext.Font.MeasureString(menuTitle) / 2;
            var titleColor = new Color(192, 192, 192) * TransitionAlpha;
            var titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            SpriteBatch.DrawString(
                ScreenContext.Font,
                menuTitle,
                titlePosition,
                titleColor,
                0,
                titleOrigin,
                titleScale,
                SpriteEffects.None,
                0);
            SpriteBatch.End();
        }

        #endregion
    }
}
