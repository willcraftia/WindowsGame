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
    public class UsmShadowCamera : LightCamera
    {
        #region Fields and Properties

        protected BoundingFrustum CameraFrustum { get; private set; }
        protected Vector3[] frustumCorners;

        #endregion

        #region Constructors

        public UsmShadowCamera()
        {
            CameraFrustum = new BoundingFrustum(Pov.ViewProjection);
            frustumCorners = new Vector3[BoundingFrustum.CornerCount];
        }

        #endregion

        public override void Prepare()
        {
            //
            // initialize light volume vectors
            //
            lightVolumePoints.Clear();

            // add points from referenceCamera frustum
            CameraFrustum.Matrix = Pov.ViewProjection;
            CameraFrustum.GetCorners(frustumCorners);
            lightVolumePoints.AddRange(frustumCorners);

            // add points of objects casting shadowColor
            lightVolumePoints.AddRange(additionalLightVolumePoints);

            var up = Vector3.Up;

            //
            // transform light volume points into light space
            //
            var position = Vector3.Zero;
            var target = LightDirection;
            target.Normalize();
            Matrix temporalLightView;
            Matrix.CreateLookAt(ref position, ref target, ref up, out temporalLightView);

            var tempBox = BoundingBox.CreateFromPoints(lightVolumePoints);
            tempBox.GetCorners(frustumCorners);
            for (int i = 0; i < frustumCorners.Length; i++)
            {
                var vector = frustumCorners[i];
                Vector3.Transform(ref vector, ref temporalLightView, out vector);
                frustumCorners[i] = vector;
            }

            var lightBox = BoundingBox.CreateFromPoints(frustumCorners);

            var boxSize = lightBox.Max - lightBox.Min;
            var halfBoxSize = boxSize * 0.5f;

            //
            // calculate the light position in the light space
            //
            var lightPosition = lightBox.Min + halfBoxSize;
            //
            // TODO: i think that should use max z but xna sample use min z
            //
            //lightPosition.Z = lightBox.Min.Z;
            lightPosition.Z = lightBox.Max.Z;

            //
            // transform the light position from the light space into the World space
            //
            Matrix lightViewInv;
            Matrix.Invert(ref temporalLightView, out lightViewInv);
            Vector3.Transform(ref lightPosition, ref lightViewInv, out lightPosition);

            target = lightPosition + LightDirection;
            Matrix.CreateLookAt(ref lightPosition, ref target, ref up, out LightView);
            Matrix.CreateOrthographic(boxSize.X, boxSize.Y, -boxSize.Z, boxSize.Z, out LightProjection);

            Matrix.Multiply(ref LightView, ref LightProjection, out LightViewProjection);
        }
    }
}
