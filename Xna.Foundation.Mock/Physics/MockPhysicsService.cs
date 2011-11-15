#region Using

using Microsoft.Xna.Framework;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Mock.Physics
{
    public class MockPhysicsService : IPhysicsService
    {
        #region IPhysicsService

        public virtual IRigidBodyFactory RigidBodyFactory { get; set; }
        public virtual ICollisionBoundsFactory CollisionBoundsFactory { get; set; }
        public virtual ICollisionShapeFactory CollisionShapeFactory { get; set; }
        public virtual ICollisionTester CollisionTester { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual Vector3 Gravity { get; set; }

        #endregion

        public MockPhysicsService()
        {
            RigidBodyFactory = new MockRigidBodyFactory();
            CollisionBoundsFactory = new MockCollisionBoundsFactory();
            CollisionShapeFactory = new MockCollisionShapeFactory();
            CollisionTester = new MockCollisionTester();
        }
    }
}
