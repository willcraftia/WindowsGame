#region Using

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Math;
using JigLibX.Physics;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// JigLibX 環境での IRigidBody 実装です。
    /// </summary>
    public class JigLibXRigidBody : Body, IRigidBody
    {
        #region Fields and Properties

        // 質量中心。
        Vector3 centerOfMass = Vector3.Zero;

        // 慣性テンソル。
        Matrix inertiaTensor = new Matrix();

        // 重力加速度。
        Vector3 gravity = Vector3.Zero;

        #endregion

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        public JigLibXRigidBody()
        {
            CollisionSkin = new JigLibXCollisionBounds();
            CollisionSkin.Owner = this;
        }

        #region IRigidBody

        public bool Enabled
        {
            get { return IsBodyEnabled; }
            set
            {
                if (IsBodyEnabled != value)
                {
                    if (value)
                    {
                        EnableBody();
                    }
                    else
                    {
                        DisableBody();
                    }
                }
            }
        }

        public bool AutoDisabled
        {
            get { return AllowFreezing; }
            set { AllowFreezing = value; }
        }

        public ICollisionBounds CollisionBounds
        {
            get { return CollisionSkin as ICollisionBounds; }
        }

        public Matrix InertiaTensor
        {
            get { return inertiaTensor; }
            set
            {
                inertiaTensor = value;
                //
                // NOTE: Dont use JigLibX BodyInertia property if M11, M22, and/or M33 are zero.
                // It will set the max float value to an inverse inertia tensor.
                //
                if (value.M11 < JiggleMath.Epsilon ||
                    value.M22 < JiggleMath.Epsilon ||
                    value.M33 < JiggleMath.Epsilon)
                {
                    SetBodyInvInertia(value.M11, value.M22, value.M33);
                }
                else
                {
                    BodyInertia = value;
                }
            }
        }

        List<IExternalForce> externalForces = new List<IExternalForce>();
        public IList<IExternalForce> ExternalForces
        {
            get { return externalForces; }
        }

        public void Activate()
        {
            SetActive();
        }

        public void Deactivate()
        {
            SetInactive();
        }

        public void GetPosition(out Vector3 result)
        {
            result = Position;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

        public void UpdatePosition(ref Vector3 position)
        {
            MoveTo(position, Orientation);
        }

        public void UpdatePosition(Vector3 position)
        {
            MoveTo(position, Orientation);
        }

        public void GetOrientation(out Matrix result)
        {
            result = Orientation;
        }

        public Matrix GetOrientation()
        {
            return Orientation;
        }

        public void UpdateOrientation(ref Matrix orientation)
        {
            MoveTo(Position, orientation);
        }

        public void UpdateOrientation(Matrix orientation)
        {
            MoveTo(Position, orientation);
        }

        public void GetTransform(out Matrix result)
        {
            var transform = Transform;

            Matrix translation;
            Matrix.CreateTranslation(ref transform.Position, out translation);
            Matrix.Multiply(ref transform.Orientation, ref translation, out result);
        }

        public void CalculateMassProperties(float density)
        {
            var pp = PrimitivePropertiesExtension.SolidDensity;
            pp.MassOrDensity = density;

            float mass;
            Vector3 centerOfMass;
            Matrix inertiaTensor;
            Matrix inertiaTensorCoM;

            CollisionSkin.GetMassProperties(pp, out mass, out centerOfMass, out inertiaTensor, out inertiaTensorCoM);

            this.Mass = mass;
            this.centerOfMass = centerOfMass;
            //
            // TODO: intertiaTensor or inertiaTensorCoM ?
            //
            InertiaTensor = inertiaTensorCoM;
        }

        public void ApplyForce(ref Vector3 force)
        {
            AddWorldForce(force);
        }

        public void ApplyForce(Vector3 force)
        {
            AddWorldForce(force);
        }

        public void GetVeclocity(out Vector3 result)
        {
            result = Velocity;
        }

        public void GetGravity(out Vector3 result)
        {
            result = gravity;
        }

        public void UpdateGravity(ref Vector3 value)
        {
            gravity = value;
        }

        #endregion

        #region Control forces

        public override void AddExternalForces(float dt)
        {
            ClearForces();

            // 重力。
            var force = Mass * gravity;
            AddWorldForce(force);

            // 外力計算を IExternalForce へ移譲します。
            if (externalForces.Count != 0)
            {
                foreach (var externalForce in externalForces)
                {
                    externalForce.ApplyExternalForce(this, dt);
                }
            }
        }

        #endregion
    }
}
