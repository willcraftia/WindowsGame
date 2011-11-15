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
    /// 物理演算サービスのインタフェースです。
    /// </summary>
    public interface IPhysicsService
    {
        /// <summary>
        /// IRigidBodyFactory。
        /// </summary>
        IRigidBodyFactory RigidBodyFactory { get; }

        /// <summary>
        /// ICollisionBoundsFactory。
        /// </summary>
        ICollisionBoundsFactory CollisionBoundsFactory { get; }

        /// <summary>
        /// ICollisionShapeFactory。
        /// </summary>
        ICollisionShapeFactory CollisionShapeFactory { get; }

        // MEMO:
        // シーン毎に ICollisionTester を切り替えることも想定して set を公開します。
        /// <summary>
        /// ICollisionTester。
        /// </summary>
        ICollisionTester CollisionTester { get; set; }

        /// <summary>
        /// 物理システムが有効かどうかを示す値。
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 物理システムで使用する重力加速度。
        /// </summary>
        /// <remarks>
        /// 物理システムに参加する剛体は、他のクラスから明示的に重力加速度が設定されない限り、
        /// このプロパティが示す重力加速度に従います。
        /// </remarks>
        Vector3 Gravity { get; set; }
    }
}
