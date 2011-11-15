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

namespace Willcraftia.Xna.Framework.Content.Pipeline.Audio
{
    public sealed class WaveBankConfigDescription
    {
        public string Filename;
        public bool IsStreaming;
        public int Offset;
        public short Packetsize;
    }
}
