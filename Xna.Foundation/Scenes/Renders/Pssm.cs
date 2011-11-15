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
using Willcraftia.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class Pssm
    {
        #region Fields and Properties

        public const int MaxSplitCount = 7;

        Pov pov = new Pov();
        public Pov Pov
        {
            get { return pov; }
        }

        int splitCount;
        public int SplitCount
        {
            get { return splitCount; }
            set
            {
                value = MathExtension.Clamp(value, 1, MaxSplitCount);
                if (splitCount != value)
                {
                    splitCount = value;
                    InitializeSplit();
                }
            }
        }

        float splitLambda = ShadowSettings.PssmSettings.Default.SplitLambda;
        public float SplitLambda
        {
            get { return splitLambda; }
            set { splitLambda = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        Vector3 lightDirection = Vector3.Down;
        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = value; }
        }

        Vector3[] boundingBoxCorners = new Vector3[BoundingBox.CornerCount];
        float cameraFarPlaneDistance;
        float inverseSplitCount;

        Matrix[] splitViewProjections;
        public Matrix[] SplitViewProjections
        {
            get
            {
                for (int i = 0; i < SplitCount; i++)
                {
                    splitViewProjections[i] = splitLightCamera[i].LightViewProjection;
                }
                return splitViewProjections;
            }
        }

        float[] splitDistances;
        public float[] SplitDistances
        {
            get { return splitDistances; }
        }

        BasicOrthographicLightCamera[] splitLightCamera;

        public int ShadowMapSize { get; set; }

        #endregion

        #region Constructors

        public Pssm()
            : this(ShadowSettings.PssmSettings.Default.SplitCount)
        {
        }

        public Pssm(int splitCount)
        {
            SplitCount = splitCount;
            pov.AspectRatio = Pov.AspectRatio1x1;
        }

        #endregion

        void InitializeSplit()
        {
            inverseSplitCount = 1.0f / (float) splitCount;
            splitViewProjections = new Matrix[splitCount];
            splitDistances = new float[splitCount + 1];

            splitLightCamera = new BasicOrthographicLightCamera[splitCount];
            for (int i = 0; i < splitCount; i++)
            {
                splitLightCamera[i] = new BasicOrthographicLightCamera();
            }
        }

        public LightCamera GetSplitLightCamera(int index)
        {
            return splitLightCamera[index];
        }

        public void PrepareSplitLightCameras(ref BoundingBox boundingBox)
        {
            CalculateCameraFarPlaneDistance(ref boundingBox);
            CalculateSplitDistances();

            Vector3 povPosition;
            Matrix povOrientation;
            pov.GetPosition(out povPosition);
            pov.GetOrientation(out povOrientation);

            for (int i = 0; i < SplitCount; i++)
            {
                var near = splitDistances[i];
                var far = splitDistances[i + 1];

                splitLightCamera[i].ShadowMapSize = ShadowMapSize;
                splitLightCamera[i].LightDirection = lightDirection;
                splitLightCamera[i].Pov.SetPosition(ref povPosition);
                splitLightCamera[i].Pov.SetOrientation(ref povOrientation);
                splitLightCamera[i].Pov.Fov = pov.Fov;
                splitLightCamera[i].Pov.AspectRatio = pov.AspectRatio;
                splitLightCamera[i].Pov.NearPlaneDistance = near;
                splitLightCamera[i].Pov.FarPlaneDistance = far;
                splitLightCamera[i].Pov.Update(false);
            }
        }

        void CalculateCameraFarPlaneDistance(ref BoundingBox boundingBox)
        {
            var view = pov.View;

            //
            // smaller z, more far
            // z == 0 means the point of view
            //
            var maxFar = 0.0f;
            boundingBox.GetCorners(boundingBoxCorners);
            for (int i = 0; i < BoundingBox.CornerCount; i++)
            {
                var z =
                    boundingBoxCorners[i].X * view.M13 +
                    boundingBoxCorners[i].Y * view.M23 +
                    boundingBoxCorners[i].Z * view.M33 +
                    view.M43;

                if (z < maxFar)
                {
                    maxFar = z;
                }
            }

            cameraFarPlaneDistance = pov.NearPlaneDistance - maxFar;
        }

        void CalculateSplitDistances()
        {
            var near = pov.NearPlaneDistance;
            var far = cameraFarPlaneDistance;
            var farNearRatio = far / near;

            for (int i = 0; i < splitCount; i++)
            {
                float idm = i * inverseSplitCount;
                float log = (float) (near * Math.Pow(farNearRatio, idm));

                // REFERENCE: the version in the main PSSM paper
                float uniform = near + (far - near) * idm;
                // REFERENCE: the version (?) in some actual codes,
                // i think the following is a wrong formula.
                //float uniform = (near + idm) * (far - near);

                splitDistances[i] = log * splitLambda + uniform * (1.0f - splitLambda);
            }

            splitDistances[0] = near;
            splitDistances[splitCount] = cameraFarPlaneDistance;
        }
    }
}
