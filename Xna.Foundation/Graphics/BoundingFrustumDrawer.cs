using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Willcraftia.Xna.Foundation.Graphics
{
    public sealed class BoundingFrustumDrawer
    {
        static readonly int[] indices;
        static readonly int primitiveCount;

        static BoundingFrustumDrawer()
        {
            indices = new int[]
            {
                0, 1,
                1, 2,
                2, 3,
                3, 0,
                0, 4,
                1, 5,
                2, 6,
                3, 7,
                4, 5,
                5, 6,
                6, 7,
                7, 4,
            };
            primitiveCount = indices.Length / 2;
        }

        GraphicsDevice graphicsDevice;

        public BoundingFrustumDrawer(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Draw(
            ref BoundingFrustum boundingFrustum,
            Effect effect,
            ref Color vertexColor)
        {
            var coners = boundingFrustum.GetCorners();
            var vertices = new VertexPositionColor[8];
            for (int i = 0; i < 8; i++)
            {
                vertices[i].Position = coners[i];
                vertices[i].Color = vertexColor;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    vertices,
                    0,
                    8,
                    indices,
                    0,
                    primitiveCount);
            }
        }
    }
}
