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

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// デバッグ用コンソールのインタフェースです。
    /// </summary>
    public interface IDebugConsole
    {
        /// <summary>
        /// デバッグ コマンドを登録します。
        /// </summary>
        /// <param name="name">コマンド文字列。</param>
        /// <param name="description">コマンドの説明。</param>
        /// <param name="callback">コマンドの入力に対するコールバック。</param>
        void RegisterDebugCommand(string name, string description, DebugCommandExecute callback);

        /// <summary>
        /// デバッグ コマンドの登録を解除します。
        /// </summary>
        /// <param name="name">コマンド文字列。</param>
        void UnregisterDebugCommand(string name);
    }
}
