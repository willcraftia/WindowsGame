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
using Willcraftia.Xna.Framework.Audio;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class AmbientSoundManager
    {
        #region Inner classes

        #endregion

        #region Fields and Properties

        IAudioManager audioManager;

        AmbientSoundCollection ambientSounds = new AmbientSoundCollection();
        public AmbientSoundCollection AmbientSounds
        {
            get { return ambientSounds; }
        }

        Dictionary<string, ISound> activeSounds = new Dictionary<string, ISound>();

        bool isPaused;
        public bool IsPaused
        {
            get { return isPaused; }
        }

        #endregion

        #region Constructors

        public AmbientSoundManager(IAudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        #endregion

        public void Play()
        {
            if (IsPaused)
            {
                return;
            }

            var listenerPosition = audioManager.Listener.Position;
            foreach (var ambientSound in ambientSounds)
            {
                ISound sound;
                activeSounds.TryGetValue(ambientSound.Name, out sound);

                if (ambientSound.Enabled && ambientSound.IsInPlayableSphere(ref listenerPosition))
                {
                    if (sound == null)
                    {
                        sound = audioManager.CreateSound(ambientSound.SoundName, ambientSound.Emitter);
                        activeSounds[ambientSound.Name] = sound;
                        sound.Play();
                    }
                }
                else
                {
                    if (sound != null)
                    {
                        if (!sound.IsDisposed)
                        {
                            sound.Stop(false);
                        }
                        else
                        {
                            activeSounds.Remove(ambientSound.Name);
                        }
                    }
                }
            }
        }

        public void Pause()
        {
            if (IsPaused)
            {
                return;
            }

            foreach (var sound in activeSounds.Values)
            {
                if (!sound.IsDisposed && sound.IsPlaying)
                {
                    sound.Pause();
                }
            }

            isPaused = true;
        }

        public void Resume()
        {
            if (!IsPaused)
            {
                return;
            }

            foreach (var sound in activeSounds.Values)
            {
                if (!sound.IsDisposed && sound.IsPaused)
                {
                    sound.Resume();
                }
            }

            isPaused = false;
        }

        public void PlayOrResume()
        {
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Play();
            }
        }
    }
}
