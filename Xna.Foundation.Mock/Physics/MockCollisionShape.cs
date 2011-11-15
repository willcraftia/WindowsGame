#region Using

using Microsoft.Xna.Framework;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Mock.Physics
{
    public class MockCollisionShape : ICollisionShape
    {
        #region ICollisionShape

        public virtual float StaticFriction { get; set; }
        public virtual float DynamicFriction { get; set; }
        public virtual float Restitution { get; set; }
        public virtual float Volume { get; set; }

        public virtual bool Intersects(ref Vector3 position, ref Vector3 vector, out Vector3 intersectPosition, out Vector3 intersectNormal, out float intersectFraction)
        {
            intersectPosition = Vector3.Zero;
            intersectNormal = Vector3.Zero;
            intersectFraction = 0;
            return false;
        }

        #endregion
    }
}
