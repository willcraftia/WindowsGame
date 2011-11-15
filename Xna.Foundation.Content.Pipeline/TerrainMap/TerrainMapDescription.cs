#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.TerrainMap
{
    public sealed class TerrainMapDescription
    {
        public ExternalReference<TextureContent> Heightmap;
        public int AltitudeScale;
        //public int Scale;
        public TerrainMapLayerDescription Layer0;
        public TerrainMapAltitudeColorDescription Red;
        public TerrainMapAltitudeColorDescription Green;
        public TerrainMapAltitudeColorDescription Blue;
        public TerrainMapAltitudeColorDescription Alpha;
        public TerrainMapFluidSurfaceDescription[] FluidSurfaces;

        public void Validate()
        {
            if (Heightmap == null)
            {
                throw new InvalidContentException("Heightmap must be not null.");
            }
            if (AltitudeScale <= 0)
            {
                throw new InvalidContentException("Heightmap's altitude scale must be a nonnegative value.");
            }
            if (Layer0 != null)
            {
                Layer0.Validate();
            }
            if (Red != null)
            {
                Red.Validate();
            }
            if (Green != null)
            {
                Green.Validate();
            }
            if (Blue != null)
            {
                Blue.Validate();
            }
            if (Alpha != null)
            {
                Alpha.Validate();
            }
            if (FluidSurfaces != null)
            {
                foreach (TerrainMapFluidSurfaceDescription fluidSurface in FluidSurfaces)
                {
                    fluidSurface.Validate();
                    if (AltitudeScale < fluidSurface.Altitude)
                    {
                        throw new InvalidContentException("Fluid surface's altitude must be less than or equal to heightmap's altitude scale.");
                    }
                }
            }
        }

        public TerrainMapFluidSurfaceDescription FindTerrainMapFluidSurfaceDescription(int altitude)
        {
            if (FluidSurfaces == null || FluidSurfaces.Length == 0)
            {
                return null;
            }

            foreach (TerrainMapFluidSurfaceDescription fluidSurface in FluidSurfaces)
            {
                if (fluidSurface.IsUnderFluidSurface(altitude))
                {
                    return fluidSurface;
                }
            }
            return null;
        }

        public TerrainMapAltitudeColorDescription FindTerrainMapAltitudeColorDescription(int altitude)
        {
            if (Red != null && Red.Contains(altitude))
            {
                return Red;
            }
            if (Green != null && Green.Contains(altitude))
            {
                return Green;
            }
            if (Blue != null && Blue.Contains(altitude))
            {
                return Blue;
            }
            if (Alpha != null && Alpha.Contains(altitude))
            {
                return Alpha;
            }
            return null;
        }
    }
}
