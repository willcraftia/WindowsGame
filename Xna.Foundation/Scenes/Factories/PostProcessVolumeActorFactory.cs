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

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Factories
{
    /// <summary>
    /// PostProcessVolumeActor の生成を担う ActoryFactory です。
    /// </summary>
    public sealed class PostProcessVolumeActorFactory : ActorFactory
    {
        /// <summary>
        /// VolumeActorConfig の VolumeActor をそのまま返します。
        /// </summary>
        /// <param name="config">VolumeActorConfig。</param>
        /// <returns>CameraActorConfig の VolumeActor。</returns>
        protected override Actor CreateActorInstance(ActorConfig config)
        {
            var actualActorConfig = config as ActualActorConfig;
            if (actualActorConfig == null)
            {
                throw new ArgumentException("config is not ActualActorConfig.");
            }
            return actualActorConfig.Actor;
        }
    }
}
