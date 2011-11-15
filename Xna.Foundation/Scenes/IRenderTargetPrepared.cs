#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// ActorModel 内部で固有の RenderTarget を管理する ActorModel のインタフェースです。
    /// </summary>
    /// <remarks>
    /// このインタフェースを備えた ActorModel は、描画対象と判定された場合に、
    /// シーンの描画開始前に PrepareRenderTarget メソッドが呼び出されます。
    /// 
    /// ActorModel の Draw メソッドで自身が管理する RenderTarget のテクスチャを用いたい場合、
    /// Draw メソッド内で RenderTarget にテクスチャを描画することはできません。
    /// RenderTarget にテクスチャを描画するには GraphicsDevice に設定する必要がありますが、
    /// Draw メソッドの呼び出し側は ActorModel 描画のための RenderTarget を GraphicsDevice に設定しており、
    /// Draw メソッド内で RenderTarget を切替えるわけにはいきません。
    /// このため、このインタフェースを ActorModel に実装させ、
    /// 一連の Draw メソッドの呼び出し前に ActorModel 内部で固有の RenderTarget へテクスチャを描画させます。
    /// </remarks>
    public interface IRenderTargetPrepared
    {
        /// <summary>
        /// ActorModel 内部で固有の RenderTarget を準備します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        void PrepareRenderTargets(GameTime gameTime);
    }
}
