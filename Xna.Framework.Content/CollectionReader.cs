#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Willcraftia.Xna.Framework.Content
{
    /// <summary>
    /// Collection に対する ContentTypeReader 実装です。
    /// </summary>
    /// <typeparam name="T">要素型。</typeparam>
    public sealed class CollectionReader<T> : ContentTypeReader<Collection<T>>
    {
        protected override Collection<T> Read(ContentReader input, Collection<T> existingInstance)
        {
            // アセンブリ修飾名を読み込みます。
            var assemblyQualifiedName = input.ReadString();

            if (existingInstance == null)
            {
                // アセンブリ修飾名から Type を取得します。
                var type = Type.GetType(assemblyQualifiedName);
                // コレクションをインスタンス化します。
                existingInstance = Activator.CreateInstance(type) as Collection<T>;
            }

            // コレクションの要素数を読み込みます。
            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                // コレクションの要素を読み込みます。
                var item = input.ReadObject<T>();
                // コレクションに追加します。
                existingInstance.Add(item);
            }
            
            return existingInstance;
        }
    }
}
