#region Using

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Mock.Physics
{
    public class MockCollisionBounds : ICollisionBounds
    {
        #region ICollisionBounds

        public virtual object Entity { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual float Volume { get; set; }
        public virtual float SurfaceArea { get; set; }
        public virtual bool IsCollided { get; set; }

        public virtual void AddCollisionShape(ICollisionShape collisionShape) { }

        public virtual bool IsCollidedForDirection(ref Vector3 direction)
        {
            return false;
        }

        public virtual bool Intersects(ref Vector3 position, ref Vector3 vector, out Vector3 intersectPosition, out Vector3 intersectNormal, out float intersectFraction)
        {
            intersectPosition = Vector3.Zero;
            intersectNormal = Vector3.Up;
            intersectFraction = 0;
            return false;
        }

        public virtual void UpdateCollisionShapeTransforms(ref Vector3 position, ref Matrix orientation) { }

        public virtual VertexPositionColor[] GetDebugWireframe()
        {
            return new VertexPositionColor[0];
        }

        #endregion
    }
}
