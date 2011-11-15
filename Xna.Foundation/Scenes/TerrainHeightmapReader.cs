#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class TerrainHeightmapReader : ContentTypeReader<TerrainHeightmap>
    {
        protected override TerrainHeightmap Read(ContentReader input, TerrainHeightmap existingInstance)
        {
            float terrainScale = input.ReadSingle();
            int xLength = input.ReadInt32();
            int zLength = input.ReadInt32();
            float[,] terrainHeights = new float[xLength, zLength];
            for (int x = 0; x < xLength; x++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    terrainHeights[x, z] = input.ReadSingle();
                }
            }
            return new TerrainHeightmap(terrainHeights, terrainScale);
        }
    }
}
