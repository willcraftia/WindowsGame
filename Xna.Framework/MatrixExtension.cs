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

#endregion

namespace Willcraftia.Xna.Framework
{
    /// <summary>
    /// Matrix の拡張です。
    /// </summary>
    public static class MatrixExtension
    {
        /// <summary>
        /// Yaw を取得します。
        /// </summary>
        /// <param name="matrix">Matrix。</param>
        /// <returns>Yaw。</returns>
        public static float GetYaw(this Matrix matrix)
        {
            float result;
            GetYaw(matrix, out result);
            return result;
        }

        /// <summary>
        /// Yaw を取得します。
        /// </summary>
        /// <param name="matrix">Matrix。</param>
        /// <param name="yaw">Yaw。</param>
        public static void GetYaw(this Matrix matrix, out float yaw)
        {
            // Singularity at north pole.
            if (matrix.M12 > 0.998f)
            {
                yaw = (float) Math.Atan2(matrix.M31, matrix.M33);
                return;
            }

            // Singularity at south pole.
            if (matrix.M12 < -0.998f)
            {
                yaw = (float) Math.Atan2(matrix.M31, matrix.M33);
                return;
            }
            yaw = (float) Math.Atan2(-matrix.M13, matrix.M11);
        }

        /// <summary>
        /// Pitch を取得します。
        /// </summary>
        /// <param name="matrix">Matrix。</param>
        /// <returns>Pitch。</returns>
        public static float GetPitch(this Matrix matrix)
        {
            float result;
            GetPitch(matrix, out result);
            return result;
        }

        /// <summary>
        /// Pitch を取得します。
        /// </summary>
        /// <param name="matrix">Matrix。</param>
        /// <param name="pitch">Pitch。</param>
        public static void GetPitch(this Matrix matrix, out float pitch)
        {
            // Singularity at north pole.
            if (matrix.M12 > 0.998f)
            {
                pitch = 0.0f;
                return;
            }

            // Singularity at south pole.
            if (matrix.M12 < -0.998f)
            {
                pitch = 0.0f;
                return;
            }
            pitch = (float) Math.Atan2(-matrix.M32, matrix.M22);
        }

        /// <summary>
        /// Roll を取得します。
        /// </summary>
        /// <param name="matrix">Matrix。</param>
        /// <returns>Roll。</returns>
        public static float GetRoll(this Matrix matrix)
        {
            float result;
            GetRoll(matrix, out result);
            return result;
        }

        /// <summary>
        /// Roll を取得します。
        /// </summary>
        /// <param name="matrix">Matrix。</param>
        /// <param name="roll">Roll。</param>
        public static void GetRoll(this Matrix matrix, out float roll)
        {
            // Singularity at north pole.
            if (matrix.M12 > 0.998f)
            {
                roll = MathHelper.PiOver2;
                return;
            }

            // Singularity at south pole.
            if (matrix.M12 < -0.998f)
            {
                roll = -MathHelper.PiOver2;
                return;
            }
            roll = (float) Math.Asin(matrix.M12);
        }

        /// <summary>
        /// Yaw、Pitch、Roll を取得します。
        /// </summary>
        /// <param name="matrix">Matrix。</param>
        /// <param name="yaw">Yaw。</param>
        /// <param name="pitch">Pitch。</param>
        /// <param name="roll">Roll。</param>
        public static void ToYawPitchRoll(this Matrix matrix, out float yaw, out float pitch, out float roll)
        {
            // Singularity at north pole.
            if (matrix.M12 > 0.998f)
            {
                yaw = (float) Math.Atan2(matrix.M31, matrix.M33);
                pitch = 0.0f;
                roll = MathHelper.PiOver2;

                return;
            }

            // Singularity at south pole.
            if (matrix.M12 < -0.998f)
            {
                yaw = (float) Math.Atan2(matrix.M31, matrix.M33);
                pitch = 0.0f;
                roll = -MathHelper.PiOver2;

                return;
            }

            yaw = (float) Math.Atan2(-matrix.M13, matrix.M11);
            pitch = (float) Math.Atan2(-matrix.M32, matrix.M22);
            roll = (float) Math.Asin(matrix.M12);
        }

        /// <summary>
        /// Yaw、Pitch、Roll を Vector3 として取得します。
        /// </summary>
        /// <param name="matrix">Matrix。</param>
        /// <returns>X 成分に Pitch、Y 成分に Yaw、Z 成分に Roll を持つ Vector3。</returns>
        public static Vector3 ToYawPitchRoll(this Matrix matrix)
        {
            Vector3 result;
            ToYawPitchRoll(matrix, out result);
            return result;
        }

        /// <summary>
        /// Yaw、Pitch、Roll を Vector3 として取得します。
        /// </summary>
        /// <param name="matrix">Matrix。</param>
        /// <param name="result">X 成分に Pitch、Y 成分に Yaw、Z 成分に Roll を持つ Vector3。</param>
        public static void ToYawPitchRoll(this Matrix matrix, out Vector3 result)
        {
            result = new Vector3();
            ToYawPitchRoll(matrix, out result.Y, out result.X, out result.Z);
        }
    }
}
