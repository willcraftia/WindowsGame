#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Xna.Framework.Content.Pipeline
{
    /// <summary>
    /// ExternalReference の拡張です。
    /// </summary>
    public static class ExternalReferenceExtension
    {
        /// <summary>
        /// クローンを生成します。
        /// </summary>
        /// <typeparam name="T">外部参照型。</typeparam>
        /// <param name="reference">実体。</param>
        /// <returns>生成されたクローン。</returns>
        public static ExternalReference<T> CreateClone<T>(this ExternalReference<T> reference)
        {
            if (string.IsNullOrEmpty(reference.Filename))
            {
                return new ExternalReference<T>();
            }
            else
            {
                return new ExternalReference<T>(reference.Filename);
            }
        }
    }
}
