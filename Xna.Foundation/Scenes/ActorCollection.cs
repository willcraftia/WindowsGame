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
    public sealed class ActorCollection<T> : KeyedCollection<string, T> where T : Actor
    {
        #region Events

        public event EventHandler<ActorCollectionEventArgs> ItemAdded;
        public event EventHandler<ActorCollectionEventArgs> ItemRemoved;

        ActorCollectionEventArgs eventArgs = new ActorCollectionEventArgs();

        #endregion

        #region Overrides

        protected override string GetKeyForItem(T item)
        {
            return item.Name;
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            OnItemAdded(item);
        }

        protected override void SetItem(int index, T item)
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
            var removedItems = new List<T>();
            removedItems.AddRange(Items);

            base.ClearItems();

            foreach (T item in removedItems)
            {
                OnItemRemoved(item);
            }
        }

        void OnItemAdded(T item)
        {
            if (ItemAdded != null)
            {
                eventArgs.Item = item;
                ItemAdded(this, eventArgs);
            }
        }

        void OnItemRemoved(T item)
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
