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
using Willcraftia.Xna.Foundation.Graphics;
using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Xna.Foundation.Cube.Scenes
{
    public sealed class CubeAssetModelActorModel : AssetModelActorModel
    {
        protected override bool PreDraw(GameTime gameTime)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            return base.PreDraw(gameTime);
        }
    }
}
