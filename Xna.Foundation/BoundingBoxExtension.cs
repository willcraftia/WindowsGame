#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation
{
    /// <summary>
    /// BoundingBox の拡張です。
    /// </summary>
    public static class BoundingBoxExtension
    {
        #region Utility

        public static BoundingBox Empty
        {
            get { return new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue)); }
        }

        public static BoundingBox Infinite
        {
            get { return new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue)); }
        }

        #endregion

        #region Extension

        public static Vector3 GetSideLengths(this BoundingBox entity)
        {
            Vector3 result;
            GetSideLengths(entity, out result);
            return result;
        }

        public static void GetSideLengths(this BoundingBox entity, out Vector3 result)
        {
            result = entity.Max - entity.Min;
        }

        public static Vector3 GetCenter(this BoundingBox entity)
        {
            Vector3 result;
            GetCenter(entity, out result);
            return result;
        }

        public static void GetCenter(this BoundingBox entity, out Vector3 result)
        {
            result = entity.Min + (entity.Max - entity.Min) * 0.5f;
        }

        public static Vector3 GetEntents(this BoundingBox entity)
        {
            Vector3 result;
            GetEntents(entity, out result);
            return result;
        }

        public static void GetEntents(this BoundingBox entity, out Vector3 result)
        {
            result = (entity.Max - entity.Min) * 0.5f;
        }

        #endregion
    }
}
