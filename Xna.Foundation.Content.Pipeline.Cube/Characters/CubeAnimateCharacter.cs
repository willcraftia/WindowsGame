#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Characters
{
    public sealed class CubeAnimateCharacter
    {
        #region Inner classes

        public sealed class Animation
        {
            public int FramesPerSecond;
            public int[] SpriteIndices;

            public Animation()
            {
            }

            public Animation(int framesPerSecond, params int[] spriteIndices)
            {
                FramesPerSecond = framesPerSecond;
                SpriteIndices = spriteIndices;
            }

            public void Validate()
            {
                if (FramesPerSecond <= 0)
                {
                    throw new InvalidContentException( "Frame per second must be a positive value.");
                }
                if (SpriteIndices == null || SpriteIndices.Length == 0)
                {
                    throw new InvalidContentException("One or more sprite index definitions are required.");
                }
            }
        }

        #endregion

        #region Fields and Properties

        public ExternalReference<TextureContent> Sprite;
        public int SpriteWidth;
        public int SpriteHeight;
        public float ModelScale;
        public float CubeScale;
        public ExternalReference<CubeMaterial> Material;
        public List<Point> FrontSprites;
        public List<Point> BackSprites;
        public List<Point> LeftSprites;
        public List<Point> RightSprites;
        public Dictionary<string, Animation> Animations;

        #endregion

        public void Validate()
        {
            if (SpriteWidth <= 0)
            {
                throw new InvalidContentException("Sprite width must be a positive value.");
            }
            if (SpriteHeight <= 0)
            {
                throw new InvalidContentException("Sprite width must be a positive value.");
            }
            if (ModelScale <= 0)
            {
                throw new InvalidContentException("Model scale must be a positive value.");
            }
            if (CubeScale <= 0)
            {
                throw new InvalidContentException("Cube scale must be a positive value.");
            }
            if (FrontSprites == null || FrontSprites.Count == 0)
            {
                throw new InvalidContentException("One or more front sprite definitions are required.");
            }
            if (BackSprites == null || BackSprites.Count == 0)
            {
                throw new InvalidContentException("One or more back sprite definitions are required.");
            }
            if (LeftSprites == null || LeftSprites.Count == 0)
            {
                throw new InvalidContentException("One or more left sprite definitions are required.");
            }
            if (RightSprites == null || RightSprites.Count == 0)
            {
                throw new InvalidContentException("One or more right sprite definitions are required.");
            }
            if (FrontSprites.Count != BackSprites.Count ||
                FrontSprites.Count != LeftSprites.Count ||
                FrontSprites.Count != RightSprites.Count)
            {
                throw new InvalidContentException("Front, back, left and right sprites must be the same number.");
            }
            if (Animations == null ||
                Animations.Count == 0)
            {
                throw new InvalidContentException("One or more animation definitions are required.");
            }
            foreach (var animation in Animations.Values)
            {
                animation.Validate();
            }
        }
    }
}
