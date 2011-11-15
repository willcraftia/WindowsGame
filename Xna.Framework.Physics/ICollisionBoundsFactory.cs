#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

#endregion

namespace Willcraftia.Xna.Framework.Physics
{
    /// <summary>
    /// ICollisionBounds の生成を担うファクトリのインタフェースです。
    /// </summary>
    public interface ICollisionBoundsFactory
    {
        /// <summary>
        /// ICollisionBounds を生成します。
        /// </summary>
        /// <returns>生成された ICollisionBounds。</returns>
        ICollisionBounds CreateCollisionBounds();
    }
}
