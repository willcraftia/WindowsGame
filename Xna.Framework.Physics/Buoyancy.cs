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

namespace Willcraftia.Xna.Framework.Physics
{
    public sealed class Buoyancy : IExternalForce
    {
        #region Fields and Properties

        public float Density = 1.0f;

        // TODO: テスト値。
        public Vector3 Velocity = Vector3.Right * 10.0f;

        // TODO: テスト値。
        public float DragCoefficient = 0.05f;

        #endregion

        #region IExternalForce

        public void ApplyExternalForce(IRigidBody rigidBody, float timeStep)
        {
            Vector3 currentGravity;
            rigidBody.GetGravity(out currentGravity);

            Vector3 buoyancy;
            Vector3.Negate(ref currentGravity, out buoyancy);

            var volume = rigidBody.CollisionBounds.Volume;
            buoyancy *= (Density * volume);

            rigidBody.ApplyForce(ref buoyancy);

            // 抗力 (D: Drag)
            //
            // D = (1 / 2) * P * V^2 * S * Cd
            //
            // P: 流体の密度 (浮力で用いる流体の密度と同じなので、このクラスでは Density)
            // V: 物体と流体の相対速度
            // S: 物体の代表面積 (ここでは簡易にするために衝突形状の全体の面積で代用)
            // Cd: 抗力係数 (算出が大変なようなので、このクラスでは任意の設定値として調整)

            Vector3 rigidBodyVelocity;
            rigidBody.GetVeclocity(out rigidBodyVelocity);
            
            // 相対速度
            //var relativeVelocity = Velocity - rigidBodyVelocity;
            var relativeVelocity = rigidBodyVelocity - Velocity;

            // 相対速度の二乗 (速度ベクトルの大きさの二乗)
            var v2 = relativeVelocity.LengthSquared();

            // 抗力
            var drag = Density * v2 * rigidBody.CollisionBounds.SurfaceArea * DragCoefficient;
            
            // 抗力の方向 (単位ベクトル)
            var direction = Velocity;
            if (direction.LengthSquared() != 0)
            {
                direction.Normalize();
            }
            var dragVector = direction * drag;

            rigidBody.ApplyForce(ref dragVector);
        }

        #endregion
    }
}
