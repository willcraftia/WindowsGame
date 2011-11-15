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

namespace Willcraftia.Xna.Foundation.Scenes
{

    public sealed class ControllerCollection : KeyedCollection<string, Controller>
    {
        #region Events

        /// <summary>
        /// Controller が追加された場合に発生します。
        /// </summary>
        public event EventHandler<ControllerCollectionEventArgs> ItemAdded;
        /// <summary>
        /// Controller が削除された場合に発生します。
        /// </summary>
        public event EventHandler<ControllerCollectionEventArgs> ItemRemoved;

        ControllerCollectionEventArgs eventArgs = new ControllerCollectionEventArgs();

        #endregion

        #region Fields

        Namer namer = new Namer();

        #endregion

        #region Overrides

        /// <summary>
        /// Controller の Name プロパティをキーとして返します。
        /// もし、Controller の Name プロパティ が null や空文字列ならば、
        /// 自動生成された文字列が Controller の Name プロパティに設定されます。
        /// </summary>
        /// <param name="item">Controller。</param>
        /// <returns>Controller の Name プロパティ。</returns>
        protected override string GetKeyForItem(Controller item)
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                // 名前が設定されていない場合には自動生成した名前を設定します。
                item.Name = namer.CreateName(item);
            }

            return item.Name;
        }

        protected override void InsertItem(int index, Controller item)
        {
            base.InsertItem(index, item);

            OnItemAdded(item);
        }

        protected override void SetItem(int index, Controller item)
        {
            var replacedItem = this[index];

            base.SetItem(index, item);

            OnItemAdded(item);
            OnItemRemoved(replacedItem);
        }

        protected override void RemoveItem(int index)
        {
            var removedItem = this[index];

            base.RemoveItem(index);

            OnItemRemoved(removedItem);
        }

        protected override void ClearItems()
        {
            var removedItems = new List<Controller>();
            removedItems.AddRange(Items);

            base.ClearItems();

            foreach (Controller removedItem in removedItems)
            {
                OnItemRemoved(removedItem);
            }
        }

        void OnItemAdded(Controller item)
        {
            if (ItemAdded != null)
            {
                eventArgs.Item = item;
                ItemAdded(this, eventArgs);
            }
        }

        void OnItemRemoved(Controller item)
        {
            if (ItemRemoved != null)
            {
                eventArgs.Item = item;
                ItemRemoved(this, eventArgs);
            }
        }

        #endregion
    }
}
