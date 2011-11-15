#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class ColorOverlap : RenderComponent
    {
        #region Inner classes

        #endregion

        Texture2D whiteTexture;

        public ColorOverlapSettings Settings;

        public ColorOverlap(IRenderContext renderContext)
            : base(renderContext)
        {
            Settings = ColorOverlapSettings.Default;
        }

        public void Filter(Texture2D source, BackBuffer destination)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("ColorOverlap disabled.");
            }

            var color = new Color(Settings.Color) * Settings.Alpha;

            destination.Begin();
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(source, destination.Bounds, Color.White);
                SpriteBatch.Draw(whiteTexture, destination.Bounds, color);
                SpriteBatch.End();
            }
            destination.End();
        }

        protected override void LoadContent()
        {
            whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            whiteTexture.SetData<Color>(new Color[] { Color.White });

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            whiteTexture.Dispose();

            base.UnloadContent();
        }
    }
}
