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
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class Monochrome : RenderComponent
    {
        #region Inner classes

        public sealed class MonochromeEffect : Effect
        {
            #region Fields and Properties

            public EffectParameter SceneMapParameter;
            public EffectParameter CbParameter;
            public EffectParameter CrParameter;

            #endregion

            #region Constructors

            public MonochromeEffect(Effect cloneSource)
                : base(cloneSource)
            {
                CbParameter = Parameters["Cb"];
                CrParameter = Parameters["Cr"];
                SceneMapParameter = Parameters["SceneMap"];
            }

            #endregion
        }

        #endregion

        MonochromeEffect monochromeEffect;

        public MonochromeSettings Settings;

        public Monochrome(IRenderContext renderContext)
            : base(renderContext)
        {
            Settings = MonochromeSettings.Default;
        }

        public void Filter(Texture2D source, BackBuffer destination)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("Monochrome is disabled");
            }

            monochromeEffect.CbParameter.SetValue(Settings.CbCr.X);
            monochromeEffect.CrParameter.SetValue(Settings.CbCr.Y);

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.Opaque;

            destination.Begin();
            {
                SpriteBatch.Render(monochromeEffect, source, destination.Bounds);
            }
            destination.End();
        }

        protected override void LoadContent()
        {
            monochromeEffect = EffectManager.Load<MonochromeEffect>();

            base.LoadContent();
        }
    }
}
