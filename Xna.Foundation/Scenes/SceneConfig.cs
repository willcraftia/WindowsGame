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

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class SceneConfig
    {
        [DefaultValue("Player")]
        public string PlayerCharacterActorName = "Player";

        List<ActorConfig> actors = new List<ActorConfig>();
        public List<ActorConfig> Actors
        {
            get { return actors; }
        }

        public string SceneAudioAssetName;
        public string CharacterAudioAssetName;

        AmbientSoundCollection ambientSounds = new AmbientSoundCollection();
        public AmbientSoundCollection AmbientSounds
        {
            get { return ambientSounds; }
        }

        SceneSettings sceneSettings = new SceneSettings();
        public SceneSettings SceneSettings
        {
            get { return sceneSettings; }
        }
    }
}
