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

namespace Willcraftia.Xna.Foundation.Scenes.Controllers
{
    public sealed class WorldTimeController : Controller
    {
        public override void Update(GameTime gameTime)
        {
            var sceneSettings = ControllerContext.Scene.SceneSettings;
            sceneSettings.Time += gameTime.GetDeltaTime() * sceneSettings.TimeScale;
        }
    }
}
