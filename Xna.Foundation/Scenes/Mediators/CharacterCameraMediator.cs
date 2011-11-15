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
    public class CharacterCameraMediator : Mediator
    {
        #region Fields and Properties

        #region Camera configuration

        [DefaultValue(null)]
        public string CameraName;

        [DefaultValue(CameraViewType.FirstPersonView)]
        public CameraViewType CameraViewType = CameraViewType.FirstPersonView;

        [DefaultValue(true)]
        public bool CanSwitchCameraViewType = true;

        [DefaultValue(1.0f)]
        public float MinCameraDistance = 1.0f;

        [DefaultValue(30.0f)]
        public float MaxCameraDistance = 30.0f;

        [DefaultValue(10.0f)]
        public float InitialOffsetDistance = 10.0f;

        // Z value equals InitialOffsetDistance.
        [DefaultValue(typeof(Vector3), "0, 0.5f, 10.0f")]
        public Vector3 InitialOffsetVector = new Vector3(0, 0.5f, 10.0f);

        [DefaultValue(10.0f)]
        public float ManualZoomVelocity = 10.0f;

        [DefaultValue(50.0f)]
        public float AutoZoomVelocity = 50.0f;

        [DefaultValue(3.0f)]
        public float ZoomMinCameraDistance = 3.0f;

        [DefaultValue(1.0f)]
        public float CameraCollisionRadius = 1.0f;

        #endregion

        #region Character configuration

        [DefaultValue(null)]
        public string CharacterActorName;

        [DefaultValue(5.0f)]
        public float MoveVelocity = 5.0f;

        [DefaultValue(10.0f)]
        public float DashVelocity = 10.0f;

        // 10 means against the gravity
        [DefaultValue(10.0f + 1.0f)]
        public float JumpVelocity = 10.0f + 1.0f;

        [DefaultValue(0.2f)]
        public float SlowlyMoveVelocityFactor = 0.2f;

        #endregion

        #endregion

        public virtual CharacterCameraController CreateCharacterCameraController()
        {
            return new CharacterCameraController();
        }

        protected virtual void PopulateCharacterCameraControllerProperties(CharacterCameraController controller)
        {
            controller.Camera = MediatorContext.Scene.Cameras[CameraName];
            controller.CameraViewType = CameraViewType;
            controller.CanSwitchCameraViewType = CanSwitchCameraViewType;
            controller.MinCameraDistance = MinCameraDistance;
            controller.MaxCameraDistance = MaxCameraDistance;
            controller.InitialOffsetDistance = InitialOffsetDistance;
            controller.InitialOffsetVector = InitialOffsetVector;
            controller.ManualZoomVelocity = ManualZoomVelocity;
            controller.AutoZoomVelocity = AutoZoomVelocity;
            controller.ZoomMinCameraDistance = ZoomMinCameraDistance;
            controller.CameraCollisionRadius = CameraCollisionRadius;

            controller.Character = MediatorContext.Scene.Characters[CharacterActorName];
            controller.MoveVelocity = MoveVelocity;
            controller.DashVelocity = DashVelocity;
            controller.JumpVelocity = JumpVelocity;
            controller.SlowlyMoveVelocityFactor = SlowlyMoveVelocityFactor;
        }

        public override void Execue()
        {
            var controller = CreateCharacterCameraController();
            PopulateCharacterCameraControllerProperties(controller);
            MediatorContext.Scene.Controllers.Add(controller);
        }
    }
}
