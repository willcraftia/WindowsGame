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
using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Xna.Foundation.Scenes.Controllers;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public abstract class Mediator
    {
        string name;

        [ContentSerializer(Optional = true)]
        [DefaultValue(null)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        IMediatorContext mediatorContext;

        [Browsable(false)]
        [ContentSerializerIgnore]
        public IMediatorContext MediatorContext
        {
            get { return mediatorContext; }
            set { mediatorContext = value; }
        }

        public abstract void Execue();
    }
}
