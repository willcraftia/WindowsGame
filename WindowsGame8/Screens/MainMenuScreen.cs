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

namespace WindowsGame8.Screens
{
    /// <summary>
    /// メイン メニュー画面は、ゲームを起動したときに表示される最初の画面です。
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Fields and Properties

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクターがメニュー コンテンツを設定します。
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            var test1MenuEntry = new MenuEntry(this, "Test #1");
            var exitMenuEntry = new MenuEntry(this, "Exit");

            test1MenuEntry.Selected += OnTest1MenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(test1MenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        #endregion

        #region Handle Input

        void OnTest1MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Audio.SharedAudioManager.CreateSound(SoundConstants.SelectionSound).Play();

            // ローディング画面を作成します。
            var screen = new LoadingScreen();
            screen.Screens.Add(new Test1Screen());
            screen.IsSlowLoading = true;
            screen.ControllingPlayer = e.PlayerIndex;
            ScreenContext.AddScreen(screen);
        }

        /// <summary>
        /// ユーザーがメイン メニューをキャンセルしたときに、サンプルを終了するかどうかを
        /// 尋ねるためのポップ アップ。
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            Audio.SharedAudioManager.CreateSound(SoundConstants.MessageBoxOpenSound).Play();

            const string message = "Are you sure you want to exit this sample?";

            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            confirmExitMessageBox.ControllingPlayer = playerIndex;
            ScreenContext.AddScreen(confirmExitMessageBox);
        }

        /// <summary>
        /// ユーザーが "are you sure you want to exit" メッセージ ボックスで [OK] を
        /// 選択した場合のイベント ハンドラー。
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            Audio.SharedAudioManager.CreateSound(SoundConstants.SelectionSound).Play();

            ScreenContext.Exit();
        }

        #endregion
    }
}
