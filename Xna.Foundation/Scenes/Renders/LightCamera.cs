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
    public abstract class LightCamera
    {
        #region Fields and Properties

        Pov pov = new Pov();
        /// <summary>
        /// Gets the view camera refered for calculating the light camera's view/projection matrix.
        /// </summary>
        public Pov Pov
        {
            get { return pov; }
        }
        
        /// <summary>
        /// The light direction.
        /// </summary>
        public Vector3 LightDirection = Vector3.Down;

        public Matrix LightView = Matrix.Identity;
        public Matrix LightProjection = Matrix.Identity;
        public Matrix LightViewProjection = Matrix.Identity;

        protected readonly List<Vector3> additionalLightVolumePoints = new List<Vector3>();
        protected readonly List<Vector3> lightVolumePoints = new List<Vector3>();

        public int ShadowMapSize { get; set; }

        Vector3[] boundingBoxCorners = new Vector3[8];

        #endregion

        #region Constructors

        public LightCamera()
        {
            Pov.AspectRatio = Pov.AspectRatio1x1;
        }

        #endregion

        #region Abstract methods

        public abstract void Prepare();

        #endregion

        public void AddLightVolumePoint(ref Vector3 point)
        {
            additionalLightVolumePoints.Add(point);
        }

        public void AddLightVolumePoints(Vector3[] points)
        {
            additionalLightVolumePoints.AddRange(points);
        }

        public void AddLightVolume(ref BoundingBox boundingBox)
        {
            boundingBox.GetCorners(boundingBoxCorners);
            for (int i = 0; i < boundingBoxCorners.Length; i++)
            {
                additionalLightVolumePoints.Add(boundingBoxCorners[i]);
            }
        }

        public void ClearLightVolumePoints()
        {
            additionalLightVolumePoints.Clear();
        }
    }
}
