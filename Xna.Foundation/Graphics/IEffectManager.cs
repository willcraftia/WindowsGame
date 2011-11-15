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

namespace Willcraftia.Xna.Foundation.Graphics
{
    public interface IEffectManager
    {
        T Load<T>() where T : Effect;
        T Load<T>(string assetName) where T : Effect;
        T Load<T>(params object[] additionalParameters) where T : Effect;
        T Load<T>(string assetName, params object[] additionalParameters) where T : Effect;
    }
}
