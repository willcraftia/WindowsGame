#region Using

using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Mock.Physics
{
    public class MockRigidBodyFactory : IRigidBodyFactory
    {
        #region IRigidBodyFactory

        public virtual IRigidBody CreateRigidBody()
        {
            return new MockRigidBody();
        }

        #endregion
    }
}
