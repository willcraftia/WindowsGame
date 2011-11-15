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

namespace WindowsGame8.Screens
{
    /// <summary>
    /// イベントをトリガーしたプレイヤーのインデックスを含むカスタム イベント引数。
    /// この引数は MenuEntry.Selected イベントによって使用されます。
    /// </summary>
    class PlayerIndexEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクター。
        /// </summary>
        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        /// <summary>
        /// このイベントをトリガーしたプレイヤーのインデックスを取得します。
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        PlayerIndex playerIndex;
    }
}
