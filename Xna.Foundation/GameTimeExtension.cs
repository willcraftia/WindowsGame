#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation
{
    /// <summary>
    /// GameTime の拡張です。
    /// </summary>
    public static class GameTimeExtension
    {
        #region Extensions

        public static void GetDeltaTime(this GameTime gameTime, out float result)
        {
            result = (float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static float GetDeltaTime(this GameTime gameTime)
        {
            float result;
            GetDeltaTime(gameTime, out result);
            return result;
        }

        #endregion
    }
}
