#region Using

using System;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Mock.Physics
{
    public class MockCollisionShapeFactory : ICollisionShapeFactory
    {
        #region ICollisionShapeFactory

        public ICollisionShape CreateCollisionShape(Type type)
        {
            return new MockCollisionShape();
        }

        #endregion
    }
}
