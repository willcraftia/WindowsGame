#region Using

using JigLibX.Collision;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// JigLibX 環境で用いる定数を定義するクラスです。
    /// </summary>
    public static class JigLibXConstants
    {
        /// <summary>
        /// 静的摩擦のデフォルト値。
        /// </summary>
        public const float DefaultStaticFriction = 0.5f;
        
        /// <summary>
        /// 動的摩擦のデフォルト値。
        /// </summary>
        public const float DefaultDynamicFriction = 0.5f;

        /// <summary>
        /// 弾性のデフォルト値。
        /// </summary>
        public const float DefaultRestitution = 0.5f;

        /// <summary>
        /// デフォルト値で構成された MaterialProperties。
        /// </summary>
        public static MaterialProperties DefaultMaterialProperties
        {
            get
            {
                return new MaterialProperties()
                {
                    StaticRoughness = DefaultStaticFriction,
                    DynamicRoughness = DefaultDynamicFriction,
                    Elasticity = DefaultRestitution
                };
            }
        }
    }
}
