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
    public sealed class AudioService : GameComponent, IAudioService
    {
        #region Inner classes

        #region XACT implementation

        sealed class XactSound : ISound
        {
            #region Fields and Properties

            XactAudioManager owner;
            internal Cue Cue;

            #endregion

            #region Constructors

            internal XactSound(XactAudioManager owner)
            {
                this.owner = owner;
            }

            #endregion

            #region ISound

            public string Name
            {
                get { return Cue.Name; }
            }

            public bool IsDisposed
            {
                get { return !CueEnabled; }
            }

            public bool IsPaused
            {
                get { return CueEnabled && Cue.IsPaused; }
            }

            public bool IsPlaying
            {
                get { return CueEnabled && Cue.IsPlaying; }
            }

            public bool IsStopped
            {
                get { return CueEnabled && Cue.IsStopped; }
            }

            public AudioEmitter Emitter { get; internal set; }

            public void Pause()
            {
                if (CueEnabled)
                {
                    Cue.Pause();
                }
            }

            public void Play()
            {
                if (CueEnabled)
                {
                    Cue.Play();
                }
            }

            public void Resume()
            {
                if (CueEnabled)
                {
                    Cue.Resume();
                }
            }

            public void Stop(bool immediate)
            {
                if (CueEnabled)
                {
                    var option = immediate ? AudioStopOptions.Immediate : AudioStopOptions.AsAuthored;
                    Cue.Stop(option);
                }
            }

            #endregion

            bool CueEnabled
            {
                get { return Cue != null && !Cue.IsDisposed; }
            }
        }

        sealed class XactAudioManager : IAudioManager, IDisposable
        {
            #region Fields and Properties

            AudioService audioService;
            internal WaveBank WaveBank;
            internal SoundBank SoundBank;

            Stack<XactSound> soundPool;
            List<XactSound> activeSounds;

            #endregion

            #region Constructors

            internal XactAudioManager(AudioService audioService, string name)
            {
                if (audioService == null)
                {
                    throw new ArgumentNullException("audioService");
                }
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name");
                }
                this.audioService = audioService;
                this.name = name;
                listener = new AudioListener();
                soundPool = new Stack<XactSound>();
                activeSounds = new List<XactSound>();
            }

            #endregion

            #region IAudioManager

            string name;
            public string Name
            {
                get { return name; }
            }

            public bool IsDisposed
            {
                get { return disposed; }
            }

            AudioListener listener;
            public AudioListener Listener
            {
                get { return listener; }
            }

            public ISound CreateSound(string name)
            {
                return CreateSound(name, null);
            }

            public ISound CreateSound(string name, AudioEmitter emitter)
            {
                // borrow/create the sound implementation
                XactSound sound;
                if (0 < soundPool.Count)
                {
                    sound = soundPool.Pop();
                }
                else
                {
                    sound = new XactSound(this);
                }

                // set fields
                sound.Cue = SoundBank.GetCue(name);
                sound.Emitter = emitter;
                if (emitter != null)
                {
                    sound.Cue.Apply3D(listener, emitter);
                }

                // add to the active list
                activeSounds.Add(sound);

                return sound;
            }

            public void Update(GameTime gameTime)
            {
                int i = 0;
                while (i < activeSounds.Count)
                {
                    var sound = activeSounds[i];
                    if (sound.IsStopped)
                    {
                        // dispose it
                        sound.Cue.Dispose();
                        sound.Cue = null;
                        sound.Emitter = null;

                        // return it to the pool
                        soundPool.Push(sound);
                        // remove it from the active list
                        activeSounds.RemoveAt(i);
                    }
                    else
                    {
                        if (sound.Emitter != null)
                        {
                            // update its 3D settings
                            sound.Cue.Apply3D(listener, sound.Emitter);
                        }
                        i++;
                    }
                }
            }

            #endregion

            #region IDisposable

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region Destructor/Dispose method and fields

            bool disposed;

            ~XactAudioManager()
            {
                Dispose(false);
            }

            void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        foreach (XactSound sound in activeSounds)
                        {
                            sound.Cue.Dispose();
                            sound.Cue = null;
                            sound.Emitter = null;
                        }
                        activeSounds.Clear();
                        soundPool.Clear();

                        WaveBank.Dispose();
                        SoundBank.Dispose();
                    }
                    disposed = true;
                }
            }

            #endregion
        }

        #endregion

        #region SoundEffect implementation

        sealed class BasicSound : ISound
        {
            #region Fields and Properties

            BasicAudioManager owner;
            internal string CategoryName;
            internal SoundEffectInstance SoundEffectInstance;

            #endregion

            #region Constructors

            internal BasicSound(BasicAudioManager owner)
            {
                this.owner = owner;
            }
            
            #endregion

            #region ISound

            public string Name { get; internal set; }

            public bool IsDisposed
            {
                get { return !SoundEffectInstanceEnabled; }
            }

            public bool IsPaused
            {
                get
                {
                    return SoundEffectInstanceEnabled &&
                        SoundEffectInstance.State == SoundState.Paused;
                }
            }

            public bool IsPlaying
            {
                get
                {
                    return SoundEffectInstanceEnabled &&
                        SoundEffectInstance.State == SoundState.Playing;
                }
            }

            public bool IsStopped
            {
                get{
                    return SoundEffectInstanceEnabled &&
                        SoundEffectInstance.State == SoundState.Stopped;
                }
            }

            public AudioEmitter Emitter { get; internal set; }

            public void Pause()
            {
                if (SoundEffectInstanceEnabled)
                {
                    SoundEffectInstance.Pause();
                }
            }

            public void Play()
            {
                if (SoundEffectInstanceEnabled)
                {
                    SoundEffectInstance.Play();
                }
            }

            public void Resume()
            {
                if (SoundEffectInstanceEnabled)
                {
                    SoundEffectInstance.Resume();
                }
            }

            public void Stop(bool immediate)
            {
                if (SoundEffectInstanceEnabled)
                {
                    SoundEffectInstance.Stop();
                }
            }

            #endregion

            bool SoundEffectInstanceEnabled
            {
                get { return SoundEffectInstance != null && !SoundEffectInstance.IsDisposed; }
            }
        }

        sealed class BasicSoundEffect
        {
            #region Fields and Properties

            SoundEffect effect;
            public SoundEffect Effect
            {
                get { return effect; }
            }

            string categoryName;
            public string CategoryName
            {
                get { return categoryName; }
            }

            #endregion

            #region Constructors

            internal BasicSoundEffect(string categoryName, SoundEffect effect)
            {
                if (string.IsNullOrEmpty(categoryName))
                {
                    throw new ArgumentNullException("categoryName");
                }
                if (effect == null)
                {
                    throw new ArgumentNullException("effect");
                }
                this.categoryName = categoryName;
                this.effect = effect;
            }

            #endregion
        }

        sealed class BasicAudioManager : IAudioManager, IDisposable
        {
            #region Fields and Properties

            AudioService audioService;
            internal Dictionary<string, BasicSoundEffect> EffectDictionary;
            Stack<BasicSound> soundPool;
            List<BasicSound> activeSounds;

            #endregion

            #region Constructors

            internal BasicAudioManager(AudioService audioService, string name)
            {
                if (audioService == null)
                {
                    throw new ArgumentNullException("audioService");
                }
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name");
                }
                this.audioService = audioService;
                this.name = name;
                EffectDictionary = new Dictionary<string, BasicSoundEffect>();
                listener = new AudioListener();
                soundPool = new Stack<BasicSound>();
                activeSounds = new List<BasicSound>();
            }

            #endregion

            #region IAudioManager

            string name;
            public string Name
            {
                get { return name; }
            }

            public bool IsDisposed
            {
                get { return disposed; }
            }

            AudioListener listener;
            public AudioListener Listener
            {
                get { return listener; }
            }

            public ISound CreateSound(string name)
            {
                return CreateSound(name, null);
            }

            public ISound CreateSound(string name, AudioEmitter emitter)
            {
                // borrow/create the sound implementation
                BasicSound sound;
                if (0 < soundPool.Count)
                {
                    sound = soundPool.Pop();
                }
                else
                {
                    sound = new BasicSound(this);
                }

                // set fields
                var basicSoundEffect = EffectDictionary[name];
                sound.Name = name;
                sound.CategoryName = basicSoundEffect.CategoryName;
                sound.SoundEffectInstance = basicSoundEffect.Effect.CreateInstance();
                sound.SoundEffectInstance.Volume =
                    audioService.basicSoundVolumes[sound.CategoryName];
                sound.Emitter = emitter;
                if (emitter != null)
                {
                    sound.SoundEffectInstance.Apply3D(listener, emitter);
                }

                // add to the active list
                activeSounds.Add(sound);

                return sound;
            }

            public void Update(GameTime gameTime)
            {
                int i = 0;
                while (i < activeSounds.Count)
                {
                    var sound = activeSounds[i];
                    if (sound.IsStopped)
                    {
                        // dispose it
                        sound.SoundEffectInstance.Dispose();
                        sound.SoundEffectInstance = null;
                        sound.Emitter = null;

                        // return it to the pool
                        soundPool.Push(sound);
                        // remove it from the active list
                        activeSounds.RemoveAt(i);
                    }
                    else
                    {
                        if (sound.Emitter != null)
                        {
                            // update its 3D settings
                            sound.SoundEffectInstance.Apply3D(listener, sound.Emitter);
                        }
                        
                        // set volume
                        sound.SoundEffectInstance.Volume =
                            audioService.basicSoundVolumes[sound.CategoryName];

                        i++;
                    }
                }
            }

            #endregion

            #region IDisposable

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region Destructor/Dispose method and fields

            bool disposed;

            ~BasicAudioManager()
            {
                Dispose(false);
            }

            void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        foreach (BasicSound sound in activeSounds)
                        {
                            sound.SoundEffectInstance.Dispose();
                            sound.SoundEffectInstance = null;
                            sound.Emitter = null;
                        }
                        activeSounds.Clear();
                        soundPool.Clear();

                        foreach (BasicSoundEffect effect in EffectDictionary.Values)
                        {
                            effect.Effect.Dispose();
                        }
                        EffectDictionary.Clear();
                    }
                    disposed = true;
                }
            }

            #endregion
        }

        #endregion

        #region KeyedCollection for IAudioManager

        sealed class AudioManagerCollection : KeyedCollection<string, IAudioManager>
        {
            #region Constructors

            public AudioManagerCollection()
            {
            }

            #endregion

            protected override string GetKeyForItem(IAudioManager item)
            {
                return item.Name;
            }
        }

        #endregion

        #endregion

        #region Fields and Properties

        public const string DefaultXactSettingsFile = "Content/Audio/Audio.xgs";
        public const string DefaultWaveBankFile = "Content/Audio/Wave Bank.xwb";
        public const string DefaultSoundBankFile = "Content/Audio/Sound Bank.xsb";
        public const string DefaultBasicSoundCategoryName = "Default";
        public const float DefaultBasicSoundVolume = 1.0f;

        public const string SharedAudioManagerName = "Shared";
        public const string DefaultSharedAudioConfig = "Audio/AudioConfig";

        string xactSettingsFile;
        bool xactEnabled;
        AudioEngine audioEngine;
        public AudioEngine AudioEngine
        {
            get { return audioEngine; }
        }
        AudioManagerCollection audioManagers;
        ContentManager content;
        Dictionary<string, float> basicSoundVolumes;

        string sharedAudioConfigAssetName = DefaultSharedAudioConfig;
        public string SharedAudioConfigAssetName
        {
            get { return sharedAudioConfigAssetName; }
            set { sharedAudioConfigAssetName = value; }
        }

        #endregion

        #region Constructors

        public AudioService(Game game)
            : this(game, false, null)
        {
        }

        public AudioService(Game game, bool xactEnabled)
            : this(game, xactEnabled, null)
        {
        }

        public AudioService(Game game, bool xactEnabled, string xactSettingFile)
            : base(game)
        {
            this.xactEnabled = xactEnabled;
            if (xactEnabled)
            {
                if (string.IsNullOrEmpty(xactSettingFile))
                {
                    this.xactSettingsFile = DefaultXactSettingsFile;
                }
                else
                {
                    this.xactSettingsFile = xactSettingFile;
                }
            }

            audioManagers = new AudioManagerCollection();
            content = new ContentManager(Game.Services);
            basicSoundVolumes = new Dictionary<string, float>();
            basicSoundVolumes[DefaultBasicSoundCategoryName] = DefaultBasicSoundVolume;
        }

        #endregion

        #region Initialize

        public override void Initialize()
        {
            if (xactEnabled)
            {
                audioEngine = new AudioEngine(xactSettingsFile);
            }

            var audioConfig = Game.Content.Load<AudioConfig>(sharedAudioConfigAssetName);
            CreateAudioManager(SharedAudioManagerName, audioConfig);

            base.Initialize();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int i = 0;
            while (i < audioManagers.Count)
            {
                var manager = audioManagers[i];
                if (manager.IsDisposed)
                {
                    audioManagers.RemoveAt(i);
                }
                else
                {
                    manager.Update(gameTime);
                    i++;
                }
            }

            if (xactEnabled)
            {
                audioEngine.Update();
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (audioEngine != null)
                {
                    audioEngine.Dispose();
                }

                foreach (var manager in audioManagers)
                {
                    manager.Dispose();
                }
                audioManagers.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region IAudioService

        public IAudioManager SharedAudioManager
        {
            get { return audioManagers[SharedAudioManagerName]; }
        }

        public void SetVolume(string categoryName, float volume)
        {
            if (xactEnabled)
            {
                var category = audioEngine.GetCategory(categoryName);
                category.SetVolume(volume);
            }

            basicSoundVolumes[categoryName] = volume;
        }

        public IAudioManager CreateAudioManager(string name, AudioConfig config)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            // create
            IAudioManager audioManager;
            if (config.XactEnabled)
            {
                if (!xactEnabled)
                {
                    throw new InvalidOperationException(
                        "The current audio service does not support XACT.");
                }

                audioManager = CreateXactAudioManager(name, config);
            }
            else
            {
                audioManager = CreateBasicAudioManager(name, config);
            }

            // register
            audioManagers.Add(audioManager);

            return audioManager;
        }

        public IAudioManager GetAudioManager(string name)
        {
            return audioManagers[name];
        }

        #region CreateXactAudioManager

        /// <summary>
        /// Create a XACT based audio manager.
        /// </summary>
        /// <param name="name">The name of audio manager.</param>
        /// <param name="config">The configuration of audio manager.</param>
        /// <returns>The created audio manager.</returns>
        IAudioManager CreateXactAudioManager(string name, AudioConfig config)
        {
            var manager = new XactAudioManager(this, name);

            manager.WaveBank = CreateWaveBank(config.WaveBank);
            manager.SoundBank = CreateSoundBank(config.SoundBank);

            return manager;
        }

        /// <summary>
        /// Create a wave bank.
        /// </summary>
        /// <param name="config">The configuration of wave bank.</param>
        /// <returns>The created wave bank.</returns>
        WaveBank CreateWaveBank(WaveBankConfig config)
        {
            if (config == null)
            {
                return new WaveBank(
                    audioEngine,
                    DefaultWaveBankFile);
            }
            else if (config.IsStreaming)
            {
                var filename = config.Filename;
                if (string.IsNullOrEmpty(filename))
                {
                    filename = DefaultWaveBankFile;
                }
                return new WaveBank(
                    audioEngine,
                    filename,
                    config.Offset,
                    config.Packetsize);
            }
            else
            {
                var filename = config.Filename;
                if (string.IsNullOrEmpty(filename))
                {
                    filename = DefaultWaveBankFile;
                }
                return new WaveBank(
                    audioEngine,
                    filename);
            }
        }

        /// <summary>
        /// Create a sound bank.
        /// </summary>
        /// <param name="config">The configuration of sound bank.</param>
        /// <returns>The created sound bank.</returns>
        SoundBank CreateSoundBank(SoundBankConfig config)
        {
            if (config == null)
            {
                return new SoundBank(
                    audioEngine,
                    DefaultSoundBankFile);
            }
            else
            {
                var filename = config.Filename;
                if (string.IsNullOrEmpty(filename))
                {
                    filename = DefaultSoundBankFile;
                }
                return new SoundBank(
                    audioEngine,
                    filename);
            }
        }

        #endregion

        #region CreateBasicAudioManager

        /// <summary>
        /// Create a SoundEffect based audio manage.
        /// </summary>
        /// <param name="name">The name of audio manager.</param>
        /// <param name="config">The configuration of audio manager.</param>
        /// <returns>The created audio manager.</returns>
        IAudioManager CreateBasicAudioManager(string name, AudioConfig config)
        {
            var manager = new BasicAudioManager(this, name);
            foreach (var soundConfig in config.BasicSounds)
            {
                var effect = content.Load<SoundEffect>(soundConfig.AssetName);
                var categoryName = soundConfig.CategoryName;
                if (string.IsNullOrEmpty(categoryName))
                {
                    categoryName = DefaultBasicSoundCategoryName;
                }
                if (!basicSoundVolumes.ContainsKey(categoryName))
                {
                    basicSoundVolumes[categoryName] = DefaultBasicSoundVolume;
                }
                manager.EffectDictionary[soundConfig.SoundName] =
                    new BasicSoundEffect(categoryName, effect);
            }

            return manager;
        }

        #endregion

        #endregion
    }
}
