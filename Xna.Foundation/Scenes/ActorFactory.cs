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
    /// ActorConfig からの Actor の生成を担うファクトリ クラスです。
    /// </summary>
    /// <remarks>
    /// ActorFactory は、デフォルト コンストラクタが存在することを仮定して実行時に動的生成されます。
    /// したがって、ActorFactory のサブクラスは必ずデフォルト コンストラクタを実装しなければなりません。
    /// 
    /// なお、Actor のコンテンツのロードは、ActorFactory へ生成を依頼したクラスで行うものとし、
    /// ActorFactory のメソッドでロードしようしてはなりません。
    /// Actor 生成依頼側のクラスは、生成された Actor へ適切な IActorContext を設定してから、
    /// Actor のコンテンツをロードします。
    /// </remarks>
    public abstract class ActorFactory
    {
        IActorFactoryContext actorFactoryContext;

        /// <summary>
        /// IActorFactoryContext を取得または設定します。
        /// </summary>
        /// <remarks>
        /// IActorFactoryContext は ActorFactory を生成および管理するクラスから設定されます。
        /// </remarks>
        public IActorFactoryContext ActorFactoryContext
        {
            get { return actorFactoryContext; }
            set { actorFactoryContext = value; }
        }

        /// <summary>
        /// ActorConfig から Actor を生成します。
        /// </summary>
        /// <param name="config">ActorConfig。</param>
        /// <returns>生成された Actor。</returns>
        /// <remarks>
        /// このメソッドは、以下の順序でメソッドを呼び出して Actor を生成します。
        /// 
        /// 1. CreateActorInstance
        /// 2. InitializeActorProperties
        /// 
        /// </remarks>
        public virtual Actor CreateActor(ActorConfig config)
        {
            var instance = CreateActorInstance(config);
            InitializeActorProperties(instance, config);
            return instance;
        }

        /// <summary>
        /// ActorConfig から Actor を生成します。
        /// </summary>
        /// <param name="config">ActorConfig。</param>
        /// <returns>生成された Actor。</returns>
        /// <remarks>
        /// このメソッドでは Actor のインスタンス生成のみを担い、
        /// プロパティの設定やコンテンツのロードは行いません。
        /// </remarks>
        protected abstract Actor CreateActorInstance(ActorConfig config);

        /// <summary>
        /// 生成された Actor を ActorConfig の設定に従って初期化します。
        /// </summary>
        /// <param name="config">ActorConfig。</param>
        /// <param name="instance">Actor。</param>
        /// <remarks>
        /// このメソッドでは Actor へのプロパティの設定のみを担い、
        /// コンテンのロードは行いません。
        /// </remarks>
        protected virtual void InitializeActorProperties(Actor instance, ActorConfig config)
        {
            instance.Name = config.Name;
            instance.Position = config.Position;
            Matrix.CreateFromYawPitchRoll(
                config.Orientation.Y,
                config.Orientation.X,
                config.Orientation.Z,
                out instance.Orientation);
            Matrix.CreateScale(ref config.Scale, out instance.Scale);
            instance.Enabled = config.Enabled;
            instance.Visible = config.Visible;
        }
    }
}
