#region Using

using Microsoft.Xna.Framework;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class GravityVolumeActor : PhysicsVolumeActor
    {
        IPhysicsService physicsService;

        public Vector3 Gravity;

        public override void LoadContent()
        {
            physicsService = ActorContext.GetRequiredService<IPhysicsService>();

            base.LoadContent();
        }

        protected override void OnCharacterEntered(CharacterActor actor)
        {
            if (actor.RigidBody != null)
            {
                actor.RigidBody.UpdateGravity(ref Gravity);
            }

            base.OnCharacterEntered(actor);
        }

        protected override void OnCharacterLeaving(CharacterActor actor)
        {
            if (actor.RigidBody != null)
            {
                var systemGravity = physicsService.Gravity;
                actor.RigidBody.UpdateGravity(ref systemGravity);
            }

            base.OnCharacterLeaving(actor);
        }
    }
}
