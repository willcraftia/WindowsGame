#region Using

using JigLibX.Collision;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// JigLibX 環境での ICollisionBoundsFactory 実装クラスです。
    /// </summary>
    public sealed class JigLibXCollisionBoundsFactory : ICollisionBoundsFactory
    {
        CollisionCallbackFn collisionCallbackFn;

        public JigLibXCollisionBoundsFactory(CollisionCallbackFn collisionCallbackFn)
        {
            this.collisionCallbackFn = collisionCallbackFn;
        }

        #region ICollisionBoundsFactory

        public ICollisionBounds CreateCollisionBounds()
        {
            var instance = new JigLibXCollisionBounds();
            instance.callbackFn += collisionCallbackFn;
            return instance;
        }

        #endregion
    }
}
