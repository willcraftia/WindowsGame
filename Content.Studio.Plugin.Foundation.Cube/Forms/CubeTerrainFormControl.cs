#region Using

using Willcraftia.Content.Studio.Forms;
using Willcraftia.Xna.Foundation.Cube.Scenes;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Cube.Forms
{
    public sealed class CubeTerrainFormControl : ModelFormControl
    {
        CubeTerrain cubeTerrain;

        public override void LoadContent()
        {
            base.LoadContent();

            cubeTerrain = RuntimeContent.LoadContent(ContentManager) as CubeTerrain;
            if (cubeTerrain != null)
            {
                Model = cubeTerrain.Model;
            }
        }

        public override void UnloadContent()
        {
            cubeTerrain = null;

            base.UnloadContent();
        }
    }
}
