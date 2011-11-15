#region Using

using System;
using System.Collections.Generic;
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
    /// Vector3 の拡張です。
    /// </summary>
    public static class Vector3Extension
    {
        #region Utility

        /// <summary>
        /// 正規化された Vector3 を取得します。
        /// もしも Vector3 がゼロ ベクトルである場合にはゼロ ベクトルを返します。
        /// </summary>
        /// <param name="vector">Vector3。</param>
        /// <param name="result">正規化された Vector3。</param>
        public static void NormalizeSafe(ref Vector3 vector, out Vector3 result)
        {
            var lengthSquared = vector.LengthSquared();
            if (lengthSquared != 0.0f)
            {
                var coeff = 1.0f / (float) Math.Sqrt(lengthSquared);
                result.X = vector.X * coeff;
                result.Y = vector.Y * coeff;
                result.Z = vector.Z * coeff;
            }
            else
            {
                result = vector;
            }
        }

        public const char DefaultParseSeperator = ' ';

        /// <summary>
        /// 文字列を解析して Vector3 を生成します。
        /// </summary>
        /// <param name="value">解析される文字列。</param>
        /// <returns>生成された Vector3。</returns>
        public static Vector3 Parse(string value)
        {
            Vector3 result;
            Parse(value, out result);
            return result;
        }

        /// <summary>
        /// 文字列を解析して Vector3 を生成します。
        /// </summary>
        /// <param name="value">解析される文字列。</param>
        /// <param name="result">生成された Vector3。</param>
        public static void Parse(string value, out Vector3 result)
        {
            Parse(value, DefaultParseSeperator, out result);
        }

        /// <summary>
        /// 文字列を解析して Vector3 を生成します。
        /// </summary>
        /// <param name="value">解析される文字列。</param>
        /// <param name="separator">区切り文字。</param>
        /// <returns>生成された Vector3。</returns>
        public static Vector3 Parse(string value, char separator)
        {
            Vector3 result;
            Parse(value, separator, out result);
            return result;
        }

        /// <summary>
        /// 文字列を解析して Vector3 を生成します。
        /// </summary>
        /// <param name="value">解析される文字列。</param>
        /// <param name="separator">区切り文字。</param>
        /// <param name="result">生成された Vector3。</param>
        public static void Parse(string value, char separator, out Vector3 result)
        {
            var values = value.Split(separator);
            Parse(values, out result);
        }

        /// <summary>
        /// 文字列を解析して Vector3 を生成します。
        /// </summary>
        /// <param name="values">解析される文字列。</param>
        /// <returns>生成された Vector3。</returns>
        public static Vector3 Parse(string[] values)
        {
            Vector3 result;
            Parse(values, out result);
            return result;
        }

        /// <summary>
        /// 各文字列を数値として Vector3 を生成します。
        /// </summary>
        /// <param name="values">Vector3 の成分を表す文字列。</param>
        /// <param name="result">生成された Vector3。</param>
        public static void Parse(string[] values, out Vector3 result)
        {
            if (values == null || values.Length == 0)
            {
                result = Vector3.Zero;
            }
            else
            {
                result = new Vector3();
                result.X = float.Parse(values[0]);
                if (2 <= values.Length)
                {
                    result.Y = float.Parse(values[1]);
                }
                if (3 <= values.Length)
                {
                    result.Z = float.Parse(values[2]);
                }
            }
        }

        #endregion

        #region Extension

        /// <summary>
        /// ゼロ ベクトルかどうかを判定します。
        /// </summary>
        /// <param name="vector">Vector3。</param>
        /// <returns>true (ゼロ ベクトルの場合)、false (それ以外の場合)。</returns>
        public static bool IsZero(this Vector3 vector)
        {
            return vector.X == 0.0f && vector.Y == 0.0f && vector.Z == 0.0f;
        }

        /// <summary>
        /// 指定の Matrix で Vector3 を同次変換します。
        /// </summary>
        /// <param name="vector">Vector3。</param>
        /// <param name="matrix">Matrix。</param>
        /// <returns>同次変換された Vector3。</returns>
        public static Vector3 TransformCoord(this Vector3 vector, ref Matrix matrix)
        {
            Vector3 result;
            TransformCoord(vector, ref matrix, out result);
            return result;
        }

        /// <summary>
        /// 指定の Matrix で Vector3 を同次変換します。
        /// </summary>
        /// <param name="vector">Vector3。</param>
        /// <param name="matrix">Matrix。</param>
        /// <param name="playerIndex">同次変換された Vector3。</param>
        public static void TransformCoord(this Vector3 vector, ref Matrix matrix, out Vector3 result)
        {
            Vector4 transformed;
            Vector4.Transform(ref vector, ref matrix, out transformed);

            Vector4 homogeneous;
            Vector4.Divide(ref transformed, transformed.W, out homogeneous);

            result = new Vector3(homogeneous.X, homogeneous.Y, homogeneous.Z);
        }

        #endregion
    }
}
