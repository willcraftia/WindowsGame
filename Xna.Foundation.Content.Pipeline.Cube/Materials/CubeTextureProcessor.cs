
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials
{
    [ContentProcessor(DisplayName = "CubeTexture Processor")]
    public class CubeTextureProcessor : TextureProcessor
    {
        public float Scale { get; set; }

        public CubeTextureProcessor()
        {
            Scale = 1;
            GenerateMipmaps = true;
            TextureFormat = TextureProcessorOutputFormat.DxtCompressed;
        }

        public override TextureContent Process(TextureContent input, ContentProcessorContext context)
        {
            input.ConvertBitmapType(typeof(PixelBitmapContent<Color>));
            foreach (var mipmapChain in input.Faces)
            {
                for (int i = 0; i < mipmapChain.Count; i++)
                {
                    var bitmap = mipmapChain[i] as PixelBitmapContent<Color>;

                    int newWidth = (int) ((float) bitmap.Width * Scale);
                    int newHeight = (int) ((float) bitmap.Height * Scale);

                    var newBitmap = new PixelBitmapContent<Color>(newWidth, newHeight);

                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            for (int scaleY = 0; scaleY < Scale; scaleY++)
                            {
                                for (int scaleX = 0; scaleX < Scale; scaleX++)
                                {
                                    newBitmap.SetPixel(
                                        (int) (x * Scale) + scaleX,
                                        (int) (y * Scale) + scaleY,
                                        bitmap.GetPixel(x, y));
                                }
                            }
                        }
                    }

                    mipmapChain[i] = newBitmap;
                }
            }

            return base.Process(input, context);
        }
    }
}