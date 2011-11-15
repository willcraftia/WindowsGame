#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public class RenderComponent : IDisposable
    {
        IRenderContext renderContext;
        public IRenderContext RenderContext
        {
            get { return renderContext; }
        }

        public ContentManager Content
        {
            get { return renderContext.Content; }
        }

        bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;

                enabled = value;
                OnEnabledChanged();
            }
        }

        EffectManager effectManager;
        public EffectManager EffectManager
        {
            get { return effectManager; }
        }

        BackBufferManager backBufferManager;
        public BackBufferManager BackBufferManager
        {
            get { return backBufferManager; }
        }

        /// <summary>
        /// 現在の GraphicsDevice を取得するヘルパ プロパティです。
        /// </summary>
        protected GraphicsDevice GraphicsDevice
        {
            get { return renderContext.GraphicsDevice; }
        }

        /// <summary>
        /// GameContext の SpriteBatch を取得するヘルパ プロパティです。
        /// </summary>
        protected SpriteBatch SpriteBatch
        {
            get { return renderContext.SpriteBatch; }
        }

        /// <summary>
        /// RenderContext の Scene を取得するヘルパ プロパティです。
        /// </summary>
        protected IRenderableScene Scene
        {
            get { return renderContext.Scene; }
        }

        bool contentLoaded;
        public bool ContentLoaded
        {
            get { return contentLoaded; }
        }

        protected RenderComponent(IRenderContext renderContext)
        {
            if (renderContext == null) throw new ArgumentNullException("renderContext");

            this.renderContext = renderContext;
        }

        public virtual void Initialize()
        {
            effectManager = new EffectManager(GraphicsDevice, Content);
            backBufferManager = new BackBufferManager(GraphicsDevice);

            LoadContent();
        }

        protected virtual void LoadContent()
        {
            contentLoaded = true;
        }

        protected virtual void UnloadContent()
        {
            effectManager.Unload();
            backBufferManager.Unload();
        }

        protected virtual void OnEnabledChanged()
        {
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected bool disposed;

        ~RenderComponent()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    UnloadContent();
                    if (contentLoaded)
                    {
                        effectManager.Dispose();
                        backBufferManager.Dispose();
                    }
                }
                disposed = true;
            }
        }

        #endregion
    }
}
