#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation
{
    public sealed class NamedTexture2DCollection : KeyedCollection<string, Texture2D>
    {
        Namer namer = new Namer();

        /// <summary>
        /// Texture2D の Name プロパティをキーとして返します。
        /// もし、Texture2D の Name プロパティ が null や空文字列ならば、
        /// 自動生成された文字列が Texture2D の Name プロパティに設定されます。
        /// </summary>
        /// <param name="item">Texture2D。</param>
        /// <returns>Texture2D の Name プロパティ。</returns>
        protected override string GetKeyForItem(Texture2D item)
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                // 名前が設定されていない場合には自動生成した名前を設定します。
                item.Name = namer.CreateName(item);
            }

            return item.Name;
        }
    }
}
