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

namespace Willcraftia.Xna.Foundation.Scenes
{
    public struct EdgeDetectionSettings
    {
        #region Fields

        public float MapScale;
        public float FarPlaneDistance;
        public float EdgeWidth;
        public float EdgeIntensity;
        public float NormalThreshold;
        public float DepthThreshold;
        public float NormalSensitivity;
        public float DepthSensitivity;
        public Vector3 EdgeColor;

        #endregion

        #region Predefined

        public static EdgeDetectionSettings Default
        {
            get
            {
                return new EdgeDetectionSettings()
                {
                    MapScale = 1.0f,
                    FarPlaneDistance = 200,
                    EdgeWidth = 1.0f,
                    EdgeIntensity = 1.0f,
                    NormalThreshold = 0.1f,
                    DepthThreshold = 0.0f,
                    NormalSensitivity = 1.0f,
                    DepthSensitivity = 10.0f,
                    EdgeColor = Vector3.Zero
                };
            }
        }

        #endregion
    }
}
