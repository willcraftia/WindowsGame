#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation
{
    /// <summary>
    /// Model の拡張です。
    /// </summary>
    public static class ModelExtension
    {
        #region Extension

        /// <summary>
        /// Model 全体を包む BoundingBox を取得します。
        /// </summary>
        /// <param name="model">Model。</param>
        /// <returns>Model 全体を包む BoundingBox。</returns>
        public static BoundingBox GetBoundingBox(this Model model)
        {
            BoundingBox result;
            GetBoundingBox(model, out result);
            return result;
        }

        /// <summary>
        /// Model 全体を包む BoundingBox を取得します。
        /// </summary>
        /// <param name="model">Model。</param>
        /// <param name="result">Model 全体を包む BoundingBox。</param>
        public static void GetBoundingBox(this Model model, out BoundingBox result)
        {
            var boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            var points = new List<Vector3>();
            foreach (var mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    var vertexBuffer = part.VertexBuffer;
                    var data = new float[vertexBuffer.VertexCount * vertexBuffer.VertexDeclaration.VertexStride / sizeof(float)];
                    vertexBuffer.GetData<float>(data);
                    var boneTransform = boneTransforms[mesh.ParentBone.Index];
                    var increment = vertexBuffer.VertexDeclaration.VertexStride / sizeof(float);
                    for (int i = 0; i < data.Length; i += increment)
                    {
                        Vector3 point;
                        point.X = data[i];
                        point.Y = data[i + 1];
                        point.Z = data[i + 2];

                        point = Vector3.Transform(point, boneTransform);

                        points.Add(point);
                    }
                }
            }
            result = BoundingBox.CreateFromPoints(points);
        }

        /// <summary>
        /// Model 全体を包む BoundingSphere を取得します。
        /// </summary>
        /// <param name="model">Model。</param>
        /// <returns>Model 全体を包む BoundingSphere。</returns>
        public static BoundingSphere GetBoundingSphere(this Model model)
        {
            BoundingSphere result;
            GetBoundingSphere(model, out result);
            return result;
        }

        /// <summary>
        /// Model 全体を包む BoundingSphere を取得します。
        /// </summary>
        /// <param name="model">Model。</param>
        /// <param name="result">Model 全体を包む BoundingSphere。</param>
        public static void GetBoundingSphere(this Model model, out BoundingSphere result)
        {
            result = model.Meshes[0].BoundingSphere;
            foreach (var mesh in model.Meshes)
            {
                result = BoundingSphere.CreateMerged(result, mesh.BoundingSphere);
            }
        }

        #endregion
    }
}
