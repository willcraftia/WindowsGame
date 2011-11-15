#region Using

using JigLibX.Collision;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    public sealed class JigLibXRigidBodyFactory : IRigidBodyFactory
    {
        CollisionCallbackFn collisionCallbackFn;

        public JigLibXRigidBodyFactory(CollisionCallbackFn collisionCallbackFn)
        {
            this.collisionCallbackFn = collisionCallbackFn;
        }

        #region IRigidBodyFactory

        public IRigidBody CreateRigidBody()
        {
            var instance = new JigLibXRigidBody();
            instance.CollisionSkin.callbackFn += collisionCallbackFn;
            return instance;
        }

        #endregion
    }
}
