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

namespace Willcraftia.Xna.Framework.Physics
{
    /// <summary>
    /// ICollisionShape の生成を担うファクトリのインタフェースです。
    /// </summary>
    public interface ICollisionShapeFactory
    {
        /// <summary>
        /// 指定の ICollisionShape を生成します。
        /// </summary>
        /// <param name="type">ICollisionShape 型。</param>
        /// <returns>生成された ICollisionShape。</returns>
        ICollisionShape CreateCollisionShape(Type type);
    }
}
