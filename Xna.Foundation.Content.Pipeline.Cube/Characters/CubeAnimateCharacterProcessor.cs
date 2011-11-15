#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Characters
{
    [ContentProcessor(DisplayName = "CubeAnimateCharacter Processor")]
    public sealed class CubeAnimateCharacterProcessor : ContentProcessor<CubeAnimateCharacter, CubeAnimateCharacterContent>
    {
        #region Inner classes

        internal struct Face
        {
            internal Color Color;
            internal Vector3 Normal;

            internal Face(Color color, Vector3 normal)
            {
                Color = color;
                Normal = normal;
            }
        }

        #endregion

        #region Fields and Properties

        public static readonly Matrix FrontTransform = Matrix.Identity;
        public static readonly Matrix BackTransform = Matrix.Identity;
        public static readonly Matrix LeftTransform = Matrix.CreateFromYawPitchRoll(-MathHelper.PiOver2, 0, 0);
        public static readonly Matrix RightTransform = Matrix.CreateFromYawPitchRoll(MathHelper.PiOver2, 0, 0);

        Texture2DContent sprite;
        PixelBitmapContent<Color> spriteBitmap;
        List<Face> faces = new List<Face>();

        #endregion

        public override CubeAnimateCharacterContent Process(CubeAnimateCharacter input, ContentProcessorContext context)
        {
            input.Validate();

            sprite = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(input.Sprite, null);
            spriteBitmap = (PixelBitmapContent<Color>) sprite.Mipmaps[0];

            var result = new CubeAnimateCharacterContent();

            foreach (var point in input.FrontSprites)
            {
                result.FrontModels.Add(CreateModel(input, context, point, FrontTransform));
            }
            foreach (var point in input.BackSprites)
            {
                result.BackModels.Add(CreateModel(input, context, point, BackTransform));
            }
            foreach (var point in input.LeftSprites)
            {
                result.LeftModels.Add(CreateModel(input, context, point, LeftTransform));
            }
            foreach (var point in input.RightSprites)
            {
                result.RightModels.Add(CreateModel(input, context, point, RightTransform));
            }

            foreach (var animationName in input.Animations.Keys)
            {
                result.Animations[animationName] = CreateAnimation(input, context, animationName);
            }

            result.ModelScale = input.ModelScale;
            result.CubeScale = input.CubeScale;
            result.SpriteWidth = input.SpriteWidth;
            result.SpriteHeight = input.SpriteHeight;

            return result;
        }

        CubeAnimateCharacterContent.AnimationContent CreateAnimation(
            CubeAnimateCharacter input,
            ContentProcessorContext context,
            string animationName)
        {
            var animationDescription = input.Animations[animationName];
            return new CubeAnimateCharacterContent.AnimationContent(
                animationDescription.FramesPerSecond,
                animationDescription.SpriteIndices);
        }

        ModelContent CreateModel(
            CubeAnimateCharacter input,
            ContentProcessorContext context,
            Point point,
            Matrix transform)
        {
            var meshBuilder = MeshBuilder.StartMesh("CubeCharacter");

            ResolvePositions(input, context, meshBuilder, point);
            ResolveMaterial(input, context, meshBuilder);
            ResolveVertices(input, context, meshBuilder, point);

            faces.Clear();

            var mesh = meshBuilder.FinishMesh();

            var parameters = new OpaqueDataDictionary();
            parameters["Scale"] = input.ModelScale;
            var model = context.Convert<MeshContent, ModelContent>(
                mesh,
                typeof(CubeAnimateCharacterModelProcessor).Name,
                parameters);

            model.Root.Transform *= transform;

            return model;
        }

        void ResolvePositions(
            CubeAnimateCharacter input,
            ContentProcessorContext context,
            MeshBuilder meshBuilder,
            Point point)
        {
            var width = input.SpriteWidth;
            var height = input.SpriteHeight;

            var cubeScale = input.CubeScale;

            var offsetX = -width / 2;
            var offsetY = -height / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var color = spriteBitmap.GetPixel(x + point.X, y + point.Y);

                    if (color.A == 0)
                    {
                        continue;
                    }

                    var vertices = new Vector3[8];

                    // move origin
                    float x0 = (x + offsetX) * cubeScale;
                    float x1 = (x + offsetX + 1) * cubeScale;

                    // reverse the vertical and move origin
                    float y0 = (height - y + offsetY - 1) * cubeScale;
                    float y1 = (height - y + offsetY) * cubeScale;

                    // front face
                    meshBuilder.CreatePosition(x0, y0, cubeScale);
                    meshBuilder.CreatePosition(x0, y1, cubeScale);
                    meshBuilder.CreatePosition(x1, y1, cubeScale);
                    meshBuilder.CreatePosition(x1, y0, cubeScale);

                    {
                        var face = new Face(color, Vector3.UnitZ);
                        faces.Add(face);
                    }

                    // back face
                    meshBuilder.CreatePosition(x1, y0, 0);
                    meshBuilder.CreatePosition(x1, y1, 0);
                    meshBuilder.CreatePosition(x0, y1, 0);
                    meshBuilder.CreatePosition(x0, y0, 0);

                    {
                        var face = new Face(color, -Vector3.UnitZ);
                        faces.Add(face);
                    }

                    // left face
                    bool leftFaceNeeded = false;
                    if (x == 0)
                    {
                        leftFaceNeeded = true;
                    }
                    else
                    {
                        var leftColor = spriteBitmap.GetPixel(x + point.X - 1, y + point.Y);
                        if (leftColor.A == 0)
                        {
                            leftFaceNeeded = true;
                        }
                    }

                    if (leftFaceNeeded)
                    {
                        meshBuilder.CreatePosition(x0, y0, 0);
                        meshBuilder.CreatePosition(x0, y1, 0);
                        meshBuilder.CreatePosition(x0, y1, cubeScale);
                        meshBuilder.CreatePosition(x0, y0, cubeScale);

                        {
                            var face = new Face(color, -Vector3.UnitX);
                            faces.Add(face);
                        }
                    }

                    // right face
                    bool rightFaceNeeded = false;
                    if (x == width - 1)
                    {
                        rightFaceNeeded = true;
                    }
                    else
                    {
                        var rightColor = spriteBitmap.GetPixel(x + point.X + 1, y + point.Y);
                        if (rightColor.A == 0)
                        {
                            rightFaceNeeded = true;
                        }
                    }

                    if (rightFaceNeeded)
                    {
                        meshBuilder.CreatePosition(x1, y0, cubeScale);
                        meshBuilder.CreatePosition(x1, y1, cubeScale);
                        meshBuilder.CreatePosition(x1, y1, 0);
                        meshBuilder.CreatePosition(x1, y0, 0);

                        {
                            var face = new Face(color, Vector3.UnitX);
                            faces.Add(face);
                        }
                    }

                    // top face
                    bool topFaceNeeded = false;
                    if (y == 0)
                    {
                        topFaceNeeded = true;
                    }
                    else
                    {
                        var topColor = spriteBitmap.GetPixel(x + point.X, y + point.Y - 1);
                        if (topColor.A == 0)
                        {
                            topFaceNeeded = true;
                        }
                    }

                    if (topFaceNeeded)
                    {
                        meshBuilder.CreatePosition(x0, y1, cubeScale);
                        meshBuilder.CreatePosition(x0, y1, 0);
                        meshBuilder.CreatePosition(x1, y1, 0);
                        meshBuilder.CreatePosition(x1, y1, cubeScale);

                        {
                            var face = new Face(color, Vector3.UnitY);
                            faces.Add(face);
                        }
                    }

                    // bottom face
                    bool bottomFaceNeeded = false;
                    if (y == height - 1)
                    {
                        bottomFaceNeeded = true;
                    }
                    else
                    {
                        var bottomColor = spriteBitmap.GetPixel(x + point.X, y + point.Y + 1);
                        if (bottomColor.A == 0)
                        {
                            bottomFaceNeeded = true;
                        }
                    }

                    if (bottomFaceNeeded)
                    {
                        meshBuilder.CreatePosition(x0, y0, 0);
                        meshBuilder.CreatePosition(x0, y0, cubeScale);
                        meshBuilder.CreatePosition(x1, y0, cubeScale);
                        meshBuilder.CreatePosition(x1, y0, 0);

                        {
                            var face = new Face(color, -Vector3.UnitY);
                            faces.Add(face);
                        }
                    }
                }
            }
        }

        void ResolveMaterial(
            CubeAnimateCharacter input,
            ContentProcessorContext context,
            MeshBuilder meshBuilder)
        {
            if (input.Material == null)
            {
                var material = new BasicMaterialContent();
                meshBuilder.SetMaterial(material);
            }
            else
            {
                var builder = new CubeMaterialBuilder();
                var material = builder.Build(input.Material, context);
                meshBuilder.SetMaterial(material);
            }
        }

        void ResolveVertices(
            CubeAnimateCharacter input,
            ContentProcessorContext context,
            MeshBuilder meshBuilder,
            Point point)
        {
            var colorChannel = meshBuilder.CreateVertexChannel<Color>(
                VertexChannelNames.Color(0));
            var normalChannel = meshBuilder.CreateVertexChannel<Vector3>(
                VertexChannelNames.Normal());

            var width = input.SpriteWidth;
            var height = input.SpriteHeight;

            for (int i = 0; i < faces.Count; i++)
            {
                meshBuilder.SetVertexChannelData(colorChannel, faces[i].Color);
                meshBuilder.SetVertexChannelData(normalChannel, faces[i].Normal);

                var index = i * 4;
                
                meshBuilder.AddTriangleVertex(index);
                meshBuilder.AddTriangleVertex(index + 1);
                meshBuilder.AddTriangleVertex(index + 2);

                meshBuilder.AddTriangleVertex(index);
                meshBuilder.AddTriangleVertex(index + 2);
                meshBuilder.AddTriangleVertex(index + 3);
            }
        }
    }
}