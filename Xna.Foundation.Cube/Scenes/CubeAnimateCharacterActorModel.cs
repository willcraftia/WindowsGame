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
using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Xna.Foundation.Cube.Scenes
{
    public sealed class CubeAnimateCharacterActorModel : ActorModel
    {
        #region Inner classes

        public enum PartType
        {
            Front,
            Back,
            Left,
            Right
        }

        #endregion

        #region Fields and Properties

        public static readonly Matrix FrontPartTransform = Matrix.Identity;
        public static readonly Matrix BackPartTransform = Matrix.Identity;
        public static readonly Matrix LeftPartTransform = Matrix.CreateFromYawPitchRoll(-MathHelper.PiOver2, 0, 0);
        public static readonly Matrix RightPartTransform = Matrix.CreateFromYawPitchRoll(MathHelper.PiOver2, 0, 0);

        string modelAssetName;
        public string ModelAssetName
        {
            get { return modelAssetName; }
            set { modelAssetName = value; }
        }

        CubeAnimateCharacter cubeAnimateCharacter;

        [Browsable(false)]
        [ContentSerializerIgnore]
        public CubeAnimateCharacter CubeAnimateCharacter
        {
            get { return cubeAnimateCharacter; }
            set { cubeAnimateCharacter = value; }
        }

        List<ModelActorModel> frontParts = new List<ModelActorModel>();
        List<ModelActorModel> backParts = new List<ModelActorModel>();
        List<ModelActorModel> leftParts = new List<ModelActorModel>();
        List<ModelActorModel> rightParts = new List<ModelActorModel>();

        PartType currentPartType = PartType.Front;

        [Browsable(false)]
        [ContentSerializerIgnore]
        public int CurrentPartIndex { get; set; }

        ModelActorModel CurrentPart
        {
            get
            {
                switch (currentPartType)
                {
                    case PartType.Back:
                        return backParts[CurrentPartIndex];
                    case PartType.Left:
                        return leftParts[CurrentPartIndex];
                    case PartType.Right:
                        return rightParts[CurrentPartIndex];
                    case PartType.Front:
                    default:
                        return frontParts[CurrentPartIndex];
                }
            }
        }

        Vector3[] corners = new Vector3[8];

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            cubeAnimateCharacter = Content.Load<CubeAnimateCharacter>(modelAssetName);
            LocalBoundingBox = cubeAnimateCharacter.CalculateModelBoundingBox();

            LoadParts();

            base.LoadContent();
        }

        void LoadParts()
        {
            foreach (var model in cubeAnimateCharacter.FrontModels)
            {
                var part = CreatePart(model);
                frontParts.Add(part);
            }
            foreach (var model in cubeAnimateCharacter.BackModels)
            {
                var part = CreatePart(model);
                backParts.Add(part);
            }
            foreach (var model in cubeAnimateCharacter.LeftModels)
            {
                var part = CreatePart(model);
                leftParts.Add(part);
            }
            foreach (var model in cubeAnimateCharacter.RightModels)
            {
                var part = CreatePart(model);
                rightParts.Add(part);
            }
        }

        ModelActorModel CreatePart(Model model)
        {
            var part = new ModelActorModel();
            part.Actor = Actor;
            part.Model = model;
            part.LoadContent();
            return part;
        }

        #endregion

        #region UnloadContent

        public override void UnloadContent()
        {
            frontParts.ForEach(part => { part.UnloadContent(); });
            backParts.ForEach(part => { part.UnloadContent(); });
            leftParts.ForEach(part => { part.UnloadContent(); });
            rightParts.ForEach(part => { part.UnloadContent(); });

            base.UnloadContent();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            PrepareCurrentPart(gameTime);
        }

        protected override void UpdateWorldBoundingBox(GameTime gameTime)
        {
            LocalBoundingBox.GetCorners(corners);

            Matrix transform;
            Matrix.Multiply(
                ref Actor.Scale,
                ref Actor.Translation,
                out transform);

            Vector3.Transform(corners, ref transform, corners);
            WorldBoundingBox = BoundingBox.CreateFromPoints(corners);
        }

        void PrepareCurrentPart(GameTime gameTime)
        {
            ResolvePartType(gameTime);

            CurrentPart.NearTransparencyEnabled = NearTransparencyEnabled;
            CurrentPart.CullingTransparencyEnabled = CullingTransparencyEnabled;

            CurrentPart.CastShadowEnabled = CastShadowEnabled;
            CurrentPart.MaxDrawDistance = MaxDrawDistance;

            CurrentPart.Update(gameTime);
        }

        void ResolvePartType(GameTime gameTime)
        {
            var cameraForward = ActorContext.ActiveCamera.Orientation.Forward;
            var modelForward = Actor.Orientation.Forward;

            var cameraDirection = new Vector2(-cameraForward.X, -cameraForward.Z);
            var modelDirection = new Vector2(modelForward.X, modelForward.Z);

            cameraDirection.NormalizeSafe(out cameraDirection);
            modelDirection.NormalizeSafe(out modelDirection);

            //
            // a = actor
            // b = referenceCamera
            //

            float cos;
            Vector2.Dot(ref modelDirection, ref cameraDirection, out cos);

            float sin =
                modelDirection.X * cameraDirection.Y -
                modelDirection.Y * cameraDirection.X;

            var angle = (float) Math.Atan2(sin, cos);

            if (-MathHelper.PiOver4 < angle && angle <= MathHelper.PiOver4)
            {
                currentPartType = CubeAnimateCharacterActorModel.PartType.Front;
            }
            else if (MathHelper.PiOver4 < angle && angle <= MathHelper.PiOver4 * 3)
            {
                currentPartType = CubeAnimateCharacterActorModel.PartType.Left;
            }
            else if (-MathHelper.PiOver4 * 3 < angle && angle < -MathHelper.PiOver4)
            {
                currentPartType = CubeAnimateCharacterActorModel.PartType.Right;
            }
            else
            {
                currentPartType = CubeAnimateCharacterActorModel.PartType.Back;
            }
        }

        #endregion

        #region Draw

        protected override void OnDraw(GameTime gameTime)
        {
            CurrentPart.Draw(gameTime);
        }

        public override void Draw(GameTime gameTime, Effect effect)
        {
            CurrentPart.Draw(gameTime, effect);
        }

        #endregion

        #region OnActorChanged

        protected override void OnActorChanged()
        {
            frontParts.ForEach(SetActorAction);
            backParts.ForEach(SetActorAction);
            leftParts.ForEach(SetActorAction);
            rightParts.ForEach(SetActorAction);

            base.OnActorChanged();
        }

        void SetActorAction(ModelActorModel actorModel)
        {
            actorModel.Actor = Actor;
        }

        #endregion
    }
}
