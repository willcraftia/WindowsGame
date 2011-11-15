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
using Willcraftia.Xna.Foundation.Scenes.Mediators;
using Willcraftia.Xna.Foundation.Cube.Scenes.Controllers;

#endregion

namespace Willcraftia.Xna.Foundation.Cube.Scenes.Mediators
{
    public sealed class CubeCharacterCameraMediator : CharacterCameraMediator
    {
        public override CharacterCameraController CreateCharacterCameraController()
        {
            return new CubeCharacterCameraController();
        }
    }
}
