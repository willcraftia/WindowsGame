#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Physics
{
    /// <summary>
    /// 衝突境界のインタフェースです。
    /// </summary>
    public interface ICollisionBounds
    {
        /// <summary>
        /// 衝突境界を持つ実体。
        /// </summary>
        object Entity { get; set; }

        /// <summary>
        /// 衝突判定が有効であるかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (衝突判定が有効である場合)、false (それ以外の場合)。
        /// </value>
        bool Enabled { get; set; }

        /// <summary>
        /// 体積。
        /// </summary>
        float Volume { get; }

        /// <summary>
        /// 表面面積。
        /// </summary>
        float SurfaceArea { get; }

        /// <summary>
        /// 他の物体と衝突したかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (他の物体と衝突した場合)、false (それ以外の場合)。
        /// </value>
        bool IsCollided { get; }

        //
        // TODO:
        // 複数の衝突形状を追加するということは、個々の形状が異なる座標を基準にするということ。
        // 現実装ではインタフェース レベルでこれに対応できていない。
        //

        /// <summary>
        /// ICollisionShape を追加します。
        /// </summary>
        /// <param name="collisionShape">ICollisionShape。</param>
        void AddCollisionShape(ICollisionShape collisionShape);

        /// <summary>
        /// 指定された方向について他の物体と衝突したかどうかを判定します。
        /// </summary>
        /// <param name="direction">方向。</param>
        /// <returns>true (他の物体と衝突した場合)、false (それ以外の場合)。</returns>
        bool IsCollidedForDirection(ref Vector3 direction);

        /// <summary>
        /// 線分が交差するかどうかを判定します。
        /// </summary>
        /// <param name="position">線分の座標ベクトル。</param>
        /// <param name="vector">線分の方向ベクトル。</param>
        /// <param name="intersectPosition">交点。</param>
        /// <param name="intersectNormal">交点での直線と衝突に対する法線ベクトル。</param>
        /// <param name="intersectFraction">交差している線分の比率。</param>
        /// <returns>true (交差する場合)、false (それ以外の場合)。</returns>
        bool Intersects(
            ref Vector3 position, ref Vector3 vector,
            out Vector3 intersectPosition, out Vector3 intersectNormal, out float intersectFraction);
        
        /// <summary>
        /// ICollisionShape の変換行列を更新します。
        /// </summary>
        /// <param name="position">座標ベクトル。</param>
        /// <param name="orientation">姿勢行列。</param>
        /// <remarks>
        /// このメソッドは、インテグレーション処理の開始前に、
        /// 各 ICollisionShape の初期変換行列を設定するために利用します。
        /// その後、インテグレーション処理が開始されると、
        /// ICollisionShape の変換行列はインテグレーション処理が更新します。
        /// </remarks>
        void UpdateCollisionShapeTransforms(ref Vector3 position, ref Matrix orientation);

        /// <summary>
        /// デバッグ用ワイヤフレームを描画するための頂点座標と色を取得します。
        /// </summary>
        /// <returns>ワイヤフレームを描画するための頂点座標と色の配列。</returns>
        VertexPositionColor[] GetDebugWireframe();
    }
}
