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
    /// 平面衝突形状のインタフェースです。
    /// </summary>
    public interface IPlaneCollisionShape : ICollisionShape
    {
        /// <summary>
        /// 面の法線ベクトル。
        /// </summary>
        Vector3 Normal { get; set; }
        
        /// <summary>
        /// 面の D 値。
        /// </summary>
        float D { get; set; }
    }
}
