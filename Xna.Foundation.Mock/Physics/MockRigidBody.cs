#region Using

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Mock.Physics
{
    public class MockRigidBody : IRigidBody
    {
        public Vector3 Position;
        public Matrix Orientation;
        public Vector3 Veclocity;
        public Vector3 Gravity;

        #region IRigidBody

        public virtual bool Enabled { get; set; }
        public virtual bool AutoDisabled { get; set; }
        public virtual bool Immovable { get; set; }
        public virtual ICollisionBounds CollisionBounds { get; set; }
        public virtual Matrix InertiaTensor { get; set; }
        public virtual float Mass { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual IList<IExternalForce> ExternalForces { get; set; }

        public virtual void Activate() { }

        public virtual void Deactivate() { }

        public virtual void GetPosition(out Vector3 result)
        {
            result = Position;
        }

        public virtual Vector3 GetPosition()
        {
            return Position;
        }

        public virtual void UpdatePosition(ref Vector3 position)
        {
            Position = position;
        }

        public virtual void UpdatePosition(Vector3 position)
        {
            Position = position;
        }

        public virtual void GetOrientation(out Matrix result)
        {
            result = Matrix.Identity;
        }

        public virtual Matrix GetOrientation()
        {
            return Matrix.Identity;
        }

        public virtual void UpdateOrientation(ref Matrix orientation)
        {
            Orientation = orientation;
        }

        public virtual void UpdateOrientation(Matrix orientation)
        {
            Orientation = orientation;
        }

        public virtual void GetTransform(out Matrix result)
        {
            result = Matrix.CreateTranslation(Position) * Orientation;
        }

        public virtual void CalculateMassProperties(float density) { }

        public virtual void ApplyForce(ref Vector3 force) { }

        public virtual void ApplyForce(Vector3 force) { }

        public virtual void GetVeclocity(out Vector3 result)
        {
            result = Veclocity;
        }

        public virtual void GetGravity(out Vector3 result)
        {
            result = Gravity;
        }

        public virtual void UpdateGravity(ref Vector3 value)
        {
            Gravity = value;
        }

        #endregion

        public MockRigidBody()
        {
            CollisionBounds = new MockCollisionBounds();
            ExternalForces = new List<IExternalForce>();
        }
    }
}
