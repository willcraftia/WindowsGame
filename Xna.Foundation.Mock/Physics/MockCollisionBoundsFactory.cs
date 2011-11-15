#region Using

using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Mock.Physics
{
    public class MockCollisionBoundsFactory : ICollisionBoundsFactory
    {
        #region ICollisionBoundsFactory

        public virtual ICollisionBounds CreateCollisionBounds()
        {
            return new MockCollisionBounds();
        }

        #endregion
    }
}
