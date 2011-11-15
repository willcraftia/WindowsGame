using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Willcraftia.Xna.Foundation.Graphics
{
    public sealed class QuadVertexBuffer : IDisposable
    {
        GraphicsDevice graphicsDevice;
        VertexBuffer vertexBuffer;

        public QuadVertexBuffer(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            VertexPositionTexture[] vertices =
            {
                new VertexPositionTexture(
                    new Vector3(-1.0f, -1.0f, 0.0f),
                    new Vector2(0, 1)),
                new VertexPositionTexture(
                    new Vector3(-1.0f, 1.0f, 0.0f),
                    new Vector2(0, 0)),
                new VertexPositionTexture(
                    new Vector3(1.0f, -1.0f, 0.0f),
                    new Vector2(1, 1)),
                new VertexPositionTexture(
                    new Vector3(1.0f, 1.0f, 0.0f),
                    new Vector2(1, 0)),
            };
            vertexBuffer = new VertexBuffer(
                graphicsDevice,
                typeof(VertexPositionTexture),
                vertices.Length,
                BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionTexture>(vertices);
        }

        public void Draw()
        {
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool disposed;

        ~QuadVertexBuffer()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    vertexBuffer.Dispose();
                }
                disposed = true;
            }
        }

        #endregion
    }
}
