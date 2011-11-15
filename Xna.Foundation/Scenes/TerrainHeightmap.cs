#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class TerrainHeightmap
    {
        float[,] heights;

        public int MapXLength
        {
            get { return heights.GetLength(0); }
        }

        public int MapZLength
        {
            get { return heights.GetLength(1); }
        }

        public float Scale { get; private set; }

        public TerrainHeightmap(float[,] heights, float scale)
        {
            if (heights == null)
            {
                throw new ArgumentNullException("heights");
            }

            this.heights = heights;
            Scale = scale;
        }

        public float GetHeight(int x, int z)
        {
            return heights[x, z];
        }
    }
}
