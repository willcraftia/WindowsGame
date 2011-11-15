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
    public sealed class CameraMediator : Mediator
    {
        #region Fields and Properties

        [DefaultValue(null)]
        public string CameraName;

        [DefaultValue(0.3f)]
        public float RotationVelocity = 0.3f;

        [DefaultValue(10.0f)]
        public float MoveVelocity = 10.0f;

        #endregion

        public override void Execue()
        {
            var controller = new CameraController();
            controller.CameraName = CameraName;
            controller.RotationVelocity = RotationVelocity;
            controller.MoveVelocity = MoveVelocity;

            MediatorContext.Scene.Controllers.Add(controller);
        }
    }
}
