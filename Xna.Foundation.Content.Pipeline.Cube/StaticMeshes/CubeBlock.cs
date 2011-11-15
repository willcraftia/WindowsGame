#region Using

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes
{
    public sealed class CubeBlock : CubeStaticMesh
    {
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }

        [ContentSerializer(Optional = true)]
        public ExternalReference<CubeMaterial> BaseMaterial { get; set; }

        [ContentSerializer(Optional = true)]
        public ExternalReference<CubeMaterial> TopMaterial { get; set; }

        [ContentSerializer(Optional = true)]
        public ExternalReference<CubeMaterial> BottomMaterial { get; set; }

        [ContentSerializer(Optional = true)]
        public ExternalReference<CubeMaterial> NorthMaterial { get; set; }

        [ContentSerializer(Optional = true)]
        public ExternalReference<CubeMaterial> SouthMaterial { get; set; }

        [ContentSerializer(Optional = true)]
        public ExternalReference<CubeMaterial> WestMaterial { get; set; }

        [ContentSerializer(Optional = true)]
        public ExternalReference<CubeMaterial> EastMaterial { get; set; }

        public CubeBlock()
        {
            SizeX = 1;
            SizeY = 1;
            SizeZ = 1;
        }
    }
}
