#region Using

using Microsoft.Xna.Framework;
using Willcraftia.Xna.Foundation.Debugs;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib.Debugs
{
    /// <summary>
    /// 非同期物理システムのインテグレーション処理に応じて ITimeRuler のマーカ開始と終了を制御するクラスです。
    /// </summary>
    public sealed class TimeRulerAsyncIntegrationListener : IAsyncIntegrationListener
    {
        // マーカ名。
        string name;

        // バーの色。
        Color color;

        // バーのインデックス。
        int index;

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="name">マーカ名。</param>
        /// <param name="color">バーの色。</param>
        public TimeRulerAsyncIntegrationListener(string name, Color color)
            : this(name, color, 0)
        {
        }

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="name">マーカ名。</param>
        /// <param name="color">バーの色。</param>
        /// <param name="index">バーのインデックス。</param>
        public TimeRulerAsyncIntegrationListener(string name, Color color, int index)
        {
            this.name = name;
            this.color = color;
            this.index = index;
        }

        #endregion

        #region IAsyncIntegrationListener

        /// <summary>
        /// インテグレーションの開始に応じてマーカを開始します。
        /// </summary>
        /// <param name="gameTime">非同期物理システムにおける前回の Update が呼び出されてからの経過時間。</param>
        public void PreIntegration(AsyncGameTime gameTime)
        {
            if (TimeRuler.Instance != null)
            {
                TimeRuler.Instance.BeginMark(name, color, index);
            }
        }

        /// <summary>
        /// インテグレーションの終了に応じてマーカを終了します。
        /// </summary>
        /// <param name="gameTime">非同期物理システムにおける前回の Update が呼び出されてからの経過時間。</param>
        public void PostIntegration(AsyncGameTime gameTime)
        {
            if (TimeRuler.Instance != null)
            {
                TimeRuler.Instance.EndMark(name, index);
            }
        }

        #endregion
    }
}
