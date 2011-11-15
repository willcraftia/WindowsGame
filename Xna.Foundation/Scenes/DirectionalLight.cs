#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public struct DirectionalLight
    {
        #region Fields

        public Vector3 Direction;
        public Vector3 AmbientColor;
        public Vector3 DiffuseColor;
        public Vector3 SpecularColor;
        public Vector3 ShadowColor;
        public bool Enabled;
        public bool ShadowEnabled;

        #endregion

        #region Predefined

        public static DirectionalLight Default
        {
            get
            {
                return new DirectionalLight()
                {
                    Direction = Vector3.Down,
                    AmbientColor = Vector3.Zero,
                    DiffuseColor = Vector3.One,
                    SpecularColor = Vector3.One,
                    ShadowColor = Vector3.Zero,
                    Enabled = true,
                    ShadowEnabled = true
                };
            }
        }

        public static DirectionalLight None
        {
            get
            {
                return new DirectionalLight()
                {
                    Direction = Vector3.Down,
                    AmbientColor = Vector3.Zero,
                    DiffuseColor = Vector3.Zero,
                    SpecularColor = Vector3.Zero,
                    ShadowColor = Vector3.Zero,
                    Enabled = false,
                    ShadowEnabled = false
                };
            }
        }

        #endregion
    }
}
