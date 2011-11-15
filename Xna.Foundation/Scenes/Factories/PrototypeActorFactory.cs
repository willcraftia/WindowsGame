#region Using

using System.Collections.Generic;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Factories
{
    /// <summary>
    /// プロトタイプとなる Actor を生成して管理し、
    /// 実際のシーンに配置する Actor をプロトタイプから生成する ActorFactory です。
    /// </summary>
    /// <remarks>
    /// この ActorFactory は AssetActorConfig からの Actor 生成を対象とします。
    /// 同一コンテンツを使用しつつ Actor 毎にプロパティを変更するような Actor は、
    /// この ActorFactory で生成を担うようにすることでコンテンツを共有させます。
    /// </remarks>
    public abstract class PrototypeActorFactory : ActorFactory
    {
        // プロトタイプを管理する Dictionary。
        Dictionary<string, Actor> prototypes = new Dictionary<string, Actor>();

        /// <summary>
        /// プロトタイプのクローンを生成して返します。
        /// </summary>
        /// <param name="config">ActorConfig。</param>
        /// <returns>プロトタイプのクローン。</returns>
        protected override Actor CreateActorInstance(ActorConfig config)
        {
            var assetActorConfig = config as AssetActorConfig;

            // アセット名からプロトタイプを取得します。
            Actor prototype;
            if (!prototypes.TryGetValue(assetActorConfig.AssetName, out prototype))
            {
                // プロトタイプが存在しないならばそれを生成して登録します。
                prototype = CreatePrototypeActor(assetActorConfig);
                prototypes.Add(assetActorConfig.AssetName, prototype);
            }

            // プロトタイプのクローンを生成して返します。
            return prototype.Clone() as Actor;
        }

        /// <summary>
        /// ActorConfig からプロトタイプを生成します。
        /// </summary>
        /// <param name="config">ActorConfig。</param>
        /// <returns>生成されたプロトタイプ。</returns>
        protected abstract Actor CreatePrototypeActor(AssetActorConfig config);
    }
}
