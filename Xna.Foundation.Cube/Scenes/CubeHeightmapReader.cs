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

namespace Willcraftia.Xna.Foundation.Cube.Scenes
{
    public sealed class CubeHeightmapReader : ContentTypeReader<CubeHeightmap>
    {
        protected override CubeHeightmap Read(ContentReader input, CubeHeightmap existingInstance)
        {
            var terrainScale = input.ReadSingle();
            var xLength = input.ReadInt32();
            var zLength = input.ReadInt32();
            var heights = new float[xLength, zLength];
            for (int x = 0; x < xLength; x++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    heights[x, z] = input.ReadSingle();
                }
            }
            return new CubeHeightmap(heights, terrainScale);
        }
    }
}
