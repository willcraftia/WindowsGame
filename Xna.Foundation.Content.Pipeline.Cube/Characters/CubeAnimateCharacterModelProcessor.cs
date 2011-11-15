#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Characters
{
    [ContentProcessor]
    [DesignTimeVisible(false)]
    public sealed class CubeAnimateCharacterModelProcessor : ModelProcessor
    {
        protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {
            return material;
        }
    }
}