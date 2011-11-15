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

namespace Willcraftia.Xna.Foundation
{
    /// <summary>
    /// 型を基準にオブジェクト名を生成するクラスです。
    /// このクラスが生成するオブジェクト名は、このクラス内で一意となる文字列です。
    /// </summary>
    public sealed class Namer
    {
        Dictionary<Type, int> sequences = new Dictionary<Type, int>();

        /// <summary>
        /// 指定されたオブジェクトの型について、このインスタンス内で一意となる名前を生成します。
        /// </summary>
        /// <param name="target">対象のオブジェクト。</param>
        /// <returns>生成された名前。</returns>
        /// <remarks>
        /// 生成される名前は、<code>target.GetType().FullName + "_" + 連番</code>です。
        /// 連番は、target.GetType().FullName 毎にインクリメントされる 0 から開始する値です。
        /// </remarks>
        public string CreateName(object target)
        {
            var type = target.GetType();

            int sequence;
            if (!sequences.TryGetValue(type, out sequence))
            {
                sequence = 0;
                sequences.Add(type, sequence);
            }

            var name = type.FullName + "_" + sequence;

            sequence++;
            sequences[type] = sequence;

            return name;
        }
    }
}
