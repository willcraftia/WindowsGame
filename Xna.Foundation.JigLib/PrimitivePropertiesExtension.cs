#region Using

using JigLibX.Geometry;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// PrimitiveProperties の拡張です。
    /// </summary>
    public static class PrimitivePropertiesExtension
    {
        #region Utility

        public static PrimitiveProperties SolidDensity
        {
            get
            {
                return new PrimitiveProperties
                {
                    MassDistribution = PrimitiveProperties.MassDistributionEnum.Solid,
                    MassType = PrimitiveProperties.MassTypeEnum.Density,
                    MassOrDensity = 0.0f
                };
            }
        }

        #endregion
    }
}
