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

namespace Willcraftia.Xna.Framework.Physics
{
    /// <summary>
    /// 衝突形状のインタフェースです。
    /// </summary>
    public interface ICollisionShape
    {
        float StaticFriction { get; set; }

        float DynamicFriction { get; set; }
        
        float Restitution { get; set; }

        /// <summary>
        /// 体積。
        /// </summary>
        float Volume { get; }

        /// <summary>
        /// 線分が交差するかどうかを判定します。
        /// </summary>
        /// <param name="position">線分の座標ベクトル。</param>
        /// <param name="vector">線分の方向ベクトル。</param>
        /// <param name="intersectPosition">交点。</param>
        /// <param name="intersectNormal">交点での直線と衝突に対する法線ベクトル。</param>
        /// <param name="intersectFraction">交差している線分の比率。</param>
        /// <returns>true (交差する場合)、false (それ以外の場合)。</returns>
        bool Intersects(
            ref Vector3 position, ref Vector3 vector,
            out Vector3 intersectPosition, out Vector3 intersectNormal, out float intersectFraction);
    }
}
