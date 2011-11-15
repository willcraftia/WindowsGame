#region Using

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Physics;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// JigLibX の PhysicsSystem を制御する GameComponent です。
    /// </summary>
    public sealed class JigLibXComponent : GameComponent
    {
        #region Fields and Properties

        // JigLibX の PhysicsSystem。
        PhysicsSystem physicsSystem;

        List<IIntegrationListener> integrationListeners = new List<IIntegrationListener>();
        
        /// <summary>
        /// IIntegrationListener のリストを取得します。
        /// </summary>
        public IList<IIntegrationListener> IntegrationListeners
        {
            get { return integrationListeners; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="game">インスタンスを登録する Game。</param>
        /// <param name="physicsSystem">JigLibX の PhysicsSystem。</param>
        public JigLibXComponent(Game game, PhysicsSystem physicsSystem)
            : base(game)
        {
            this.physicsSystem = physicsSystem;
        }

        #endregion

        #region Update

        /// <summary>
        /// JigLibX の PhysicsSystem のインテグレーションを実行します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <remarks>
        /// インテグレーション実行の前後では登録された IIntegrationListener が呼び出されます。
        /// </remarks>
        public override void Update(GameTime gameTime)
        {
            if (IntegrationListeners.Count != 0)
            {
                foreach (var listener in integrationListeners)
                {
                    listener.PreIntegration(gameTime);
                }
            }

            physicsSystem.Integrate(gameTime.GetDeltaTime());

            if (IntegrationListeners.Count != 0)
            {
                foreach (var listener in integrationListeners)
                {
                    listener.PostIntegration(gameTime);
                }
            }
        }

        #endregion
    }
}
