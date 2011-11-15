using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Willcraftia.Xna.Foundation.Content.Pipeline.TerrainMap
{
    [ContentProcessor(DisplayName = "Willcraftia Terrain Map Texture Processor")]
    public sealed class TerrainMapTextureProcessor : ContentProcessor<TerrainMapDescription, TextureContent>
    {
        public override TextureContent Process(TerrainMapDescription input, ContentProcessorContext context)
        {
            input.Validate();

            var heightmap = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(input.Heightmap, null);
            heightmap.ConvertBitmapType(typeof(PixelBitmapContent<float>));
            var heights = (PixelBitmapContent<float>) heightmap.Mipmaps[0];

            var alphas0 = GetAlphas(context, input.Layer0.Alphamap);

            var map = new PixelBitmapContent<Color>(heights.Width, heights.Height);
            for (int y = 0; y < heights.Height; y++)
            {
                for (int x = 0; x < heights.Width; x++)
                {
                    var altitude = (int) (heights.GetPixel(x, y) * (float) input.AltitudeScale);

                    Color color;

                    var fluidSurface = input.FindTerrainMapFluidSurfaceDescription(altitude);
                    if (fluidSurface != null)
                    {
                        //
                        // use fluid color
                        //
                        color = fluidSurface.Color;
                    }
                    else if (alphas0 != null && alphas0.GetPixel(x, y) != 0)
                    {
                        //
                        // use alphamap0 color
                        //
                        color = input.Layer0.Color;
                    }
                    else
                    {
                        //
                        // use Altitude color
                        //
                        var altitudeColorDescription = input.FindTerrainMapAltitudeColorDescription(altitude);
                        if (altitudeColorDescription == null)
                        {
                            color = Color.Black;
                        }
                        else
                        {
                            altitudeColorDescription.ResolveColor(altitude, out color);
                        }

                    }

                    map.SetPixel(x, y, color);
                }
            }

            var result = new Texture2DContent();
            result.Faces[0].Add(map);

            return result;
        }

        PixelBitmapContent<float> GetAlphas(ContentProcessorContext context, ExternalReference<TextureContent> alphamap)
        {
            var map = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(alphamap, null);
            map.ConvertBitmapType(typeof(PixelBitmapContent<float>));
            return (PixelBitmapContent<float>) map.Mipmaps[0];
        }
    }
}