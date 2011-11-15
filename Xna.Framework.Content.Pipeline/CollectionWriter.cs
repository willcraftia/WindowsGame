#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

#endregion

namespace Willcraftia.Xna.Framework.Content.Pipeline
{
    /// <summary>
    /// Collection に対する ContentTypeWriter 実装です。
    /// </summary>
    /// <typeparam name="T">要素型。</typeparam>
    [ContentTypeWriter]
    public sealed class CollectionWriter<T> : ContentTypeWriter<Collection<T>>
    {
        protected override void Write(ContentWriter output, Collection<T> value)
        {
            // アセンブリ修飾名を書き込みます。
            output.Write(value.GetType().AssemblyQualifiedName);
            // コレクションの要素数を書き込みます。
            output.Write(value.Count);
            foreach (var item in value)
            {
                // コレクションの各要素を書き込みます。
                output.WriteObject<T>(item);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            //return "Willcraftia.Xna.Framework.Content.CollectionReader`1[[" + typeof(T).AssemblyQualifiedName + "]]" +
            //    ", Willcraftia.Xna.Framework.Content, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            return typeof(CollectionReader<T>).AssemblyQualifiedName;
        }
    }
}
