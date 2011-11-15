#region Using

using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Mock.Physics
{
    public class MockCollisionTester : ICollisionTester
    {
        #region ICollisionTester

        public bool OnCollided(ICollisionBounds bounds0, ICollisionBounds bounds1)
        {
            return false;
        }

        #endregion
    }
}
