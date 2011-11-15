#region Using

using System.ComponentModel;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains
{
    [ContentProcessor]
    [DesignTimeVisible(false)]
    public class CubeTerrainModelProcessor : ModelProcessor
    {
        protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {
            return material;
        }
    }
}