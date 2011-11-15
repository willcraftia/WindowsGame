#region Using

using System;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// 非同期物理システムでの GameTime を表すクラスです。
    /// </summary>
    public sealed class AsyncGameTime
    {
        #region Fields and Properties

        internal TimeSpan elapsedGameTime;

        /// <summary>
        /// 前回の更新から経過したゲーム時間の合計。
        /// </summary>
        public TimeSpan ElapsedGameTime
        {
            get { return elapsedGameTime; }
        }

        internal bool isRunningSlowly;
        
        /// <summary>
        /// ゲーム ループの時間が TargetElapsedTime を超えていることを示す値。
        /// </summary>
        /// <value>
        /// true (ゲーム ループに時間がかかりすぎている場合)、false (それ以外の場合)。
        /// </value>
        public bool IsRunningSlowly
        {
            get { return isRunningSlowly; }
        }

        internal TimeSpan totalGameTime;

        /// <summary>
        /// ゲームの開始以降のゲーム時間の合計。
        /// </summary>
        public TimeSpan TotalGameTime
        {
            get { return totalGameTime; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        internal AsyncGameTime()
        {
        }

        #endregion
    }
}
