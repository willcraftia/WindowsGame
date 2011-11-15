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
    public sealed class AmbientSound
    {
        #region Fields and Properties

        public const float DefaultPlayableRadius = 1000.0f;

        string name;

        [ContentSerializer(AllowNull = false)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        bool enabled = true;

        [DefaultValue(true)]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        float playableRadiusSquared = DefaultPlayableRadius * DefaultPlayableRadius;
        float playableRadius = DefaultPlayableRadius;
        public float PlayableRadius
        {
            get { return playableRadius; }
            set
            {
                if (playableRadius != value)
                {
                    playableRadius = value;
                    playableRadiusSquared = value * value;
                }
            }
        }
        
        [DefaultValue(null)]
        [ContentSerializer(Optional = true)]
        public AudioEmitter Emitter { get; set; }

        string soundName;

        [ContentSerializer(AllowNull = false)]
        public string SoundName
        {
            get { return soundName; }
            set { soundName = value; }
        }

        #endregion

        public bool IsInPlayableSphere(ref Vector3 position)
        {
            if (Emitter != null)
            {
                var emitterPositionn = Emitter.Position;

                float distanceSquared;
                Vector3.DistanceSquared(ref emitterPositionn, ref position, out distanceSquared);

                return distanceSquared <= playableRadiusSquared;
            }
            else
            {
                return true;
            }
        }
    }
}
