#region Using

using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// インテグレーション処理のリスナ インタフェースです。
    /// </summary>
    public interface IIntegrationListener
    {
        /// <summary>
        /// インテグレーション処理の開始前に呼び出されます。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        void PreIntegration(GameTime gameTime);

        /// <summary>
        /// インテグレーション処理の完了後に呼び出されます。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        void PostIntegration(GameTime gameTime);
    }
}
