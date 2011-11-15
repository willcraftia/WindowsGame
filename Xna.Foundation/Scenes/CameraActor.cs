#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Willcraftia.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// カメラを表す Actor です。
    /// </summary>
    public sealed class CameraActor : Actor
    {
        #region Events

        /// <summary>
        /// Active プロパティの変更で発生します。
        /// </summary>
        public event EventHandler<EventArgs> ActiveChanged;

        #endregion

        #region Fields and Properties

        Pov pov = new Pov();

        /// <summary>
        /// POV。
        /// </summary>
        /// <remarks>
        /// このプロパティへ座標ベクトルと姿勢行列を直接設定してはなりません。
        /// Update メソッドが呼び出されると、Actor の座標ベクトルと姿勢行列が
        /// このプロパティの座標ベクトルと姿勢行列へコピーされ、内部の行列が更新されます。
        /// つまり、このプロパティへそれらを設定しても、
        /// Update メソッドの呼び出しで無効化されてしまいます。
        /// </remarks>
        public Pov Pov
        {
            get { return pov; }
        }

        bool active;

        /// <summary>
        /// シーン内でアクティブなカメラかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (シーン内でアクティブなカメラ)、false (それ以外の場合)。
        /// </value>
        public bool Active
        {
            get { return active; }
            set
            {
                if (active != value)
                {
                    active = value;
                    if (ActiveChanged != null)
                    {
                        ActiveChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public CameraActor()
        {
            // 自動設定を有効にします。
            pov.AspectRatio = 0;
        }

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            // アスペクト比が 0 以下の場合、現在の Viewport のアスペクト比を設定します。
            if (pov.AspectRatio <= 0)
            {
                pov.AspectRatio = GraphicsDevice.Viewport.AspectRatio;
            }

            pov.Update();

            base.LoadContent();
        }

        #endregion

        #region Update

        /// <summary>
        /// Pov を更新します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <remarks>
        /// Active プロパティが true である場合にのみ更新処理を実行します。
        /// </remarks>
        public override void Update(GameTime gameTime)
        {
            if (!active)
            {
                return;
            }

            pov.SetPosition(ref Position);
            pov.SetOrientation(ref Orientation);
            pov.Update(false);

            base.Update(gameTime);
        }

        #endregion

        #region Helper

        /// <summary>
        /// 指定された座標を見るように姿勢行列を設定します。
        /// </summary>
        /// <param name="target">見る座標。</param>
        public void LookAt(Vector3 target)
        {
            LookAt(ref target);
        }

        /// <summary>
        /// 指定された座標を見るように姿勢行列を設定します。
        /// </summary>
        /// <param name="target">見る座標。</param>
        public void LookAt(ref Vector3 target)
        {
            Vector3 forward;
            Vector3.Subtract(ref target, ref Position, out forward);
            forward.Normalize();

            Vector3 up = Orientation.Up;
            Vector3 right;
            Vector3.Cross(ref forward, ref up, out right);
            right.Normalize();

            Vector3.Cross(ref right, ref forward, out up);
            up.Normalize();

            Orientation.Forward = forward;
            Orientation.Up = up;
            Orientation.Right = right;
        }

        /// <summary>
        /// 指定された座標を見るように姿勢行列を設定します。
        /// </summary>
        /// <param name="target">見る座標。</param>
        /// <param name="initialUp">初期 Up ベクトル。</param>
        public void LookAt(ref Vector3 target, ref Vector3 initialUp)
        {
            Vector3 forward;
            Vector3.Subtract(ref target, ref Position, out forward);
            forward.Normalize();

            Vector3 up = initialUp;
            Vector3 right;
            Vector3.Cross(ref forward, ref up, out right);
            right.Normalize();

            Vector3.Cross(ref right, ref forward, out up);
            up.Normalize();

            Orientation.Forward = forward;
            Orientation.Up = up;
            Orientation.Right = right;
        }

        #endregion
    }
}
