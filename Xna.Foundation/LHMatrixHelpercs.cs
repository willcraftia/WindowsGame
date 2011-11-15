#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation
{
    public static class LHMatrixHelpercs
    {

        public static void CreateOrthographicLH(
            float width,
            float height,
            float zNearPlane,
            float zFarPlane,
            out Matrix result)
        {
            result.M11 = 2.0f / width;
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;

            result.M21 = 0;
            result.M22 = 2.0f / height;
            result.M23 = 0;
            result.M24 = 0;

            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1.0f / (zFarPlane - zNearPlane);
            result.M34 = 0;

            result.M41 = 0;
            result.M42 = 0;
            result.M43 = zNearPlane / (zNearPlane - zFarPlane);
            result.M44 = 1.0f;
        }

        public static void CreateCreateOrthographicOffCenterLH(
            float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, out Matrix result)
        {
            result.M11 = 2.0f / (right - left);
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;

            result.M21 = 0;
            result.M22 = 2.0f / (top - bottom);
            result.M23 = 0;
            result.M24 = 0;

            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1.0f / (zFarPlane - zNearPlane);
            result.M34 = 0;

            result.M41 = (left + right) / (left - right);
            result.M42 = (top + bottom) / (bottom - top);
            result.M43 = zNearPlane / (zNearPlane - zFarPlane);
            result.M44 = 1.0f;
        }

        public static void CreateLookAtLH(ref Vector3 position, ref Vector3 target, ref Vector3 up, out Matrix result)
        {
            Vector3 zaxis = position - target;
            zaxis.Normalize();

            Vector3 xaxis;
            Vector3.Cross(ref zaxis, ref up, out xaxis);
            xaxis.Normalize();

            Vector3 yaxis;
            Vector3.Cross(ref xaxis, ref zaxis, out yaxis);
            yaxis.Normalize();

            result.M11 = xaxis.X;
            result.M12 = yaxis.X;
            result.M13 = zaxis.X;
            result.M14 = 0;

            result.M21 = xaxis.Y;
            result.M22 = yaxis.Y;
            result.M23 = zaxis.Y;
            result.M24 = 0;

            result.M31 = xaxis.Z;
            result.M32 = yaxis.Z;
            result.M33 = zaxis.Z;
            result.M34 = 0;

            result.M41 = -Vector3.Dot(xaxis, position);
            result.M42 = -Vector3.Dot(yaxis, position);
            result.M43 = -Vector3.Dot(zaxis, position);
            result.M44 = 1;
        }
    }
}
