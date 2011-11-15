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
    public sealed class LspsmLightCamera : UsmShadowCamera
    {
        #region Fields and Properties

        public ShadowSettings.LspsmSettings LspsmSettings;
        public bool LspsmEnabled { get; set; }

        List<Vector3> lightVolumePointsClone;

        #endregion

        #region Constructors

        public LspsmLightCamera()
        {
            LspsmSettings = ShadowSettings.LspsmSettings.Default;
            LspsmEnabled = true;
            lightVolumePointsClone = new List<Vector3>();
        }

        #endregion

        public override void Prepare()
        {
            if (!LspsmEnabled)
            {
                //
                // Use USM
                //
                base.Prepare();
                return;
            }

            Vector3 povPosition;
            Matrix povOrientation;
            Pov.GetPosition(out povPosition);
            Pov.GetOrientation(out povOrientation);

            //
            // cacluate the angle between referenceCamera forward and light forward
            //
            var lightDirection = LightDirection;
            lightDirection.Normalize();
            var viewDirection = povOrientation.Forward;
            viewDirection.Normalize();

            float vDotL;
            Vector3.Dot(ref viewDirection, ref lightDirection, out vDotL);
            if (1.0f - 0.01f <= Math.Abs(vDotL))
            {
                //
                // Use USM
                //
                base.Prepare();
                return;
            }

            //
            // initialize light volume vectors
            //
            lightVolumePoints.Clear();
            lightVolumePointsClone.Clear();

            // add points from referenceCamera frustum
            CameraFrustum.Matrix = Pov.ViewProjection;
            CameraFrustum.GetCorners(frustumCorners);
            lightVolumePoints.AddRange(frustumCorners);

            // add points of objects casting shadowColor
            lightVolumePoints.AddRange(additionalLightVolumePoints);

            //
            // copy ligt volume vectors
            //
            lightVolumePointsClone.AddRange(lightVolumePoints);

            //
            // calculate up vector of the light
            //
            Vector3 up;
            Vector3 right;
            Vector3.Cross(ref lightDirection, ref viewDirection, out right);
            right.Normalize();
            Vector3.Cross(ref right, ref lightDirection, out up);
            up.Normalize();

            //
            // calculate the temporal source matrix of light space
            //
            var lightPosition = povPosition;
            var lightTarget = lightPosition + lightDirection;
            Matrix temporalLightView;
            Matrix.CreateLookAt(ref lightPosition, ref lightTarget, ref up, out temporalLightView);

            //
            // matrix light volume vectors from World into temporal light space
            //
            TransformCoordList(lightVolumePoints, ref temporalLightView);

            //
            // create the bounding box containing transformed light volumen vectors
            //
            var lightBox = BoundingBox.CreateFromPoints(lightVolumePoints);

            //
            // create a perspective frustum
            //
            float perspectiveDepth = lightBox.Max.Y - lightBox.Min.Y;

            // calculate near plane perspectiveDepth
            float near;
            if (LspsmSettings.ExplicitNEnabled)
            {
                near = LspsmSettings.N;
            }
            else
            {
                if (LspsmSettings.NewNFormulaEnabled)
                {
                    // new formula
                    var sinGamma = (float) Math.Sqrt(1.0d - vDotL * vDotL);
                    var factor = 1.0f / sinGamma;
                    var zNear = factor * Pov.NearPlaneDistance;

                    var z0 = -zNear;
                    var z1 = -(zNear + perspectiveDepth * sinGamma);
                    near = perspectiveDepth / ((float) Math.Sqrt(z1 / z0) - 1.0f);
                }
                else
                {
                    // original formula
                    var sinGamma = (float) Math.Sqrt(1.0d - vDotL * vDotL);
                    var factor = 1.0f / sinGamma;
                    var zNear = factor * Pov.NearPlaneDistance;

                    var zFar = zNear + perspectiveDepth * sinGamma;
                    //near = (zNear + (float) Math.Sqrt(zFar * zNear)) / sinGamma;
                    near = (zNear + (float) Math.Sqrt(zFar * zNear)) * factor;
                }
            }

            // far
            float far = near + perspectiveDepth;

            //
            // calculate the new ovserver position and the final source matrix of light space
            //
            lightPosition = povPosition - up * (near - Pov.NearPlaneDistance);
            lightTarget = lightPosition + lightDirection;
            Matrix.CreateLookAt(ref lightPosition, ref lightTarget, ref up, out LightView);

            //
            // create matrix projecting to y
            //
            // a = (far + near) / (far - near)
            // b = -2 * far * near / (far - near)
            //
            // [ 1 0 0 0]
            // [ 0 a 0 1]
            // [ 0 0 1 0]
            // [ 0 b 0 0]
            //
            var lispMatrix = Matrix.Identity;
            lispMatrix.M22 = (far + near) / (far - near);
            lispMatrix.M24 = 1;
            lispMatrix.M42 = -2.0f * far * near / (far - near);
            lispMatrix.M44 = 0;

            //
            // matrix light volume vectors from World into the distored light space with temporal transformation
            //
            Matrix temporalViewProjection;
            Matrix.Multiply(ref LightView, ref lispMatrix, out temporalViewProjection);
            TransformCoordList(lightVolumePointsClone, ref temporalViewProjection);

            //
            // create a bounding box from light volumen vectors
            //
            var tempBox = BoundingBox.CreateFromPoints(lightVolumePointsClone);

            //
            // create matrix matrix to rescale light source matrix
            //
            Matrix scaleTranslation;

            //
            // rescale 0: use orthographic
            //
            //var boxSize = tempBox.Max - tempBox.Min;
            //Matrix.CreateOrthographic(boxSize.X, boxSize.Y, -boxSize.Z, boxSize.Z, out scaleTranslation);

            //
            // rescale 2: use use orthographic-offcenter
            //
            Matrix.CreateOrthographicOffCenter(
                tempBox.Min.X, tempBox.Max.X,
                tempBox.Min.Y, tempBox.Max.Y,
                -tempBox.Max.Z, -tempBox.Min.Z,
                out scaleTranslation);

            //
            // create the final source matrix
            //
            Matrix.Multiply(ref lispMatrix, ref scaleTranslation, out LightProjection);

            //resultProjection = resultProjection * Matrix.CreateScale(1, 1, -1);
            //resultProjection = Matrix.CreateScale(1, 1, -1) * resultProjection;

            Matrix.Multiply(ref LightView, ref LightProjection, out LightViewProjection);
        }

        void CalculateNewDirection(out Vector3 result)
        {
            //lightVolumePoints;
            result = Vector3.Zero;

            Vector3 povPosition;
            Pov.GetPosition(out povPosition);

            for (int i = 0; i < lightVolumePoints.Count; i++)
            {
                Vector3 p = lightVolumePoints[i] - povPosition;
                result += p;
            }
            result.Normalize();
        }

        void TransformCoordList(List<Vector3> vectors, ref Matrix matrix)
        {
            for (int i = 0; i < vectors.Count; i++)
            {
                vectors[i] = vectors[i].TransformCoord(ref matrix);
            }
        }
    }
}
