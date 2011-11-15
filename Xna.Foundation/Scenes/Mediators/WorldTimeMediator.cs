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
using Willcraftia.Xna.Foundation.Scenes.Controllers;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Mediators
{
    public sealed class WorldTimeMediator : Mediator
    {
        public override void Execue()
        {
            MediatorContext.Scene.Controllers.Add(new WorldTimeController());
        }
    }
}
