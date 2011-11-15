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
using Microsoft.Xna.Framework.Content.Pipeline.Audio;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Content.Pipeline.Audio
{
    public sealed class AudioConfigDescription
    {
        public bool? XactEnabled;
        public string XactSettingsFile;
        public WaveBankConfigDescription WaveBank;
        public SoundBankConfigDescription SoundBank;
        public List<BasicSoundConfigDescription> BasicSounds;
    }
}
