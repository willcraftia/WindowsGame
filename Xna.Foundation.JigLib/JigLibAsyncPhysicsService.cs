#region Using

using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Willcraftia.Xna.Framework.Physics;
using Willcraftia.Xna.Foundation.JigLib;
using JigLibX.Physics;
using JigLibX.Collision;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib
{
    /// <summary>
    /// IPhysicsService インタフェースを備えた JigLibX の非同期インテグレーション実装です。
    /// </summary>
    public sealed class JigLibXAsyncPhysicsService : IPhysicsService
    {
        #region Fields and Properties

        PhysicsSystem physicsSystem;

        /// <summary>
        /// JigLibX の PhysicsSystem。
        /// </summary>
        public PhysicsSystem PhysicsSystem
        {
            get { return physicsSystem; }
        }

        // JigLibX の PhysicsSystem を非同期化する AsyncPhysicsSystem。
        AsyncPhysicsSystem asyncPhysicsSystem;

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="game">サービスが属する Game。</param>
        public JigLibXAsyncPhysicsService(Game game, PhysicsSystem physicsSystem)
        {
            this.physicsSystem = physicsSystem;

            rigidBodyFactory = new JigLibXRigidBodyFactory(OnCollided);
            collisionBoundsFactory = new JigLibXCollisionBoundsFactory(OnCollided);

            // 非同期処理を初期化します。
            asyncPhysicsSystem = new AsyncPhysicsSystem(game, physicsSystem);
            asyncPhysicsSystem.Enabled = true;
        }

        public bool OnCollided(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (collisionTester != null)
            {
                var bounds0 = skin0 as ICollisionBounds;
                var bounds1 = skin1 as ICollisionBounds;
                if (bounds0 != null && bounds1 != null)
                {
                    return collisionTester.OnCollided(bounds0, bounds1);
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// ICollisionShape 型と実装クラスのマッピングを登録します。
        /// </summary>
        /// <typeparam name="TInterface">ICollisionShape 型。</typeparam>
        /// <typeparam name="TImplementation">ICollisionShape 実装型。</typeparam>
        /// <remarks>
        /// コードを簡略化するために ICollisionShape 実装型はデフォルト コンストラクタを仮定しています。
        /// </remarks>
        public void AddCollisionShape<TInterface, TImplementation>()
            where TInterface : ICollisionShape
            where TImplementation : ICollisionShape, new()
        {
            var interfaceType = typeof(TInterface);
            var implementationType = typeof(TImplementation);

            var constructor = implementationType.GetConstructor(Type.EmptyTypes);
            collisionShapeFactory.Implementations.Add(interfaceType, constructor);
        }

        /// <summary>
        /// 登録されている ICollisionShape 型と実装クラスのマッピングを削除します。
        /// </summary>
        /// <typeparam name="TInterface">ICollisionShape 型。</typeparam>
        public void RemoveCollisionShape<TInterface>() where TInterface : ICollisionShape
        {
            collisionShapeFactory.Implementations.Remove(typeof(TInterface));
        }

        public Collection<IAsyncIntegrationListener> AsyncIntegrationListeners
        {
            get { return asyncPhysicsSystem.AsyncIntegrationListeners; }
        }

        /// <summary>
        /// 非同期インテグレーションを開始します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        public void BeginUpdate(GameTime gameTime)
        {
            asyncPhysicsSystem.BeginUpdate(gameTime);
        }

        /// <summary>
        /// 非同期インテグレーションを終了します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        public void EndUpdate(GameTime gameTime)
        {
            asyncPhysicsSystem.EndUpdate();
        }

        #region IPhysicsService

        JigLibXRigidBodyFactory rigidBodyFactory;
        public IRigidBodyFactory RigidBodyFactory
        {
            get { return rigidBodyFactory; }
        }

        JigLibXCollisionBoundsFactory collisionBoundsFactory;
        public ICollisionBoundsFactory CollisionBoundsFactory
        {
            get { return collisionBoundsFactory; }
        }

        JigLibXCollisionShapeFactory collisionShapeFactory = new JigLibXCollisionShapeFactory();
        public ICollisionShapeFactory CollisionShapeFactory
        {
            get { return collisionShapeFactory; }
        }

        ICollisionTester collisionTester;
        public ICollisionTester CollisionTester
        {
            get { return collisionTester; }
            set { collisionTester = value; }
        }

        public bool Enabled
        {
            get { return asyncPhysicsSystem.Enabled; }
            set { asyncPhysicsSystem.Enabled = value; }
        }

        public Vector3 Gravity
        {
            get { return physicsSystem.Gravity; }
            set { physicsSystem.Gravity = value; }
        }

        #endregion
    }
}
