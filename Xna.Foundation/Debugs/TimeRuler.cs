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
    /// 現在有効な ITimeRuler にアクセスするためのクラスです。
    /// </summary>
    /// <remarks>
    /// デバッグ機能は自由に呼び出せる事が重要となるため、
    /// このクラスにより静的にアクセスできるようにしています。
    /// </remarks>
    public static class TimeRuler
    {
        static ITimeRuler instance;

        /// <summary>
        /// 現在有効な ITimeRuler を取得または設定します。
        /// </summary>
        /// <value>
        /// ITimeRuler インスタンス(有効な ITimeRuler が存在する場合)、null (それ以外の場合)。
        /// </value>
        /// <remarks>
        /// ITimeRuler を利用するクラスは、このプロパティで取得して利用します。
        /// TimeRulerComponent は、それが GameComponent として登録される時に自分自身をこのプロパティに設定し、
        /// GameComponent としての登録が解除される時にこのプロパティに null を設定します。
        /// </remarks>
        public static ITimeRuler Instance
        {
            get { return instance; }
            set { instance = value; }
        }
    }
}
