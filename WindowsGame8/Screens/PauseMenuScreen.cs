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
using Willcraftia.Xna.Foundation.Screens;

#endregion

namespace WindowsGame8.Screens
{
    /// <summary>
    /// ポーズ メニューはメイン ゲームの上に立ち上がり、このメニューから
    /// プレイヤーはゲームの再開または終了を選択できます。
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Constructors

        /// <summary>
        /// コンストラクター
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // ゲーム上にポーズ メニューが表示されている場合、ゲームを
            // オフに移行する必要がないことを示すフラグ。
            IsPopup = true;

            // メニュー エントリを作成します。
            var resumeGameMenuEntry = new MenuEntry(this, "Resume Game");
            var quitGameMenuEntry = new MenuEntry(this, "Quit Game");

            // メニュー イベント ハンドラーを登録します。
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // エントリをメニューに追加します。
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// [Quit Game] メニュー エントリが選択された場合のイベント ハンドラー。
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Audio.SharedAudioManager.CreateSound(SoundConstants.MessageBoxOpenSound).Play();

            const string message = "Are you sure you want to quit this game?";

            var confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            confirmQuitMessageBox.ControllingPlayer = ControllingPlayer;
            ScreenContext.AddScreen(confirmQuitMessageBox);
        }

        /// <summary>
        /// ユーザーが "are you sure you want to quit? (終了してもよろしいですか?)" 
        /// メッセージ ボックスを選択した場合のイベント ハンドラー。
        /// このイベント ハンドラーは、ローディング画面を表示してゲームから
        /// メイン メニュー画面に戻ります。
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            Audio.SharedAudioManager.CreateSound(SoundConstants.SelectionSound).Play();

            // ローディング画面を作成します。
            var screen = new LoadingScreen();
            screen.Screens.Add(new BackgroundScreen());
            screen.Screens.Add(new MainMenuScreen());
            screen.ControllingPlayer = e.PlayerIndex;
            ScreenContext.AddScreen(screen);
        }

        #endregion

        #region Draw

        /// <summary>
        /// ポーズ メニュー画面を描画します。リストの中では直下にある
        /// ゲームプレイ画面が暗くなり、基底 MenuScreen.Draw に処理が渡されます。
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenContext.FadeBackBuffer(TransitionAlpha * 2 / 3, Color.Black);

            base.Draw(gameTime);
        }

        #endregion
    }
}
