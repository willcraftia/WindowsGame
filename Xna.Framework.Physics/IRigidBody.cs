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
    /// 剛体のインタフェースです。
    /// </summary>
    public interface IRigidBody
    {
        /// <summary>
        /// 剛体が物理システム内で有効かどうかを示す値。
        /// </summary>
        /// <value>
        /// true (剛体が物理システム内で有効)、false (それ以外の場合)。
        /// </value>
        /// <remarks>
        /// このプロパティを true にすると剛体が現在の物理システムに追加され、
        /// インテグレーション処理に含まれます。
        /// 一方、false にすると、現在の物理システムに追加されない、あるいは、
        /// 既に現在の物理システムに追加されている場合にはそこから削除され、
        /// インテグレーション処理から外れます。
        /// </remarks>
        bool Enabled { get; set; }

        /// <summary>
        /// 静止状態に入った場合に、一時的にインテグレーション処理から自動的に除外するかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (自動的に除外する場合)、false (それ以外の場合)。
        /// </value>
        /// <remarks>
        /// 剛体が静止状態に入った場合、外力が剛体に働かない限り、インテグレーション処理に含まれる必要がありません。
        /// つまり、静止状態の剛体を一時的にインテグレーション処理から除外することで、
        /// インテグレーション処理にかかる負荷を下げることができます。
        /// </remarks>
        bool AutoDisabled { get; set; }
        
        /// <summary>
        /// 運動を行わないかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (運動を行わない場合)、false (それ以外の場合)。
        /// </value>
        /// <remarks>
        /// 運動を行わない剛体は、インテグレーション処理で運動が制御されず、座標ベクトルや回転行列が更新されません。
        /// つまり、運動を行わない剛体は常に静止状態となります。
        /// </remarks>
        bool Immovable { get; set; }

        /// <summary>
        /// 剛体の ICollisionSkin。
        /// </summary>
        ICollisionBounds CollisionBounds { get; }

        /// <summary>
        /// 慣性テンソル。
        /// </summary>
        Matrix InertiaTensor { get; set; }

        /// <summary>
        /// 質量。
        /// </summary>
        float Mass { get; set; }

        /// <summary>
        /// 剛体がインテグレーション処理に含まれているかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (インテグレーション処理に含まれている場合)、false (それ以外の場合)。
        /// </value>
        bool IsActive { get; }

        /// <summary>
        /// IExternalForce のリスト。
        /// </summary>
        IList<IExternalForce> ExternalForces { get; }

        /// <summary>
        /// インテグレーション処理から一時的に除外されている剛体を、強制的にインテグレーション処理へ含めます。
        /// </summary>
        void Activate();

        /// <summary>
        /// インテグレーション処理に含まれている剛体を、強制的にインテグレーション処理から一時的に除外します。
        /// </summary>
        void Deactivate();

        /// <summary>
        /// 座標ベクトルを取得します。
        /// </summary>
        /// <param name="result">座標ベクトル。</param>
        void GetPosition(out Vector3 result);

        /// <summary>
        /// 座標ベクトルを取得します。
        /// </summary>
        /// <returns>座標ベクトル。</returns>
        Vector3 GetPosition();
        
        /// <summary>
        /// 座標ベクトルを設定します。
        /// </summary>
        /// <param name="position">座標ベクトル。</param>
        /// <remarks>
        /// インテグレーション処理に含まれている剛体は、
        /// インテグレーション処理が決定する剛体の運動に従って座標ベクトルが更新されます。
        /// 剛体の初期座標ベクトルを設定したい場合などの、
        /// インテグレーション処理によらずに座標ベクトルを更新したい場合を除いて、
        /// このメソッドで座標ベクトルを設定するべきではありません。
        /// </remarks>
        void UpdatePosition(ref Vector3 position);

        /// <summary>
        /// 座標ベクトルを設定します。
        /// </summary>
        /// <param name="position">座標ベクトル。</param>
        /// <remarks>
        /// インテグレーション処理に含まれている剛体は、
        /// インテグレーション処理が決定する剛体の運動に従って座標ベクトルが更新されます。
        /// 剛体の初期座標ベクトルを設定したい場合などの、
        /// インテグレーション処理によらずに座標ベクトルを更新したい場合を除いて、
        /// このメソッドで座標ベクトルを設定するべきではありません。
        /// </remarks>
        void UpdatePosition(Vector3 position);

        /// <summary>
        /// 姿勢行列を取得します。
        /// </summary>
        /// <param name="result">姿勢行列。</param>
        void GetOrientation(out Matrix result);
        
        /// <summary>
        /// 姿勢行列を取得します。
        /// </summary>
        /// <returns>姿勢行列。</returns>
        Matrix GetOrientation();

        /// <summary>
        /// 姿勢行列を設定します。
        /// </summary>
        /// <param name="orientation">姿勢行列。</param>
        /// <remarks>
        /// インテグレーション処理に含まれている剛体は、
        /// インテグレーション処理が決定する剛体の運動に従って姿勢行列が更新されます。
        /// 剛体の初期姿勢行列を設定したい場合などの、
        /// インテグレーション処理によらずに剛体の姿勢行列を更新したい場合を除いて、
        /// このメソッドで姿勢行列を設定するべきではありません。
        /// </remarks>
        void UpdateOrientation(ref Matrix orientation);
        
        /// <summary>
        /// 姿勢行列を設定します。
        /// </summary>
        /// <param name="orientation">姿勢行列。</param>
        /// <remarks>
        /// インテグレーション処理に含まれている剛体は、
        /// インテグレーション処理が決定する剛体の運動に従って姿勢行列が更新されます。
        /// 剛体の初期姿勢行列を設定したい場合などの、
        /// インテグレーション処理によらずに剛体の姿勢行列を更新したい場合を除いて、
        /// このメソッドで姿勢行列を設定するべきではありません。
        /// </remarks>
        void UpdateOrientation(Matrix orientation);

        /// <summary>
        /// 変換行列を取得します。
        /// </summary>
        /// <param name="result"></param>
        void GetTransform(out Matrix result);

        /// <summary>
        /// 質量プロパティを計算します。
        /// </summary>
        /// <param name="density">密度。</param>
        void CalculateMassProperties(float density);

        /// <summary>
        /// 力ベクトルを剛体に作用させます。
        /// </summary>
        /// <param name="force">力ベクトル。</param>
        /// <remarks>
        /// このメソッドは、物理システムのインテグレーション処理に含まれる実装でのみ利用できます。
        /// インテグレーション処理の外でこのメソッドを呼び出しても、剛体に力を作用させることはできません。
        /// </remarks>
        void ApplyForce(ref Vector3 force);

        /// <summary>
        /// 力ベクトルを剛体に作用させます。
        /// </summary>
        /// <param name="force">力ベクトル。</param>
        /// <remarks>
        /// このメソッドは、物理システムのインテグレーション処理に含まれる実装でのみ利用できます。
        /// インテグレーション処理の外でこのメソッドを呼び出しても、剛体に力を作用させることはできません。
        /// </remarks>
        void ApplyForce(Vector3 force);

        /// <summary>
        /// 速度ベクトルを取得します。
        /// </summary>
        /// <param name="result"></param>
        void GetVeclocity(out Vector3 result);

        /// <summary>
        /// 重力加速度を取得します。
        /// </summary>
        /// <param name="result">重力加速度。</param>
        void GetGravity(out Vector3 result);

        /// <summary>
        /// 重力加速度を設定します。
        /// </summary>
        /// <param name="value">重力加速度。</param>
        void UpdateGravity(ref Vector3 value);
    }
}
