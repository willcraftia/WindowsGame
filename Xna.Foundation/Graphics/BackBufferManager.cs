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
    public sealed class BackBufferManager : IDisposable
    {
        #region Fields and Properties

        GraphicsDevice graphicsDevice;
        List<BackBuffer> backBuffers;

        #endregion

        #region Constructors

        public BackBufferManager(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }

            this.graphicsDevice = graphicsDevice;

            backBuffers = new List<BackBuffer>();
        }

        #endregion

        public BackBuffer Load(string name)
        {
            return Load(name, null);
        }

        public BackBuffer Load(string name, BackBuffer cloneSource)
        {
            var backBuffer = new BackBuffer(graphicsDevice, name);

            if (cloneSource != null)
            {
                backBuffer.Width = cloneSource.Width;
                backBuffer.Height = cloneSource.Height;
                backBuffer.MipMap = cloneSource.MipMap;
                backBuffer.SurfaceFormat = cloneSource.SurfaceFormat;
                backBuffer.DepthFormat = cloneSource.DepthFormat;
                backBuffer.MultiSampleCount = backBuffer.MultiSampleCount;
                backBuffer.RenderTargetUsage = backBuffer.RenderTargetUsage;
                backBuffer.RenderTargetCount = backBuffer.RenderTargetCount;
            }

            backBuffers.Add(backBuffer);
            return backBuffer;
        }

        public void Unload()
        {
            foreach (var backBuffer in backBuffers)
            {
                backBuffer.Dispose();
            }
            backBuffers.Clear();
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool disposed;

        ~BackBufferManager()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Unload();
                }
                disposed = true;
            }
        }

        #endregion
    }
}
