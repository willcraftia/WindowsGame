#region Using

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes
{
    public sealed class CubeTile : CubeStaticMesh
    {
        public int SizeX { get; set; }
        public int SizeZ { get; set; }

        [ContentSerializer(Optional = true)]
        public ExternalReference<CubeMaterial> Material { get; set; }

        public CubeTile()
        {
            SizeX = 1;
            SizeZ = 1;
        }
    }
}
