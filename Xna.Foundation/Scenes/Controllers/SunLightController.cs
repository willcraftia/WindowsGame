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
using Willcraftia.Xna.Foundation.Scenes.Renders;
using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Controllers
{
    public sealed class SunLightController : Controller
    {
        #region Fields and Properties

        public Vector3 MiddaySunColor;
        public Vector3 MidnightSunColor;
        public Vector3 MidnightSunDirection;
        public Vector3 MiddayShadowColor;
        public Vector3 MidnightShadowColor;
        public Vector3 MiddayFogColor;
        public Vector3 MidnightFogColor;

        Vector3 sunRotationAxis;

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            MidnightSunDirection.Normalize();
            var right = Vector3.Cross(MidnightSunDirection, Vector3.Up);
            sunRotationAxis = Vector3.Cross(right, MidnightSunDirection);

            base.LoadContent();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            var time = ControllerContext.Scene.SceneSettings.Time;

            var transform = Matrix.CreateFromAxisAngle(sunRotationAxis, MathHelper.TwoPi * time);
            var newSunDirection = Vector3.Transform(MidnightSunDirection, transform);
            newSunDirection.Normalize();
            Scene.SceneSettings.DirectionalLight0.Direction = -newSunDirection;

            Vector3 newSunColor;
            Vector3 newFogColor;
            Vector3 newShadowColor;
            float timeInDay = time - (float) Math.Floor(time);

            // MEMO: [0, 1] で時間を正規化した場合
            // 0.00:  0 時
            // 0.25:  6 時
            // 0.50: 12 時
            // 0.75: 18 時
            // 1.00: 24 時

            bool shadowEnabled = true;
            if (timeInDay > 0.25f && timeInDay <= 0.75f)
            {
                // 昼の場合:
                // 12 時 (0.5f) で MiddaySunColor になるように線形補間。

                float amount;
                if (timeInDay <= 0.5f)
                {
                    // 明け方から正午
                    amount = 0.5f - timeInDay;
                }
                else
                {
                    // 正午から夕暮れ
                    amount = timeInDay - 0.5f;
                }

                newSunColor = MiddaySunColor + amount * (MidnightSunColor - MiddaySunColor);
                newFogColor = MiddayFogColor + amount * (MidnightFogColor - MiddayFogColor);
                newShadowColor = MiddayShadowColor + amount * (MidnightShadowColor - MiddayShadowColor);
            }
            else
            {
                // 夜の場合:
                // 0 時 (0.0f と 1.0f) で MidnightSunColor になるように線形補間。

                float amount;
                if (timeInDay <= 0.25f)
                {
                    // 深夜から明け方
                    amount = timeInDay;
                }
                else
                {
                    // 夕暮れから深夜
                    amount = 1.0f - timeInDay;
                }

                newSunColor = MidnightSunColor + amount * (MiddaySunColor - MidnightSunColor);
                newFogColor = MidnightFogColor + amount * (MiddayFogColor - MidnightFogColor);
                newShadowColor = MidnightShadowColor + amount * (MiddayShadowColor - MidnightShadowColor);

                // 夜はシャドウ マッピングを OFF にする。
                shadowEnabled = false;
                newShadowColor = newSunColor;
            }

            Scene.SceneSettings.DirectionalLight0.DiffuseColor = newSunColor;
            Scene.SceneSettings.DirectionalLight0.ShadowColor = newShadowColor;
            Scene.SceneSettings.DirectionalLight0.ShadowEnabled = shadowEnabled;
            Scene.SceneSettings.Fog.Color = newFogColor;
        }

        #endregion
    }
}
