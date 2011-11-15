#region Using

using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// JigLibX の Primitive の数値表現を管理するクラスです。
    /// </summary>
    public static class PrimitiveTypeManager
    {
        static int typeSequence = (int) PrimitiveType.NumTypes;
        static Dictionary<Type, int> typeDictionary = new Dictionary<Type, int>();

        public static int GetPrimitiveType<T>() where T : ICollisionShape
        {
            var type = typeof(T);

            lock (typeDictionary)
            {
                int primitiveType;
                if (typeDictionary.TryGetValue(type, out primitiveType))
                {
                    return primitiveType;
                }

                primitiveType = typeSequence++;
                typeDictionary[type] = primitiveType;

                return primitiveType;
            }
        }
    }
}
