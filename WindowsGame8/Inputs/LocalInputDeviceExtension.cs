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
using Willcraftia.Xna.Framework.Input;

namespace WindowsGame8.Inputs
{
    public static class LocalInputDeviceExtension
    {
        /// <summary>
        /// "メニュー項目の決定" 入力アクションを確認します。
        /// result パラメーターは、どのプレイヤーの入力を読み込むかを指定します。
        /// このパラメーターが null の場合、すべてのプレイヤーからの入力を読み込みます。
        /// ボタン押下が検出されると、ボタンを押したプレイヤーのインデックスが
        /// 出力パラメーター result としてレポートされます。
        /// </summary>
        public static bool IsMenuSelect(this IInputDevice inputService, PlayerIndex? playerIndex, out PlayerIndex result)
        {
            return inputService.IsKeyPressed(Keys.Space, playerIndex, out result) ||
                inputService.IsKeyPressed(Keys.Enter, playerIndex, out result) ||
                inputService.IsButtonPressed(Buttons.A, playerIndex, out result) ||
                inputService.IsButtonPressed(Buttons.Start, playerIndex, out result);
        }

        /// <summary>
        /// "メニューの取り消し" 入力アクションを確認します。
        /// playerIndex パラメーターは、どのプレイヤーの入力を読み込むかを指定します。
        /// このパラメーターが null の場合、すべてのプレイヤーからの入力を読み込みます。
        /// ボタン押下が検出されると、ボタンを押したプレイヤーのインデックスが
        /// 出力パラメーター result としてレポートされます。
        /// </summary>
        public static bool IsMenuCancel(this IInputDevice inputService, PlayerIndex? playerIndex, out PlayerIndex result)
        {
            return inputService.IsKeyPressed(Keys.Escape, playerIndex, out result) ||
                inputService.IsButtonPressed(Buttons.B, playerIndex, out result) ||
                inputService.IsButtonPressed(Buttons.Back, playerIndex, out result);
        }

        /// <summary>
        /// "メニュー カーソルの上移動" 入力アクションを確認します。
        /// playerIndex パラメーターは、どのプレイヤーの入力を読み込むかを指定します。
        /// このパラメーターが null の場合、すべてのプレイヤーからの入力を読み込みます。
        /// </summary>
        public static bool IsMenuUp(this IInputDevice inputService, PlayerIndex? playerIndex, out PlayerIndex result)
        {
            return inputService.IsKeyPressed(Keys.Up, playerIndex, out result) ||
                   inputService.IsButtonPressed(Buttons.DPadUp, playerIndex, out result) ||
                   inputService.IsButtonPressed(Buttons.LeftThumbstickUp, playerIndex, out result);
        }


        /// <summary>
        /// "メニュー カーソルの下移動" 入力アクションを確認します。
        /// playerIndex パラメーターは、どのプレイヤーの入力を読み込むかを指定します。
        /// このパラメーターが null の場合、すべてのプレイヤーからの入力を読み込みます。
        /// </summary>
        public static bool IsMenuDown(this IInputDevice inputService, PlayerIndex? playerIndex, out PlayerIndex result)
        {
            return inputService.IsKeyPressed(Keys.Down, playerIndex, out result) ||
                   inputService.IsButtonPressed(Buttons.DPadDown, playerIndex, out result) ||
                   inputService.IsButtonPressed(Buttons.LeftThumbstickDown, playerIndex, out result);
        }

        /// <summary>
        /// "ゲームのポーズ" 入力アクションを確認します。
        /// playerIndex パラメーターは、どのプレイヤーの入力を読み込むかを指定します。
        /// このパラメーターが null の場合、すべてのプレイヤーからの入力を読み込みます。
        /// </summary>
        public static bool IsPauseGame(this IInputDevice inputService, PlayerIndex? playerIndex, out PlayerIndex result)
        {
            return inputService.IsKeyPressed(Keys.Escape, playerIndex, out result) ||
                   inputService.IsButtonPressed(Buttons.Back, playerIndex, out result) ||
                   inputService.IsButtonPressed(Buttons.Start, playerIndex, out result);
        }
    }
}
