#region Using

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// 非同期インテグレーションに対するリスナ インタフェースです。
    /// </summary>
    public interface IAsyncIntegrationListener
    {
        /// <summary>
        /// インテグレーションの開始前に呼び出されます。
        /// </summary>
        /// <param name="gameTime">非同期物理システムにおける前回の Update が呼び出されてからの経過時間。</param>
        void PreIntegration(AsyncGameTime gameTime);

        /// <summary>
        /// インテグレーションの終了後に呼び出されます。
        /// </summary>
        /// <param name="gameTime">非同期物理システムにおける前回の Update が呼び出されてからの経過時間。</param>
        void PostIntegration(AsyncGameTime gameTime);
    }
}
