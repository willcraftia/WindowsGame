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
    /// 箱型衝突形状のインタフェースです。
    /// </summary>
    public interface IBoxCollisionShape : ICollisionShape
    {
        /// <summary>
        /// TODO: どの頂点の座標だっけ？
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// TODO: 要素の順序はなんだっけ？
        /// </summary>
        Vector3 SideLengths { get; set; }
    }
}
