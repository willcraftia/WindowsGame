#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class NormalDepthMapEffect : Effect, IEffectMatrices
    {
        #region IEffectMatrices

        EffectParameter projection;
        public Matrix Projection
        {
            get { return projection.GetValueMatrix(); }
            set { projection.SetValue(value); }
        }

        EffectParameter view;
        public Matrix View
        {
            get { return view.GetValueMatrix(); }
            set { view.SetValue(value); }
        }

        EffectParameter world;
        public Matrix World
        {
            get { return world.GetValueMatrix(); }
            set { world.SetValue(value); }
        }

        #endregion

        public NormalDepthMapEffect(Effect cloneSource)
            : base(cloneSource)
        {
            world = Parameters["World"];
            view = Parameters["View"];
            projection = Parameters["Projection"];
        }
    }
}
