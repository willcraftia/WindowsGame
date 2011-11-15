#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// デバッグ用テクスチャ表示機能のインタフェースです。
    /// </summary>
    public interface IDebugMap
    {
        /// <summary>
        /// 表示するテクスチャのリストを取得します。
        /// </summary>
        /// <remarks>
        /// このプロパティが返すリストにテクスチャを設定すると、
        /// デバッグ用テクスチャ機能での描画対象となります。
        /// </remarks>
        IList<Texture2D> Maps { get; }
    }
}
