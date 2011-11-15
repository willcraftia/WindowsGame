#region Using

using System;
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
    /// JigLibX 環境での IMeshCollisionShape 実装です。
    /// </summary>
    public sealed class MeshCollisionShape : TriangleMesh, IMeshCollisionShape
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
            return SegmentIntersect(
                out resultFraction,
                out resultPosition,
                out resultNormal,
                segment);
        }

        #endregion

        #region IMeshCollisionShape

        public void CreateMesh(Model model)
        {
            var vertices = new List<Vector3>();
            var indices = new List<TriangleVertexIndices>();
            var boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (var mesh in model.Meshes)
            {
                var transform = boneTransforms[mesh.ParentBone.Index];
                foreach (var part in mesh.MeshParts)
                {
                    if (part.IndexBuffer.IndexElementSize != IndexElementSize.SixteenBits)
                    {
                        throw new InvalidOperationException(
                            "Model uses 32-bit indices, which are not supported.");
                    }

                    var declaration = part.VertexBuffer.VertexDeclaration;

                    var vertexPosition = new VertexElement();
                    foreach (var element in declaration.GetVertexElements())
                    {
                        if (element.VertexElementUsage == VertexElementUsage.Position &&
                            element.VertexElementFormat == VertexElementFormat.Vector3)
                        {
                            vertexPosition = element;
                            break;
                        }
                    }

                    if (vertexPosition.VertexElementUsage != VertexElementUsage.Position ||
                        vertexPosition.VertexElementFormat != VertexElementFormat.Vector3)
                    {
                        throw new InvalidOperationException(
                            "Model has no vertex element with Vector3 position.");
                    }

                    int offset = vertices.Count;
                    var vData = new Vector3[part.NumVertices];
                    part.VertexBuffer.GetData<Vector3>(
                        part.VertexOffset * declaration.VertexStride + vertexPosition.Offset,
                        vData,
                        0,
                        part.NumVertices,
                        declaration.VertexStride);

                    for (int i = 0; i < vData.Length; i++)
                    {
                        Vector3.Transform(
                            ref vData[i],
                            ref transform,
                            out vData[i]);
                    }

                    vertices.AddRange(vData);

                    var iData = new short[part.PrimitiveCount * 3];
                    part.IndexBuffer.GetData<short>(
                        part.StartIndex * 2,
                        iData,
                        0,
                        part.PrimitiveCount * 3);

                    for (int i = 0; i < part.PrimitiveCount; i++)
                    {
                        var tvi = new TriangleVertexIndices()
                        {
                            I0 = iData[i * 3 + 2] + offset,
                            I1 = iData[i * 3 + 1] + offset,
                            I2 = iData[i * 3 + 0] + offset
                        };
                        indices.Add(tvi);
                    }
                }
            }

            CreateMesh(vertices, indices);
        }

        void CreateMesh(List<Vector3> vertices, List<TriangleVertexIndices> indices)
        {
            CreateMesh(vertices, indices, 4, 1.0f);
        }

        #endregion
    }
}
