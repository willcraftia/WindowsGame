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

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// 描画処理の開始と終了に応じて ITimeRuler のマーカ開始と終了を制御するクラスです。
    /// </summary>
    public sealed class TimeRulerDrawCallback : IDrawCallback
    {
        #region Fields and Properties

        // マーカ名。
        string name;

        // バーの色。
        Color color;
        
        // バーのインデックス。
        int index;

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="name">マーカ名。</param>
        /// <param name="color">バーの色。</param>
        public TimeRulerDrawCallback(string name, Color color)
            : this(name, color, 0)
        {
        }

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="name">マーカ名。</param>
        /// <param name="color">バーの色。</param>
        /// <param name="index">バーのインデックス。</param>
        public TimeRulerDrawCallback(string name, Color color, int index)
        {
            this.name = name;
            this.color = color;
            this.index = index;
        }

        #endregion

        #region IDrawCallback

        /// <summary>
        /// 描画の開始に応じてマーカを開始します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        public void OnDrawStarted(GameTime gameTime)
        {
            if (TimeRuler.Instance != null)
            {
                TimeRuler.Instance.BeginMark(name, color, index);
            }
        }

        /// <summary>
        /// 描画の終了に応じてマーカを終了します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        public void OnDrawCompleted(GameTime gameTime)
        {
            if (TimeRuler.Instance != null)
            {
                TimeRuler.Instance.EndMark(name, index);
            }
        }

        #endregion
    }
}
