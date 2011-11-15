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
    /// GameServiceContainer の拡張です。
    /// </summary>
    public static class GameServiceContainerExtension
    {
        #region Extensions

        /// <summary>
        /// サービスを登録します。
        /// </summary>
        /// <typeparam name="T">サービスの登録に使用するサービスの型。</typeparam>
        /// <param name="container">GameServiceContainer。</param>
        /// <param name="service">登録するサービス。</param>
        public static void AddService<T>(this GameServiceContainer container, T service)
        {
            container.AddService(typeof(T), service);
        }

        /// <summary>
        /// サービスを削除します。
        /// </summary>
        /// <typeparam name="T">サービスの登録で使用したサービスの型。</typeparam>
        /// <param name="container">GameServiceContainer。</param>
        public static void RemoveService<T>(this GameServiceContainer container)
        {
            container.RemoveService(typeof(T));
        }

        #endregion
    }
}
