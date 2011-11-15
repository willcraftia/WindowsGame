#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation
{
    /// <summary>
    /// Vector2 の拡張です。
    /// </summary>
    public static class Vector2Extension
    {
        #region Extension

        /// <summary>
        /// ゼロ ベクトルかどうかを判定します。
        /// </summary>
        /// <param name="vector">Vector2。</param>
        /// <returns>true (ゼロ ベクトルの場合)、false (それ以外の場合)。</returns>
        public static bool IsZero(this Vector2 vector)
        {
            return vector.X == 0.0f && vector.Y == 0.0f;
        }

        /// <summary>
        /// 正規化された Vector2 を取得します。
        /// もしも Vector2 がゼロ ベクトルである場合にはゼロ ベクトルを返します。
        /// </summary>
        /// <param name="vector">Vector2。</param>
        /// <param name="result">正規化された Vector2。</param>
        public static void NormalizeSafe(this Vector2 vector, out Vector2 result)
        {
            var lengthSquared = vector.LengthSquared();
            if (lengthSquared != 0.0f)
            {
                var coeff = 1.0f / (float) Math.Sqrt(lengthSquared);
                result.X = vector.X * coeff;
                result.Y = vector.Y * coeff;
            }
            else
            {
                result = vector;
            }
        }

        #endregion
    }
}
