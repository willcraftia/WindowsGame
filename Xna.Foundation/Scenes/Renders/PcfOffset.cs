#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public static class PcfOffset
    {
        public static void CreateOffsets3x3(int shadowMapSize, out Vector2[] result)
        {
            var shadowMapScale = CalculateShadowMapScale(shadowMapSize);
            result = new Vector2[9];

            int index = 0;
            for (float y = -1.5f; y <= 1.5f; y += 1.5f)
            {
                for (float x = -1.5f; x <= 1.5f; x += 1.5f)
                {
                    result[index++] = new Vector2(x, y) * shadowMapScale;
                }
            }
        }

        public static void CreateOffsets4x4(int shadowMapSize, out Vector2[] result)
        {
            var shadowMapScale = CalculateShadowMapScale(shadowMapSize);
            result = new Vector2[16];

            int index = 0;
            for (float y = -1.5f; y <= 1.5f; y += 1.0f)
            {
                for (float x = -1.5f; x <= 1.5f; x += 1.0f)
                {
                    result[index++] = new Vector2(x, y) * shadowMapScale;
                }
            }
        }

        public static void CreateOffsets5x5(int shadowMapSize, out Vector2[] result)
        {
            var shadowMapScale = CalculateShadowMapScale(shadowMapSize);
            result = new Vector2[25];

            int index = 0;
            //for (float y = -1.5f; y <= 1.5f; y += 0.75f)
            //{
            //    for (float x = -1.5f; x <= 1.5f; x += 0.75f)
            //    {
            //        result[index++] = new Vector2(x, y) * shadowMapScale;
            //    }
            //}
            for (int y = -2; y <= 2; y++)
            {
                for (int x = -2; x <= 2; x++)
                {
                    result[index++] = new Vector2(x, y) * shadowMapScale;
                }
            }
        }

        static Vector2 CalculateShadowMapScale(int shadowMapSize)
        {
            return new Vector2(1.0f / (float) shadowMapSize, 1.0f / (float) shadowMapSize);
        }
    }
}
