#region Using

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Willcraftia.Xna.Framework.Physics;
using Willcraftia.Xna.Foundation.JigLib.Debugs;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// JigLibX 環境での ICollisionBounds 実装です。
    /// </summary>
    public sealed class JigLibXCollisionBounds : CollisionSkin, ICollisionBounds
    {
        #region ICollisionBounds

        object entity;
        public object Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;

                enabled = value;

                var collisionSystem = PhysicsSystem.CurrentPhysicsSystem.CollisionSystem;
                if (enabled)
                {
                    collisionSystem.AddCollisionSkin(this);
                }
                else
                {
                    collisionSystem.RemoveCollisionSkin(this);
                }
            }
        }

        public float Volume
        {
            get { return GetVolume(); }
        }

        public float SurfaceArea
        {
            get { return GetSurfaceArea(); }
        }

        public bool IsCollided
        {
            get { return 0 < Collisions.Count; }
        }

        public void AddCollisionShape(ICollisionShape collisionShape)
        {
            var primitive = collisionShape as Primitive;
            if (primitive == null) throw new ArgumentException("Invalid collision shape type.", "collisionShape");

            var material = new MaterialProperties(
                collisionShape.Restitution, collisionShape.StaticFriction, collisionShape.DynamicFriction);

            AddPrimitive(primitive, material);
        }

        public bool IsCollidedForDirection(ref Vector3 direction)
        {
            CollisionInfo collision;
            Vector3? normal;

            FindCollisionAndNormal(ref direction, out collision, out normal);

            return collision != null;
        }

        /// <summary>
        /// 指定の方向ベクトルについての衝突情報とその衝突法線ベクトルを探します。
        /// </summary>
        /// <param name="direction">判定する方向ベクトル。</param>
        /// <param name="resultCollision">衝突情報 (JigLibX)。</param>
        /// <param name="resultNormal">衝突法線ベクトル。</param>
        void FindCollisionAndNormal(ref Vector3 direction, out CollisionInfo resultCollision, out Vector3? resultNormal)
        {
            resultCollision = null;
            resultNormal = null;

            if (direction.IsZero()) return;

            // 衝突リストから指定された検査方向ベクトルの逆ベクトルを
            // 衝突法線ベクトルとする CollisionSkin を探します。
            // 逆ベクトルと法線ベクトルの一致性は、
            // その内積結果が一定の範囲内である場合に一致とみなします。

            var dir = -direction;
            dir.Normalize();

            foreach (var collision in Collisions)
            {
                var normal = collision.DirToBody0;
                if (collision.SkinInfo.Skin1 == this)
                {
                    Vector3.Negate(ref normal, out normal);
                }

                float nDotDir;
                Vector3.Dot(ref normal, ref dir, out nDotDir);
                if (0.7f < nDotDir)
                {
                    resultCollision = collision;
                    resultNormal = normal;
                    break;
                }
            }
        }

        public bool Intersects(
            ref Vector3 position, ref Vector3 vector,
            out Vector3 intersectPosition, out Vector3 intersectNormal, out float intersectFraction)
        {
            var segment = new Segment(position, vector);
            return SegmentIntersect(out intersectFraction, out intersectPosition, out intersectNormal, segment);
        }

        public void UpdateCollisionShapeTransforms(ref Vector3 position, ref Matrix orientation)
        {
            var transform = new Transform(position, orientation);
            ApplyLocalTransform(transform);
        }

        public VertexPositionColor[] GetDebugWireframe()
        {
            var wf = this.GetLocalSkinWireframe();

            if (Owner != null)
            {
                var transform = Owner.Transform;
                Matrix bodyTransform;

                Matrix translation;
                Matrix.CreateTranslation(ref transform.Position, out translation);
                Matrix.Multiply(ref transform.Orientation, ref translation, out bodyTransform);

                for (int i = 0; i < wf.Length; i++)
                {
                    Vector3.Transform(ref wf[i].Position, ref bodyTransform, out wf[i].Position);
                }
            }

            return wf;
        }

        #endregion
    }
}
