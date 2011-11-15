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
    /// タイム ルーラのインタフェースです。
    /// </summary>
    public interface ITimeRuler
    {
        /// <summary>
        /// ログを表示するかどうかを示します。
        /// </summary>
        /// <value>
        /// true (ログを表示する場合)、false (それ以外の場合)。
        /// </value>
        bool LogVisible { get; set; }
        
        /// <summary>
        /// バーを表示するかどうかを示します。
        /// </summary>
        /// <value>
        /// true (バーを表示する場合)、false (それ以外の場合)。
        /// </value>
        bool BarVisible { get; set; }

        /// <summary>
        /// サンプル フレーム数を取得または設定します。
        /// </summary>
        int TargetSampleFrames { get; set; }

        /// <summary>
        /// マーカを開始します。
        /// </summary>
        /// <param s="markerName">マーカ名。</param>
        /// <param s="color">バーの色。</param>
        /// <param s="barIndex">バーのインデックス値。</param>
        void BeginMark(string markerName, Color color, int barIndex);

        /// <summary>
        /// マーカを開始します。
        /// </summary>
        /// <param s="markerName">マーカ名。</param>
        /// <param s="color">バーの色。</param>
        void BeginMark(string markerName, Color color);

        /// <summary>
        /// マーカを終了します。
        /// </summary>
        /// <param s="markerName">マーカ名。</param>
        /// <param s="barIndex">バーのインデックス値。</param>
        void EndMark(string markerName, int barIndex);

        /// <summary>
        /// マーカを終了します。
        /// </summary>
        /// <param s="markerName">マーカ名。</param>
        void EndMark(string markerName);

        /// <summary>
        /// ログをリセットします。
        /// </summary>
        void ResetLog();
    }
}
