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
    public interface IAudioManager : IDisposable
    {
        string Name { get; }
        bool IsDisposed { get; }
        AudioListener Listener { get; }

        ISound CreateSound(string name);
        ISound CreateSound(string name, AudioEmitter emitter);
        void Update(GameTime gameTime);
    }
}
