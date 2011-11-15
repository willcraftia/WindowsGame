#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class ShadowMapEffect : Effect, IEffectMatrices
    {
        #region IEffectMatrices

        public Matrix Projection
        {
            get { return Matrix.Identity; }
            set { }
        }

        public Matrix View
        {
            get { return Matrix.Identity; }
            set { }
        }

        EffectParameter world;
        public Matrix World
        {
            get { return world.GetValueMatrix(); }
            set { world.SetValue(value); }
        }

        #endregion

        EffectParameter lightViewProjection;
        public Matrix LightViewProjection
        {
            get { return lightViewProjection.GetValueMatrix(); }
            set { lightViewProjection.SetValue(value); }
        }

        public ShadowMapEffect(Effect cloneSource)
            : base(cloneSource)
        {
            world = Parameters["World"];
            lightViewProjection = Parameters["LightViewProjection"];
        }
    }
}
