#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// StringBuilder 用の拡張メソッド宣言用静的クラス
    /// </summary>
    /// <remarks>
    /// XNA GS 3.0 から SpriteFont.DrawString には StringBuilder を
    /// 指定できるようになり、不必要なメモリ確保が少なくなりました。
    /// 
    /// しかし、数字を表示するには 2 つの問題があります。
    /// ひとつは StringBuilder.AppendFormat メソッドを使うと
    /// ボクシングが発生してしまうことです。
    /// もうひとつは、StringBuilder.Append で整数値や float 値を指定すると
    /// 内部でメモリ確保が発生してしまうことです。
    /// 
    /// メモリ確保やボクシングが発生させずに数字表示に最低限必要なフォーマット機能を
    /// このクラスは提供します。
    /// 
    /// StringBuilder の拡張メソッドとして宣言しているので以下のようにして使用します。
    /// 
    /// stringBuilder.AppendNumber(12345);
    /// </remarks>
    public static class StringBuilderExtension
    {
        /// <summary>
        /// NumberFormat クラスの NumberGroupSizes のキッシュ。
        /// </summary>
        static int[] numberGroupSizes = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSizes;

        /// <summary>
        /// 文字列変換用のバッファ。
        /// </summary>
        static char[] numberString = new char[32];

        /// <summary>
        /// 整数を文字列に変換して StringBuilder に追加します。
        /// </summary>
        public static void AppendNumber(this StringBuilder builder, int number)
        {
            AppendNumbernternal(builder, number, 0, AppendNumberOptions.None);
        }

        /// <summary>
        /// 整数を文字列に変換して StringBuilder に追加します。
        /// </summary>
        /// <param s="number"></param>
        /// <param s="options">フォーマット指定オプション。</param>
        public static void AppendNumber(this StringBuilder builder, int number, AppendNumberOptions options)
        {
            AppendNumbernternal(builder, number, 0, options);
        }

        /// <summary>
        /// float 値を文字列に変換して StringBuilder に追加します。
        /// </summary>
        /// <param s="number">変換する数字。</param>
        /// <remarks>小数点以下二桁まで表示する。</remarks>
        public static void AppendNumber(this StringBuilder builder, float number)
        {
            AppendNumber(builder, number, 2, AppendNumberOptions.None);
        }

        /// <summary>
        /// float 値を文字列に変換して StringBuilder に追加します。
        /// </summary>
        /// <param s="number">変換する数字。</param>
        /// <param s="options">フォーマット指定オプション。</param>
        /// <remarks>小数点以下二桁まで表示する。</remarks>
        public static void AppendNumber(this StringBuilder builder, float number, AppendNumberOptions options)
        {
            AppendNumber(builder, number, 2, options);
        }

        /// <summary>
        /// float 値を文字列に変換して StringBuilder に追加します。
        /// </summary>
        /// <param s="number">変換する数字。</param>
        /// <param s="decimalCount">表示する小数点以下の桁数。</param>
        /// <param s="options">フォーマット指定オプション。</param>
        public static void AppendNumber(this StringBuilder builder, float number, int decimalCount, AppendNumberOptions options)
        {
            // NaN, Infinity等の数値の特殊ケース判定
            if (float.IsNaN(number))
            {
                builder.Append("NaN");
            }
            else if (float.IsNegativeInfinity(number))
            {
                builder.Append("-Infinity");
            }
            else if (float.IsPositiveInfinity(number))
            {
                builder.Append("+Infinity");
            }
            else
            {
                int intNumber = (int) (number * (float) Math.Pow(10, decimalCount) + 0.5f);

                AppendNumbernternal(builder, intNumber, decimalCount, options);
            }
        }


        static void AppendNumbernternal(StringBuilder builder, int number, int decimalCount, AppendNumberOptions options)
        {
            // 変換に必要な変数の初期化
            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;

            int idx = numberString.Length;
            int decimalPos = idx - decimalCount;

            if (decimalPos == idx)
                decimalPos = idx + 1;

            int numberGroupIdx = 0;
            int numberGroupCount = numberGroupSizes[numberGroupIdx] + decimalCount;

            bool showNumberGroup = (options & AppendNumberOptions.NumberGroup) != 0;
            bool showPositiveSign = (options & AppendNumberOptions.PositiveSign) != 0;

            bool isNegative = number < 0;
            number = Math.Abs(number);

            // 最小桁から各桁を文字に変換する
            do
            {
                // 小数点の区切り文字(日本では".")の追加
                if (idx == decimalPos)
                {
                    numberString[--idx] = nfi.NumberDecimalSeparator[0];
                }

                // 桁グループ区切り文字(日本では",")の追加
                if (--numberGroupCount < 0 && showNumberGroup)
                {
                    numberString[--idx] = nfi.NumberGroupSeparator[0];

                    if (numberGroupIdx < numberGroupSizes.Length - 1)
                        numberGroupIdx++;

                    numberGroupCount = numberGroupSizes[numberGroupIdx++];
                }

                // 現在の桁を文字に変換してバッファに追加
                numberString[--idx] = (char) ('0' + (number % 10));
                number /= 10;

            } while (number > 0 || decimalPos <= idx);


            // 符号文字を必要なら追加する
            if (isNegative)
            {
                numberString[--idx] = nfi.NegativeSign[0];
            }
            else if (showPositiveSign)
            {
                numberString[--idx] = nfi.PositiveSign[0];
            }

            // 変換結果をStringBuilderに追加する
            builder.Append(numberString, idx, numberString.Length - idx);
        }
    }
}
