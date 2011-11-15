#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Audio
{
    public sealed class AudioConfig
    {
        public bool XactEnabled;
        public WaveBankConfig WaveBank;
        public SoundBankConfig SoundBank;
        public List<BasicSoundConfig> BasicSounds;
    }
}
