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

namespace Willcraftia.Xna.Foundation.Cube.Scenes
{
    public sealed class CubeAnimateCharacter
    {
        #region Inner classes

        public enum ModelType
        {
            Front, Back, Left, Right
        }

        public sealed class Animation
        {
            public int FramesPerSecond;
            public int[] ModelIndices;

            public int FrameCount
            {
                get { return ModelIndices != null ? ModelIndices.Length : 0; }
            }
        }

        #endregion

        #region Fields and Properties

        public List<Model> FrontModels;
        public List<Model> BackModels;
        public List<Model> LeftModels;
        public List<Model> RightModels;
        public Dictionary<string, Animation> Animations;
        public int SpriteWidth;
        public int SpriteHeight;
        public float ModelScale;
        public float CubeScale;

        #endregion

        public BoundingBox CalculateModelBoundingBox()
        {
            float width = ModelScale * CubeScale * (float) SpriteWidth;
            float height = ModelScale * CubeScale * (float) SpriteHeight;
            float halfWidth = width * 0.5f;
            float haltHeight = height * 0.5f;
            return new BoundingBox(
                new Vector3(-halfWidth, -haltHeight, -halfWidth),
                new Vector3(halfWidth, haltHeight, halfWidth));
        }

        public void ApplyBaseTransformToFrontModels(Matrix value)
        {
            foreach (var model in FrontModels)
            {
                model.Root.Transform *= value;
            }
        }

        public void ApplyBaseTransformToBackModels(Matrix value)
        {
            foreach (var model in BackModels)
            {
                model.Root.Transform *= value;
            }
        }

        public void ApplyBaseTransformToLeftModels(Matrix value)
        {
            foreach (var model in LeftModels)
            {
                model.Root.Transform *= value;
            }
        }

        public void ApplyBaseTransformToRightModels(Matrix value)
        {
            foreach (var model in RightModels)
            {
                model.Root.Transform *= value;
            }
        }
    }
}
