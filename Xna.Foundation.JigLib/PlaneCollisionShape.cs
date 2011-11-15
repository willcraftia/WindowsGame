#region Using

using Microsoft.Xna.Framework;
using JigLibX.Geometry;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    using JigLibXPlane = JigLibX.Geometry.Plane;

    /// <summary>
    /// JigLibX 環境での IPlaneCollisionShape 実装です。
    /// </summary>
    public sealed class PlaneCollisionShape : JigLibXPlane, IPlaneCollisionShape
    {
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        public PlaneCollisionShape()
            : base(Vector3.Up, 0)
        {
        }

        public override Primitive Clone()
        {
            var clone = new PlaneCollisionShape();
            clone.Normal = Normal;
            clone.D = D;
            clone.Transform = Transform;
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

        public bool Intersects(
            ref Vector3 position,
            ref Vector3 vector,
            out Vector3 resultPosition,
            out Vector3 resultNormal,
            out float resultFraction)
        {
            var segment = new Segment(position, vector);
            return SegmentIntersect(out resultFraction,  out resultPosition, out resultNormal, segment);
        }

        #endregion
    }
}
