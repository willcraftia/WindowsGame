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
    /// IServiceProvider の拡張です。
    /// </summary>
    public static class ServiceProviderExtension
    {
        /// <summary>
        /// サービスを取得します。
        /// サービスが存在しない場合には ServiceNotFoundException を発生させます。
        /// </summary>
        /// <param name="provider">IServiceProvider。</param>
        /// <param name="serviceType">サービスの型。</param>
        /// <returns>サービス。</returns>
        public static object GetRequiredService(this IServiceProvider provider, Type serviceType)
        {
            var service = provider.GetService(serviceType);
            if (service == null)
            {
                throw new InvalidOperationException(string.Format("Service '{0}' not found", serviceType));
            }
            return service;
        }

        /// <summary>
        /// サービスを取得します。
        /// </summary>
        /// <typeparam name="T">サービスの型。</typeparam>
        /// <param name="provider">IServiceProvider。</param>
        /// <returns>サービス。</returns>
        public static T GetService<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// サービスを取得します。
        /// サービスが存在しない場合には ServiceNotFoundException を発生させます。
        /// </summary>
        /// <typeparam name="T">サービスの型。</typeparam>
        /// <param name="provider">IServiceProvider。</param>
        /// <returns>サービス。</returns>
        public static T GetRequiredService<T>(this IServiceProvider provider) where T : class
        {
            return GetRequiredService(provider, typeof(T)) as T;
        }
    }
}
