#region Using

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains
{
    /// <summary>
    /// CubeHeightmapContent の ContentTypeWriter です。
    /// </summary>
    [ContentTypeWriter]
    public class CubeHeightmapWriter : ContentTypeWriter<CubeHeightmapContent>
    {
        protected override void Write(ContentWriter output, CubeHeightmapContent value)
        {
            output.Write(value.Scale);
            output.Write(value.Heights.GetLength(0));
            output.Write(value.Heights.GetLength(1));
            foreach (float height in value.Heights)
            {
                output.Write(height);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Willcraftia.Xna.Foundation.Cube.Scenes.CubeHeightmapReader, Willcraftia.Xna.Foundation.Cube, Version=1.0.0.0, Culture=neutral";
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Willcraftia.Xna.Foundation.Cube.Scenes.CubeHeightmap, Willcraftia.Xna.Foundation.Cube, Version=1.0.0.0, Culture=neutral";
        }
    }
}
