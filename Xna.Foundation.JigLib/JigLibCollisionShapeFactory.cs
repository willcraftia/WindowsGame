#region Using

using System;
using System.Collections.Generic;
using System.Reflection;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    public sealed class JigLibXCollisionShapeFactory : ICollisionShapeFactory
    {
        Dictionary<Type, ConstructorInfo> implementations = new Dictionary<Type, ConstructorInfo>();
        public Dictionary<Type, ConstructorInfo> Implementations
        {
            get { return implementations; }
        }

        #region ICollisionShapeFactory

        public ICollisionShape CreateCollisionShape(Type type)
        {
            ConstructorInfo constructor;
            if (!implementations.TryGetValue(type, out constructor))
            {
                throw new InvalidOperationException(string.Format("Type '{0}' is not registered.", type));
            }

            return constructor.Invoke(null) as ICollisionShape;
        }

        #endregion
    }
}
