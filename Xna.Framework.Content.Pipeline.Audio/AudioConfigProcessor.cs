#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

#endregion

namespace Willcraftia.Xna.Framework.Content.Pipeline.Audio
{
    [ContentProcessor(DisplayName = "Willcraftia Audio Config Processor")]
    public class AudioConfigProcessor : ContentProcessor<AudioConfigDescription, AudioConfigContent>
    {
        public override AudioConfigContent Process(AudioConfigDescription input, ContentProcessorContext context)
        {
            var result = new AudioConfigContent();

            result.XactEnabled = (input.XactEnabled.HasValue) ? input.XactEnabled.Value : true;

            result.WaveBank = new WaveBankConfigContent();
            result.WaveBank.Filename = input.WaveBank.Filename;
            result.WaveBank.IsStreaming = input.WaveBank.IsStreaming;
            result.WaveBank.Offset = input.WaveBank.Offset;
            result.WaveBank.Packetsize = input.WaveBank.Packetsize;

            result.SoundBank = new SoundBankConfigContent();
            result.SoundBank.Filename = input.SoundBank.Filename;

            result.BasicSounds = new List<BasicSoundConfigContent>();
            if (input.BasicSounds != null && 0 < input.BasicSounds.Count)
            {
                foreach (var basicSoundConfig in input.BasicSounds)
                {
                    var sound = new BasicSoundConfigContent();
                    sound.AssetName = basicSoundConfig.AssetName;
                    sound.SoundName = basicSoundConfig.SoundName;
                    sound.CategoryName = basicSoundConfig.CategoryName;
                    sound.IsLooped = basicSoundConfig.IsLooped;
                    result.BasicSounds.Add(sound);
                }
            }

            return result;
        }
    }
}