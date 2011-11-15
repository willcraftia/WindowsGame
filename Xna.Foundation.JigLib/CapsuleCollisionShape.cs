#region Using

using Microsoft.Xna.Framework;
using JigLibX.Geometry;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    public sealed class CapsuleCollisionShape : Capsule, ICollisionShape
    {
        public CapsuleCollisionShape()
            : base(Vector3.Zero, Matrix.Identity, 1, 1)
        {
        }

        public override Primitive Clone()
        {
            var clone = new CapsuleCollisionShape();
            clone.Position = Transform.Position;
            clone.Orientation = Transform.Orientation;
            clone.Radius = Radius;
            clone.Length = Length;
            return clone;
        }

        #region ICollisionShape

        float staticFriction;
        public float StaticFriction
        {
            get { return staticFriction; }
            set { staticFriction = value; }
        }

        float dynamicFriction;
        public float DynamicFriction
        {
            get { return dynamicFriction; }
            set { dynamicFriction = value; }
        }

        float restitution;
        public float Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }

        public float Volume
        {
            get { return GetVolume(); }
        }

        public bool Intersects(ref Vector3 position, ref Vector3 vector,
            out Vector3 resultPosition, out Vector3 resultNormal, out float resultFraction)
        {
            var segment = new Segment(position, vector);
            return SegmentIntersect(out resultFraction, out resultPosition, out resultNormal, segment);
        }

        #endregion
    }
}
