#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// 現在有効な IDebugConsole にアクセスするためのクラスです。
    /// </summary>
    /// <remarks>
    /// デバッグ機能は自由に呼び出せる事が重要となるため、
    /// このクラスにより静的にアクセスできるようにしています。
    /// </remarks>
    public static class DebugConsole
    {
        static IDebugConsole instance;

        /// <summary>
        /// 現在有効な IDebugConsole を取得または設定します。
        /// </summary>
        /// <value>
        /// IDebugConsole インスタンス(有効な IDebugConsole が存在する場合)、null (それ以外の場合)。
        /// </value>
        /// <remarks>
        /// IDebugConsole を利用するクラスは、このプロパティで取得して利用します。
        /// DebugConsoleComponent は、それが GameComponent として登録される時に自分自身をこのプロパティに設定し、
        /// GameComponent としての登録が解除される時にこのプロパティに null を設定します。
        /// </remarks>
        public static IDebugConsole Instance
        {
            get { return instance; }
            set { instance = value; }
        }
    }
}
