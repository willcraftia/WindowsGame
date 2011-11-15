using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Willcraftia.Xna.Foundation.Content.Pipeline.TerrainMap
{
    public sealed class TerrainMapLayerDescription
    {
        public ExternalReference<TextureContent> Alphamap;
        public Color Color;

        public void Validate()
        {
            if (Alphamap == null)
            {
                throw new InvalidContentException("Alphamap must be not null.");
            }
        }
    }
}
