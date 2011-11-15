using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Willcraftia.Xna.Foundation.Content.Pipeline.TerrainMap
{
    public sealed class TerrainMapFluidSurfaceDescription
    {
        public Point MinPoint;
        public Point MaxPoint;
        public int Altitude;
        public Color Color;

        public void Validate()
        {
            if (MinPoint.X < 0 || MinPoint.Y < 0)
            {
                throw new InvalidContentException("Fluid surface's min point must be nonnegative X and Y values.");
            }
            if (MaxPoint.X < 0 || MaxPoint.Y < 0 || MaxPoint.X <= MinPoint.X || MaxPoint.Y <= MinPoint.Y)
            {
                throw new InvalidContentException("Fluid surface's max point must be nonnegative X and Y values, and greater than min ones.");
            }
            if (Altitude < 0)
            {
                throw new InvalidContentException("Fluid surface's altitude must be a nonnegative value.");
            }
        }

        public bool IsUnderFluidSurface(int altitude)
        {
            return altitude <= Altitude;
        }
    }
}
