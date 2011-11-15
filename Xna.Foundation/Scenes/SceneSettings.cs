#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// シーン設定です。
    /// </summary>
    public sealed class SceneSettings
    {
        /// <summary>
        /// 時間。
        /// 1 日を [0, 1] (0 時が 0、24 時が 1) で表した累計。
        /// </summary>
        [DefaultValue(0)]
        public float Time;

        /// <summary>
        /// 現実時間に対するゲーム ワールド時間のスケール。
        /// </summary>
        /// <remarks>
        /// この時間スケールを現実時間へ乗算してゲーム ワールド時間を算出します。
        /// </remarks>
        [DefaultValue(0)]
        public float TimeScale;

        /// <summary>
        /// グローバル アンビエント色。
        /// </summary>
        [DefaultValue(typeof(Vector3), "0, 0, 0")]
        public Vector3 GlobalAmbientColor = Vector3.Zero;

        /// <summary>
        /// ディレクショナル ライト設定 #1。
        /// </summary>
        public DirectionalLight DirectionalLight0 = DirectionalLight.None;

        /// <summary>
        /// ディレクショナル ライト設定 #2。
        /// </summary>
        public DirectionalLight DirectionalLight1 = DirectionalLight.None;

        /// <summary>
        /// ディレクショナル ライト設定 #3。
        /// </summary>
        public DirectionalLight DirectionalLight2 = DirectionalLight.None;
        
        /// <summary>
        /// 線形フォグ設定。
        /// </summary>
        public LinearFog Fog = LinearFog.Default;

        ShadowSettings shadowSettings = new ShadowSettings();

        /// <summary>
        /// 影設定。
        /// </summary>
        public ShadowSettings ShadowSettings
        {
            get { return shadowSettings; }
        }
    }
}
