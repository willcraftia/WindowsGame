#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Graphics
{
    public sealed class BoundingSphereDrawer
    {
        #region Inner classes

        #endregion

        #region Fields and Properties

        GraphicsDevice graphicsDevice;
        VertexBuffer vertexBuffer;
        int sphereResolution;

        #endregion

        #region Constructors

        public BoundingSphereDrawer(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, 30)
        {
        }

        public BoundingSphereDrawer(GraphicsDevice graphicsDevice, int sphereResolution)
        {
            this.graphicsDevice = graphicsDevice;
            this.sphereResolution = sphereResolution;

            var vertices = new VertexPositionColor[(sphereResolution + 1) * 3];

            int index = 0;

            float step = MathHelper.TwoPi / (float) sphereResolution;

            //create the loop on the XY plane first
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                vertices[index++] = new VertexPositionColor(
                    new Vector3((float) Math.Cos(a), (float) Math.Sin(a), 0f),
                    Color.White);
            }

            //next on the XZ plane
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                vertices[index++] = new VertexPositionColor(
                    new Vector3((float) Math.Cos(a), 0f, (float) Math.Sin(a)),
                    Color.White);
            }

            //finally on the YZ plane
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                vertices[index++] = new VertexPositionColor(
                    new Vector3(0f, (float) Math.Cos(a), (float) Math.Sin(a)),
                    Color.White);
            }

            vertexBuffer = new VertexBuffer(
                graphicsDevice,
                typeof(VertexPositionColor),
                vertices.Length,
                BufferUsage.None);
            vertexBuffer.SetData(vertices);
        }

        #endregion

        public void Draw(ref BoundingSphere sphere, BasicEffect effect, ref Color vertexColor)
        {
            graphicsDevice.SetVertexBuffer(vertexBuffer);

            Matrix scale;
            Matrix.CreateScale(sphere.Radius, out scale);
            Matrix translation;
            Matrix.CreateTranslation(ref sphere.Center, out translation);
            Matrix transform;
            Matrix.Multiply(ref scale, ref translation, out transform);

            effect.World = transform;
            effect.DiffuseColor = vertexColor.ToVector3();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //render each circle individually
                graphicsDevice.DrawPrimitives(
                        PrimitiveType.LineStrip,
                        0,
                        sphereResolution);
                graphicsDevice.DrawPrimitives(
                        PrimitiveType.LineStrip,
                        sphereResolution + 1,
                        sphereResolution);
                graphicsDevice.DrawPrimitives(
                        PrimitiveType.LineStrip,
                        (sphereResolution + 1) * 2,
                        sphereResolution);
            }
        }
    }
}
