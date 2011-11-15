#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Graphics
{
    /// <summary>
    /// RenderTarget を管理するクラスです。
    /// </summary>
    public sealed class BackBuffer : IDisposable
    {
        #region Fields and Properties

        GraphicsDevice graphicsDevice;

        public string Name { get; private set; }

        int width;
        public int Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    Deactivate();
                }
            }
        }

        int height;
        public int Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    Deactivate();
                }
            }
        }

        bool mipMap;
        public bool MipMap
        {
            get { return mipMap; }
            set
            {
                if (mipMap != value)
                {
                    mipMap = value;
                    Deactivate();
                }
            }
        }

        SurfaceFormat surfaceFormat;
        public SurfaceFormat SurfaceFormat
        {
            get { return surfaceFormat; }
            set
            {
                if (surfaceFormat != value)
                {
                    surfaceFormat = value;
                    Deactivate();
                }
            }
        }

        DepthFormat depthFormat;
        public DepthFormat DepthFormat
        {
            get { return depthFormat; }
            set
            {
                if (depthFormat != value)
                {
                    depthFormat = value;
                    Deactivate();
                }
            }
        }

        int multiSampleCount;
        public int MultiSampleCount
        {
            get { return multiSampleCount; }
            set
            {
                if (multiSampleCount != value)
                {
                    multiSampleCount = value;
                    Deactivate();
                }
            }
        }

        RenderTargetUsage renderTargetUsage;
        public RenderTargetUsage RenderTargetUsage
        {
            get { return renderTargetUsage; }
            set
            {
                if (renderTargetUsage != value)
                {
                    renderTargetUsage = value;
                    Deactivate();
                }
            }
        }

        int renderTargetCount;
        public int RenderTargetCount
        {
            get { return renderTargetCount; }
            set
            {
                value = value < 1 ? 1 : value;
                if (renderTargetCount != value)
                {
                    renderTargetCount = value;
                    Deactivate();
                }
            }
        }

        bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (!enabled)
                    {
                        Deactivate();
                    }
                }
            }
        }

        int currentIndex;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                if (value < 0 || renderTargetCount <= value)
                {
                    throw new ArgumentOutOfRangeException("CurrentIndex");
                }

                if (currentIndex != value)
                {
                    currentIndex = value;
                }
            }
        }

        RenderTarget2D[] renderTargets;
        bool activated;

        /// <summary>
        /// 矩形サイズを取得します。
        /// </summary>
        /// <remarks>
        /// このプロパティは new Rectangle(0, 0, Width, Height) を返します。
        /// </remarks>
        public Rectangle Bounds
        {
            get { return new Rectangle(0, 0, width, height); }
        }

        #endregion

        #region Constructors

        public BackBuffer(GraphicsDevice graphicsDevice, string name)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }

            this.graphicsDevice = graphicsDevice;
            
            Name = name;

            var pp = graphicsDevice.PresentationParameters;
            width = pp.BackBufferWidth;
            height = pp.BackBufferHeight;
            mipMap = true;
            surfaceFormat = pp.BackBufferFormat;
            depthFormat = pp.DepthStencilFormat;
            multiSampleCount = pp.MultiSampleCount;
            renderTargetUsage = pp.RenderTargetUsage;

            renderTargetCount = 1;
            currentIndex = 0;

            activated = false;
            disposed = false;
        }

        #endregion

        void AssertEnabled()
        {
            if (!enabled)
            {
                throw new InvalidOperationException(
                    string.Format("Back buffer '{0}' disabled", Name));
            }
        }

        public RenderTarget2D GetRenderTarget(int index)
        {
            AssertEnabled();
            Activate();
            return renderTargets[index];
        }

        public RenderTarget2D GetRenderTarget()
        {
            AssertEnabled();
            return GetRenderTarget(currentIndex);
        }

        void Activate()
        {
            if (!activated)
            {
                CreateRenderTargets();
                activated = true;
            }
        }

        void CreateRenderTargets()
        {
            renderTargets = new RenderTarget2D[renderTargetCount];

            for (int i = 0; i < renderTargetCount; i++)
            {
                renderTargets[i] = new RenderTarget2D(
                    graphicsDevice,
                    width,
                    height,
                    mipMap,
                    surfaceFormat,
                    depthFormat,
                    multiSampleCount,
                    renderTargetUsage);

                if (!string.IsNullOrEmpty(Name))
                {
                    if (1 < renderTargetCount)
                    {
                        renderTargets[i].Name = Name + "." + i;
                    }
                    else
                    {
                        renderTargets[i].Name = Name;
                    }
                }
            }
        }

        void Deactivate()
        {
            if (activated)
            {
                DisposeRenderTargets();
                activated = false;
            }
        }

        void DisposeRenderTargets()
        {
            if (renderTargets != null)
            {
                for (int i = 0; i < renderTargets.Length; i++)
                {
                    if (renderTargets[i] != null)
                    {
                        renderTargets[i].Dispose();
                    }
                }
            }
        }

        public void Begin()
        {
            AssertEnabled();
            Begin(currentIndex);
        }

        bool begun;

        void Begin(int index)
        {
            AssertEnabled();
            if (begun)
            {
                throw new InvalidOperationException(
                    "Begin() must not be invoked after the other Begin()");
            }

            Activate();

            graphicsDevice.SetRenderTarget(renderTargets[index]);

            begun = true;
        }

        public void End()
        {
            AssertEnabled();
            if (!begun)
            {
                throw new InvalidOperationException(
                    "End() must be invoked after Begin()");
            }

            graphicsDevice.SetRenderTarget(null);

            begun = false;
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        bool disposed;

        ~BackBuffer()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Deactivate();
                }
                disposed = true;
            }
        }

        #endregion
    }
}
