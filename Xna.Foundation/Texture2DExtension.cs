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

namespace Willcraftia.Xna.Foundation
{
    public static class Texture2DExtension
    {
        #region Extensions

        public static SamplerState ResolveSamplerState(this Texture2D texture)
        {
            if (texture.Format == SurfaceFormat.Single ||
                texture.Format == SurfaceFormat.HalfSingle ||
                texture.Format == SurfaceFormat.Vector2 ||
                texture.Format == SurfaceFormat.Vector4)
            {
                return SamplerState.PointClamp;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
