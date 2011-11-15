#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Characters
{
    [ContentSerializerRuntimeType("Willcraftia.Xna.Foundation.Cube.Scenes.CubeAnimateCharacter, Willcraftia.Xna.Foundation.Cube")]
    public sealed class CubeAnimateCharacterContent
    {
        #region Inner classes

        [ContentSerializerRuntimeType("Willcraftia.Xna.Foundation.Cube.Scenes.CubeAnimateCharacter+Animation, Willcraftia.Xna.Foundation.Cube")]
        public sealed class AnimationContent
        {
            public int FramesPerSecond;
            public int[] ModelIndices;

            public AnimationContent(int framesPerSecond, params int[] modelIndices)
            {
                FramesPerSecond = framesPerSecond;
                ModelIndices = modelIndices;
            }
        }

        #endregion

        #region Fields and Properties

        public List<ModelContent> FrontModels = new List<ModelContent>();
        public List<ModelContent> BackModels = new List<ModelContent>();
        public List<ModelContent> LeftModels = new List<ModelContent>();
        public List<ModelContent> RightModels = new List<ModelContent>();
        public Dictionary<string, AnimationContent> Animations = new Dictionary<string, AnimationContent>();
        public int SpriteWidth;
        public int SpriteHeight;
        public float ModelScale;
        public float CubeScale;

        #endregion
    }
}
