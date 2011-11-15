#region Using

using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains
{
    public sealed class CubeHeightmapContent
    {
        float[,] heights;
        public float[,] Heights
        {
            get { return heights; }
        }

        float scale;
        public float Scale
        {
            get { return scale; }
        }

        public CubeHeightmapContent(PixelBitmapContent<float> bitmap, float unitScale, int blockScale, int altitudeScale)
        {
            scale = unitScale * (float) blockScale;
            var xLength = bitmap.Width;
            var zLength = bitmap.Height;
            heights = new float[xLength, zLength];
            for (int z = 0; z < zLength; z++)
            {
                for (int x = 0; x < xLength; x++)
                {
                    //Heights[x, z] = altitudeScale * (spriteBitmap.GetPixel(x, z) - 1);

                    int y = (int) (bitmap.GetPixel(x, z) * (float) altitudeScale);
                    Heights[x, z] = y * Scale;
                }
            }
        }
    }
}
