#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

#endregion

namespace Willcraftia.Xna.Framework.Audio
{
    public interface ISound
    {
        string Name { get; }
        bool IsDisposed { get; }
        bool IsPaused { get; }
        bool IsPlaying { get; }
        bool IsStopped { get; }
        AudioEmitter Emitter { get; }

        void Pause();
        void Play();
        void Resume();
        void Stop(bool immediate);
    }
}
