#region Using

using System.Collections.Generic;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class CollisionBoundsConfig
    {
        List<CollisionShapeConfig> collisionShapeConfigs = new List<CollisionShapeConfig>();
        
        public List<CollisionShapeConfig> CollisionShapeConfigs
        {
            get { return collisionShapeConfigs; }
        }
    }
}
