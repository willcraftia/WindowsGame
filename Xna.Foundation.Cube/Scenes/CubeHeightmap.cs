#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Cube.Scenes
{
    public sealed class CubeHeightmap
    {
        #region Fields and Properties

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

        #endregion

        #region Constructors

        public CubeHeightmap(float[,] heights, float scale)
        {
            if (heights == null)
            {
                throw new ArgumentNullException("heights");
            }
            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException("scale");
            }

            this.heights = heights;
            Scale = scale;
        }

        #endregion

        public float GetHeight(int x, int z)
        {
            return heights[x, z];
        }
    }
}
