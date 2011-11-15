#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// 線形フォグの設定を表す構造体です。
    /// </summary>
    public struct LinearFog
    {
        #region Fields

        /// <summary>
        /// フォグ色です。
        /// </summary>
        public Vector3 Color;

        /// <summary>
        /// フォグを開始する距離です。
        /// </summary>
        public float Start;

        /// <summary>
        /// フォグを終了する距離です。
        /// </summary>
        public float End;

        /// <summary>
        /// フォグが有効がどうかを示します。
        /// true (フォグが有効)、false (それ以外の場合)。
        /// </summary>
        public bool Enabled;

        #endregion

        #region Predefined

        /// <summary>
        /// デフォルトのフォグ設定です。
        /// </summary>
        public static LinearFog Default
        {
            get
            {
                return new LinearFog()
                {
                    Color = Vector3.One,
                    Start = 500.0f,
                    End = 1000.0f,
                    Enabled = true
                };
            }
        }

        /// <summary>
        /// フォグ無しを表すフォグ設定です。
        /// </summary>
        public static LinearFog None
        {
            get
            {
                return new LinearFog()
                {
                    Color = Vector3.Zero,
                    Start = 0.0f,
                    End = 0.0f,
                    Enabled = false
                };
            }
        }

        #endregion
    }
}
