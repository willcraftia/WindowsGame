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
    /// IRigidBody の生成を担うファクトリのインタフェースです。
    /// </summary>
    public interface IRigidBodyFactory
    {
        /// <summary>
        /// IRigidBody を生成します。
        /// </summary>
        /// <returns>生成された IRigidBody。</returns>
        IRigidBody CreateRigidBody();
    }
}
