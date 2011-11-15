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

namespace Willcraftia.Xna.Framework.Audio
{
    public interface IAudioService
    {
        IAudioManager SharedAudioManager { get; }

        void SetVolume(string categoryName, float volume);

        /// <summary>
        /// Create an audio manager, then register it.
        /// </summary>
        /// <param name="name">The name of audio manager.</param>
        /// <param name="config">The configuration of audio manager.</param>
        /// <returns>The created audio manager.</returns>
        IAudioManager CreateAudioManager(string name, AudioConfig config);

        /// <summary>
        /// Gets the audio manager.
        /// </summary>
        /// <param name="name">The name of audio manager.</param>
        /// <returns>The audio manager.</returns>
        IAudioManager GetAudioManager(string name);
    }
}
