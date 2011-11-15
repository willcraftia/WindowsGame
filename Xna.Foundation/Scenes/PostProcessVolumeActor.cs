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

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class PostProcessVolumeActor : VolumeActor
    {
        #region Fields and Properties

        public PostProcessSettings Settings = PostProcessSettings.Default;

        bool active;
        public bool Active
        {
            get { return active; }
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            //
            // TODO: support any actors
            //
            var camera = ActorContext.ActiveCamera;

            if (Shape.Contains(ref camera.Position))
            {
                active = true;
            }
            else
            {
                active = false;
            }

            base.Update(gameTime);
        }
    }
}
