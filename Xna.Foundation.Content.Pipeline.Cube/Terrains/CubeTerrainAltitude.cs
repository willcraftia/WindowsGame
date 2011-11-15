#region Using

using Microsoft.Xna.Framework.Content.Pipeline;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains
{
    public sealed class CubeTerrainAltitude
    {
        public int MinAltitude;
        public int MaxAltitude;
        public ExternalReference<CubeMaterial> TopMaterial;
        public ExternalReference<CubeMaterial> SideMaterial;

        public bool ContainsAltitude(int altitude)
        {
            return MinAltitude <= altitude && altitude <= MaxAltitude;
        }

        public bool ContainsAltitudeRange(
            int bottomAltitude,
            int topAltitude,
            out int resultBottomAltitude,
            out int resultTopAltitude)
        {
            if (MinAltitude <= bottomAltitude && bottomAltitude <= MaxAltitude)
            {
                resultBottomAltitude = bottomAltitude;
                if (topAltitude <= MaxAltitude)
                {
                    resultTopAltitude = topAltitude;
                }
                else
                {
                    resultTopAltitude = MaxAltitude;
                }
                return true;
            }
            else if (MinAltitude <= topAltitude && topAltitude <= MaxAltitude)
            {
                resultTopAltitude = topAltitude;
                if (MinAltitude <= bottomAltitude)
                {
                    resultBottomAltitude = bottomAltitude;
                }
                else
                {
                    resultBottomAltitude = MinAltitude;
                }
                return true;
            }
            else if (bottomAltitude < MinAltitude && MaxAltitude < topAltitude)
            {
                resultBottomAltitude = MinAltitude;
                resultTopAltitude = MaxAltitude;
                return true;
            }
            else
            {
                resultBottomAltitude = MinAltitude;
                resultTopAltitude = MaxAltitude;
                return false;
            }
        }
    }
}
