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

namespace Willcraftia.Xna.Foundation
{
    /// <summary>
    /// StringBuilder 拡張メソッド用オプションです。
    /// </summary>
    [Flags]
    public enum AppendNumberOptions
    {
        // 通常フォーマット。
        None = 0,

        // 正の値の時にも "+" をつける。
        PositiveSign = 1,

        // 3 桁毎に "," を表示する。
        NumberGroup = 2,
    }
}
