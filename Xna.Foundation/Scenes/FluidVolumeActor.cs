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
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class FluidVolumeActor : PhysicsVolumeActor
    {
        #region Fields and Properties

        Buoyancy buoyancy = new Buoyancy();

        public float Density
        {
            get { return buoyancy.Density; }
            set { buoyancy.Density = value; }
        }

        #endregion

        protected override void OnCharacterEntered(CharacterActor actor)
        {
            if (actor.RigidBody != null)
            {
                actor.RigidBody.ExternalForces.Add(buoyancy);
            }

            base.OnCharacterEntered(actor);
        }

        protected override void OnCharacterLeaving(CharacterActor actor)
        {
            if (actor.RigidBody != null)
            {
                actor.RigidBody.ExternalForces.Remove(buoyancy);
            }

            base.OnCharacterLeaving(actor);
        }
    }
}
