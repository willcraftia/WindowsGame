#region Using

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// Actor の描画を担うクラスです。
    /// </summary>
    /// <remarks>
    /// ActorModel は、同じ描画を行う Actor 間で共有されます。
    /// </remarks>
    public abstract class ActorModel : ICloneable
    {
        float maxDrawDistance = 0.0f;

        /// <summary>
        /// カメラから描画されうる最大距離。
        /// </summary>
        public float MaxDrawDistance
        {
            get { return maxDrawDistance; }
            set { maxDrawDistance = value; }
        }

        bool nearTransparencyEnabled = false;

        /// <summary>
        /// カメラが近接した場合に透明化するかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (透明化する場合)、false (それ以外の場合)。
        /// </value>
        public bool NearTransparencyEnabled
        {
            get { return nearTransparencyEnabled; }
            set { nearTransparencyEnabled = value; }
        }

        bool cullingTransparencyEnabled = false;

        /// <summary>
        /// カリング時に透明化するかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (透明化する場合)、false (それ以外の場合)。
        /// </value>
        public bool CullingTransparencyEnabled
        {
            get { return cullingTransparencyEnabled; }
            set { cullingTransparencyEnabled = value; }
        }

        bool castShadowEnabled = false;

        /// <summary>
        /// 他の Actor へ影を落とすかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (影を落とす場合)、false (それ以外の場合)。
        /// </value>
        public bool CastShadowEnabled
        {
            get { return castShadowEnabled; }
            set { castShadowEnabled = value; }
        }

        public static float NearTransparencyAlpha = 0.5f;
        public static float NearTransparentDistance = 2;
        public static float CullingTransparentRange = 10;

        double preparedEffectOnceDrawTime;

        Actor actor;

        /// <summary>
        /// Actor。
        /// </summary>
        [ContentSerializerIgnore]
        public Actor Actor
        {
            get { return actor; }
            set
            {
                if (actor == value) return;

                actor = value;
                OnActorChanged();
            }
        }

        /// <summary>
        /// Actor プロパティの変更で呼び出されます。
        /// </summary>
        protected virtual void OnActorChanged()
        {
        }

        /// <summary>
        /// ローカル座標系での BoundingBox。
        /// </summary>
        [ContentSerializerIgnore]
        public BoundingBox LocalBoundingBox = BoundingBoxExtension.Empty;

        /// <summary>
        /// ワールド座標系での BoundingBox。
        /// </summary>
        [ContentSerializerIgnore]
        public BoundingBox WorldBoundingBox = BoundingBoxExtension.Empty;

        /// <summary>
        /// 透明化されているかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (透明化されている場合)、false (それ以外の場合)。
        /// </value>
        protected bool Transparent = false;

        /// <summary>
        /// 透明化されている場合での現在のアルファ値。
        /// </summary>
        protected float CurrentAlpha = 1.0f;

        bool contentLoaded;

        /// <summary>
        /// コンテンツがロード済みであるかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (ロード済みの場合)、false (それ以外の場合)。
        /// </value>
        [ContentSerializerIgnore]
        public bool ContentLoaded
        {
            get { return contentLoaded; }
        }

        /// <summary>
        /// Actor 実行環境。
        /// </summary>
        protected IActorContext ActorContext
        {
            get { return actor.ActorContext; }
        }

        /// <summary>
        /// 現在の GraphicsDevice を取得するヘルパ プロパティです。
        /// </summary>
        protected GraphicsDevice GraphicsDevice
        {
            get { return ActorContext.GraphicsDevice; }
        }

        protected ContentManager Content
        {
            get { return ActorContext.Content; }
        }

        /// <summary>
        /// コンテンツをロードします。
        /// </summary>
        /// <remarks>
        /// 呼び出しにより ContentLoaded プロパティが true に設定されます。
        /// </remarks>
        public virtual void LoadContent()
        {
            contentLoaded = true;
        }

        /// <summary>
        /// コンテンツをアンロードします。
        /// </summary>
        /// <remarks>
        /// 呼び出しにより ContentLoaded プロパティが false に設定されます。
        /// </remarks>
        public virtual void UnloadContent()
        {
            contentLoaded = false;
        }

        public virtual bool Intersects(
            ref Vector3 position,
            ref Vector3 vector,
            out Vector3 intersectPosition,
            out Vector3 intersectNormal,
            out float intersectFraction)
        {
            if (actor.CollisionBounds != null)
            {
                return actor.CollisionBounds.Intersects(
                    ref position,
                    ref vector,
                    out intersectPosition,
                    out intersectNormal,
                    out intersectFraction);
            }
            else
            {
                intersectPosition = Vector3.Zero;
                intersectNormal = Vector3.Up;
                intersectFraction = float.MaxValue;
                return false;
            }
        }

        /// <summary>
        /// Game の Update メソッド内で呼び出されます。
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            UpdateWorldBoundingBox(gameTime);
            UpdateTransparency(gameTime);
        }

        /// <summary>
        /// ワールド座標系での BoundingBox を更新します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        protected abstract void UpdateWorldBoundingBox(GameTime gameTime);

        /// <summary>
        /// 透明化情報を更新します。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        protected virtual void UpdateTransparency(GameTime gameTime)
        {
            var offset = Actor.Position - ActorContext.ActiveCamera.Position;
            var distance = offset.Length();
            if (NearTransparencyEnabled && distance <= NearTransparentDistance)
            {
                Transparent = true;
                CurrentAlpha = NearTransparencyAlpha;
            }
            else if (CullingTransparencyEnabled &&
                0 < MaxDrawDistance &&
                MaxDrawDistance - CullingTransparentRange <= distance)
            {
                Transparent = true;
                CurrentAlpha = (MaxDrawDistance - distance) / CullingTransparentRange;
            }
            else
            {
                Transparent = false;
                CurrentAlpha = 1.0f;
            }
        }

        /// <summary>
        /// Game の Draw メソッド呼び出し内で呼び出されます。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <remarks>
        /// この Draw メソッドは複数回呼び出される可能性があります。
        /// また、Actor が描画対象から除外されることで呼び出されない場合もあります。
        /// </remarks>
        public void Draw(GameTime gameTime)
        {
            if (PreDraw(gameTime))
            {
                OnDraw(gameTime);
                PostDraw(gameTime);
            }
        }

        /// <summary>
        /// Draw メソッドの本処理です。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        protected abstract void OnDraw(GameTime gameTime);

        /// <summary>
        /// OnDraw メソッドの前処理です。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <returns>
        /// true (Draw メソッドの本処理を実行する場合)、false (それ以外の場合)。
        /// </returns>
        protected virtual bool PreDraw(GameTime gameTime)
        {
            if (preparedEffectOnceDrawTime < gameTime.TotalGameTime.TotalSeconds)
            {
                PrepareEffectOnceDraw(gameTime);
            }

            if (Transparent)
            {
                GraphicsDevice.BlendState = BlendState.AlphaBlend;
            }

            return true;
        }

        /// <summary>
        /// Game の Draw メソッド呼び出し内で 1 度だけ呼び出される Effect 初期化処理です。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        protected virtual void PrepareEffectOnceDraw(GameTime gameTime)
        {
            preparedEffectOnceDrawTime = gameTime.TotalGameTime.TotalSeconds;
        }

        /// <summary>
        /// OnDraw メソッドの後処理です。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        protected virtual void PostDraw(GameTime gameTime)
        {
            if (Transparent)
            {
                GraphicsDevice.BlendState = BlendState.Opaque;
            }
        }

        /// <summary>
        /// 指定の Effect に基づいて描画するメソッドです。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <param name="effect">Effect。</param>
        /// <remarks>
        /// このメソッドは、Shadow Mapping などの Actor 描画側が指定する Effect で Actor を描画するために使用されます。
        /// </remarks>
        public virtual void Draw(GameTime gameTime, Effect effect)
        {
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
