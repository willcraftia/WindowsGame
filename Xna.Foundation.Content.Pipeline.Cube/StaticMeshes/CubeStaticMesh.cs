#region Using

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes
{
    public class CubeStaticMesh
    {
        public const float DefaultUnitScale = 0.1f;
        public const int DefaultBlockScale = 16;

        public float UnitScale { get; set; }
        public int BlockScale { get; set;}

        protected CubeStaticMesh()
        {
            UnitScale = DefaultUnitScale;
            BlockScale = DefaultBlockScale;
        }
    }
}
