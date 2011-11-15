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
    /// 剛体に作用する力を決定する際に呼び出されるインタフェースです。
    /// </summary>
    public interface IExternalForce
    {
        void ApplyExternalForce(IRigidBody rigidBody, float timeStep);
    }
}
