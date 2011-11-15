#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Willcraftia.Xna.Foundation.Scenes.Controllers;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Mediators
{
    public sealed class SunLightMediator : Mediator
    {
        #region Fields and Properties

        // TODO: DefaultValue
        public Vector3 MiddaySunColor = Vector3.One;
        public Vector3 MidnightSunColor = new Vector3(0.01f);
        public Vector3 MidnightSunDirection = new Vector3(0, -1, 1);
        public Vector3 MiddayShadowColor = Color.DimGray.ToVector3();
        public Vector3 MidnightShadowColor = Vector3.One;
        public Vector3 MiddayFogColor = Vector3.One;
        public Vector3 MidnightFogColor = Vector3.Zero;

        #endregion

        public override void Execue()
        {
            MidnightSunDirection.Normalize();

            var controller = new SunLightController();

            controller.MiddaySunColor = MiddaySunColor;
            controller.MidnightSunColor = MidnightSunColor;
            controller.MidnightSunDirection = MidnightSunDirection;
            controller.MiddayShadowColor = MiddayShadowColor;
            controller.MidnightShadowColor = MidnightShadowColor;
            controller.MiddayFogColor = MiddayFogColor;
            controller.MidnightFogColor = MidnightFogColor;
            
            MediatorContext.Scene.Controllers.Add(controller);
        }
    }
}
