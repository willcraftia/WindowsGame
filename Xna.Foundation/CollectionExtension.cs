#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// コレクションの拡張です。
    /// </summary>
    public static class CollectionExtension
    {
        #region Extensions

        public static void FindAll<T>(this IEnumerable<T> collection, Predicate<T> predicate, ICollection<T> results)
        {
            foreach (T item in collection)
            {
                if (predicate(item))
                {
                    results.Add(item);
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }

        #endregion
    }
}
