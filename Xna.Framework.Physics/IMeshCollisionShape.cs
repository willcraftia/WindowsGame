#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Framework.Physics
{
    /// <summary>
    /// Model が表す Mesh にフィットする衝突形状のインタフェースです。。
    /// </summary>
    public interface IMeshCollisionShape : ICollisionShape
    {
        /// <summary>
        /// Model が表す Mesh にフィットする衝突形状を生成します。
        /// </summary>
        /// <param name="model">Model。</param>
        void CreateMesh(Model model);
    }
}
