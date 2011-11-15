#region Using

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains
{
    public sealed class CubeTerrain
    {
        public ExternalReference<TextureContent> Heightmap;
        public float UnitScale;
        public int AltitudeScale;
        public int BlockScale;
        public float TexCoordScale;
        public CubeTerrainLayer Layer0;
        public CubeTerrainLayer Layer1;
        public CubeTerrainLayer Layer2;
        public CubeTerrainLayer Layer3;
        public CubeTerrainAltitude Altitude0;
        public CubeTerrainAltitude Altitude1;
        public CubeTerrainAltitude Altitude2;
        public CubeTerrainAltitude Altitude3;
    }
}
