#region Using

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Collision;
using JigLibX.Geometry;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// JigLibX 環境での ISlopeCollisionShape 実装です。
    /// </summary>
    public sealed class SlopeCollisionShape : TriangleMesh, ISlopeCollisionShape
    {
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
            return SegmentIntersect(out resultFraction, out resultPosition, out resultNormal, segment);
        }

        #endregion

        #region ISlopeCollisionShape

        public void CreateMesh(Model model)
        {
            var box = model.GetBoundingBox();
            var vertices = new List<Vector3>();

            // stairs
            vertices.Add(new Vector3(box.Min.X, box.Min.Y, box.Max.Z));
            vertices.Add(new Vector3(box.Min.X, box.Max.Y, box.Min.Z));
            vertices.Add(new Vector3(box.Max.X, box.Max.Y, box.Min.Z));
            vertices.Add(new Vector3(box.Max.X, box.Min.Y, box.Max.Z));
            // west
            vertices.Add(new Vector3(box.Min.X, box.Min.Y, box.Min.Z));
            // east
            vertices.Add(new Vector3(box.Max.X, box.Min.Y, box.Min.Z));
            var indices = new List<TriangleVertexIndices>();
            // stairs
            indices.Add(new TriangleVertexIndices(2, 1, 0));
            indices.Add(new TriangleVertexIndices(3, 2, 0));
            // west
            indices.Add(new TriangleVertexIndices(0, 1, 4));
            // east
            indices.Add(new TriangleVertexIndices(5, 2, 3));
            // north
            indices.Add(new TriangleVertexIndices(4, 1, 2));
            indices.Add(new TriangleVertexIndices(4, 2, 5));
            // bottom
            indices.Add(new TriangleVertexIndices(0, 4, 5));
            indices.Add(new TriangleVertexIndices(0, 5, 3));

            CreateMesh(vertices, indices);
        }

        void CreateMesh(List<Vector3> vertices, List<TriangleVertexIndices> indices)
        {
            CreateMesh(vertices, indices, 4, 1.0f);
        }

        #endregion
    }
}
