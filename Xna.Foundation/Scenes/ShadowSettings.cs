#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// 影設定です。
    /// </summary>
    public sealed class ShadowSettings
    {
        #region Inner classes

        /// <summary>
        /// VSM (Variant Shadow Map) 設定です。
        /// </summary>
        public struct VsmSettings
        {
            #region Fields

            /// <summary>
            /// ブラーが有効かどうかを示します。
            /// true (ブラーが有効な場合)、false (それ以外の場合)。
            /// </summary>
            public bool BlurEnabled;

            /// <summary>
            /// ブラー適用半径です。
            /// </summary>
            public int BlurRadius;

            /// <summary>
            /// ブラー適用量です。
            /// </summary>
            public float BlurAmount;

            #endregion

            #region Predefined

            /// <summary>
            /// デフォルトの VSM 設定です。
            /// </summary>
            public static VsmSettings Default
            {
                get
                {
                    return new VsmSettings()
                    {
                        BlurEnabled = true,
                        BlurRadius = 1,
                        BlurAmount = 1.0f
                    };
                }
            }

            #endregion
        }

        /// <summary>
        /// PCF (Percentage Closer Filtering) 設定です。
        /// </summary>
        public struct PcfSettings
        {
            #region Fields

            /// <summary>
            /// カーネル サイズです。
            /// </summary>
            public int KernelSize;

            #endregion

            #region Predefined

            /// <summary>
            /// デフォルトの PCF 設定です。
            /// </summary>
            public static PcfSettings Default
            {
                get
                {
                    return new PcfSettings()
                    {
                        KernelSize = 2,
                    };
                }
            }

            #endregion
        }

        /// <summary>
        /// Screen Space Shadow 設定です。
        /// </summary>
        public struct ScreenSpaceShadowSettings
        {
            #region Fields

            /// <summary>
            /// マップのスケールです。
            /// </summary>
            public float MapScale;

            /// <summary>
            /// ブラーが有効かどうかを示します。
            /// true (ブラーが有効な場合)、false (それ以外の場合)。
            /// </summary>
            public bool BlurEnabled;

            /// <summary>
            /// ブラー適用半径です。
            /// </summary>
            public int BlurRadius;

            /// <summary>
            /// ブラー適用量です。
            /// </summary>
            public float BlurAmount;

            #endregion

            #region Predefined

            /// <summary>
            /// デフォルトの Screen Space Shadow 設定です。
            /// </summary>
            public static ScreenSpaceShadowSettings Default
            {
                get
                {
                    return new ScreenSpaceShadowSettings()
                    {
                        MapScale = 0.25f,
                        BlurEnabled  = false,
                        BlurRadius = 1,
                        BlurAmount = 1.0f
                    };
                }
            }

            #endregion
        }

        /// <summary>
        /// PSSM (Paralle Split Shadow Maps) 設定です。
        /// </summary>
        public struct PssmSettings
        {
            #region Fields

            /// <summary>
            /// 分割数です。
            /// </summary>
            public int SplitCount;

            /// <summary>
            /// 分割ラムダ値です。
            /// </summary>
            public float SplitLambda;

            #endregion

            #region Predefined

            /// <summary>
            /// デフォルトの PSSM 設定です。
            /// </summary>
            public static PssmSettings Default
            {
                get
                {
                    return new PssmSettings()
                    {
                        SplitCount = 3,
                        SplitLambda = 0.7f
                    };
                }
            }

            #endregion
        }

        /// <summary>
        /// LSPSM (Light Space Perspective Shadow Maps) 設定です。
        /// </summary>
        public struct LspsmSettings
        {
            #region Fields

            /// <summary>
            /// LSPSM の新しい式が有効かどうかを示します。
            /// true (新しい式が有効な場合)、false (それ以外の場合)。
            /// </summary>
            public bool NewNFormulaEnabled;

            /// <summary>
            /// 明示した N 値を使用するかどうかを示します。
            /// true (明示した N 値を使用する場合)、false (それ以外の場合)。
            /// </summary>
            public bool ExplicitNEnabled;

            /// <summary>
            /// 明示する N 値です。
            /// </summary>
            public float N;

            #endregion

            #region Predefined

            /// <summary>
            /// デフォルトの LSPSM 設定です。
            /// </summary>
            public static LspsmSettings Default
            {
                get
                {
                    return new LspsmSettings()
                    {
                        NewNFormulaEnabled = true,
                        ExplicitNEnabled = false,
                        N = 10.0f
                    };
                }
            }

            #endregion
        }

        #endregion

        #region Fields and Properties

        #region Const [Shadow map]

        public const int DefaultShadowMapSize = 512;
        public const SurfaceFormat DefaultShadowMapFormat = SurfaceFormat.Vector2;
        public const ShadowTest DefaultShadowTest = ShadowTest.Vsm;
        public const float DefaultShadowNearPlaneDistance = 0.1f;
        public const float DefaultShadowFarPlaneDistance = 500.0f;
        
        #endregion

        #region Const [Shadow scene map]
        
        public const float DefaultDepthBias = 0.005f;
        
        #endregion

        #region Const [Light frustum]

        public const LightFrustumShape DefaultLightFrustumShape = LightFrustumShape.Pssm;
        public const float DefaultBackwardLightVolumeRadius = 10.0f;
        
        #endregion

        /// <summary>
        /// 影が有効かどうかを示します。
        /// </summary>
        public bool Enabled = false;

        /// <summary>
        /// Shadow Mapping 検査の種類です。
        /// </summary>
        public ShadowTest Test = DefaultShadowTest;

        /// <summary>
        /// Shadow Map のサイズです。
        /// </summary>
        public int Size = DefaultShadowMapSize;

        /// <summary>
        /// Shadow Map の SurfaceFormat です。
        /// </summary>
        public SurfaceFormat Format = DefaultShadowMapFormat;

        /// <summary>
        /// Shadow Map の作成で行われる影の判定で使用する深度バイアスです。
        /// </summary>
        public float DepthBias = DefaultDepthBias;

        /// <summary>
        /// Shadow Mapping で使用するライト カメラの y 方向の視野角 (ラジアン単位) です。
        /// </summary>
        public float Fov = MathHelper.PiOver4;

        /// <summary>
        /// Shadow Mapping で使用するライト カメラのアスペクト比です。
        /// </summary>
        public float AspectRatio = 1.0f;

        /// <summary>
        /// Shadow Mapping で使用するライト カメラの、近くのビュー プレーンとの距離。
        /// </summary>
        public float NearPlaneDistance = DefaultShadowNearPlaneDistance;

        /// <summary>
        /// Shadow Mapping で使用するライト カメラの、遠くのビュー プレーンとの距離。
        /// </summary>
        public float FarPlaneDistance = DefaultShadowFarPlaneDistance;

        /// <summary>
        /// 影の判定に含めるカメラ後方領域の半径です。
        /// </summary>
        public float BackwardLightVolumeRadius = DefaultBackwardLightVolumeRadius;

        /// <summary>
        /// Shadow Mapping で使用するライト カメラの視錐台の形状です。
        /// </summary>
        public LightFrustumShape Shape = DefaultLightFrustumShape;

        /// <summary>
        /// VSM (Variant Shadow Mapping) 設定です。
        /// </summary>
        public VsmSettings Vsm = VsmSettings.Default;

        /// <summary>
        /// PCF (Percentage Closer Filtering) 設定です。
        /// </summary>
        public PcfSettings Pcf = PcfSettings.Default;

        /// <summary>
        /// Screen Space Shadow Mapping 設定です。
        /// </summary>
        public ScreenSpaceShadowSettings ScreenSpaceShadow = ScreenSpaceShadowSettings.Default;

        /// <summary>
        /// PSSM (Parallel Split Shadow Maps) 設定です。
        /// </summary>
        public PssmSettings Pssm = PssmSettings.Default;

        /// <summary>
        /// Light Space Perspective Shadow Maps) 設定です。
        /// </summary>
        public LspsmSettings Lspsm = LspsmSettings.Default;

        #endregion
    }
}
