#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class BasicOrthographicLightCamera : LightCamera
    {
        #region Inner classes

        #endregion

        #region Fields and Properties

        BoundingFrustum cameraFrustum;
        Vector3[] frustumCorners;

        #endregion

        #region Constructors

        public BasicOrthographicLightCamera()
        {
            cameraFrustum = new BoundingFrustum(Pov.ViewProjection);
            frustumCorners = new Vector3[BoundingFrustum.CornerCount];
        }

        #endregion

        public override void Prepare()
        {
            //
            // initialize light volume vectors
            //
            lightVolumePoints.Clear();

            cameraFrustum.Matrix = Pov.ViewProjection;
            cameraFrustum.GetCorners(frustumCorners);

            //var frustumCenter = Vector3.Zero;
            //for (int i = 0; i < 8; i++)
            //{
            //    frustumCenter += frustumCorners[i];
            //}
            //frustumCenter /= 8.0f;

            //const float adjustment = 0;
            //var distanceFromCenter = MathHelper.Max(
            //    ReferenceCamera.FarPlaneDistance - ReferenceCamera.NearPlaneDistance,
            //    Vector3.Distance(frustumCorners[4], frustumCorners[5])) + adjustment;
            //LightView = Matrix.CreateLookAt(
            //    frustumCenter - LightDirection * distanceFromCenter,
            //    frustumCenter,
            //    Vector3.Up);

            //LightView = Matrix.CreateLookAt(
            //    ReferenceCamera.Position,
            //    ReferenceCamera.Position + LightDirection,
            //    Vector3.Up);

            LightView = Matrix.CreateLookAt(
                Vector3.Zero,
                LightDirection,
                Vector3.Up);

            //Vector3.Transform(frustumCorners, ref LightView, frustumCorners);

            lightVolumePoints.AddRange(frustumCorners);
            lightVolumePoints.AddRange(additionalLightVolumePoints);

            Vector3 initialPointLS;
            Vector3.Transform(ref frustumCorners[0], ref LightView, out initialPointLS);
            var min = initialPointLS;
            var max = initialPointLS;
            foreach (Vector3 point in lightVolumePoints)
            {
                Vector3 pointWS = point;
                Vector3 pointLS;
                Vector3.Transform(ref pointWS, ref LightView, out pointLS);
                if (max.X < pointLS.X)
                {
                    max.X = pointLS.X;
                }
                else if (pointLS.X < min.X)
                {
                    min.X = pointLS.X;
                }
                if (max.Y < pointLS.Y)
                {
                    max.Y = pointLS.Y;
                }
                else if (pointLS.Y < min.Y)
                {
                    min.Y = pointLS.Y;
                }
                if (max.Z < pointLS.Z)
                {
                    max.Z = pointLS.Z;
                }
                else if (pointLS.Z < min.Z)
                {
                    min.Z = pointLS.Z;
                }
            }

            //
            // REFERECE: http://msdn.microsoft.com/ja-jp/library/ee416324(VS.85).aspx
            //
            var texelSize = 1.0f / (float) ShadowMapSize;
            var minX = AdjustProjectionBoundary(min.X, texelSize);
            var minY = AdjustProjectionBoundary(min.Y, texelSize);
            var maxX = AdjustProjectionBoundary(max.X, texelSize);
            var maxY = AdjustProjectionBoundary(max.Y, texelSize);
            Matrix.CreateOrthographicOffCenter(minX, maxX, minY, maxY, -max.Z, -min.Z, out LightProjection);
            //Matrix.CreateOrthographicOffCenter(min.X, max.X, min.Y, max.Y, -max.Z, -min.Z, out LightProjection);

            Matrix.Multiply(ref LightView, ref LightProjection, out LightViewProjection);
        }

        float AdjustProjectionBoundary(float value, float texelSize)
        {
            var result = value;
            result /= texelSize;
            result = (float) Math.Floor(result);
            result *= texelSize;
            return result;
        }
    }
}
