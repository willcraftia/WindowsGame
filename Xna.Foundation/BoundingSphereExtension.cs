#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation
{
    public static class BoundingSphereExtension
    {
        #region Utility

        public static BoundingSphere Empty
        {
            get { return new BoundingSphere(Vector3.Zero, 0.0f); }
        }

        #endregion
    }
}
