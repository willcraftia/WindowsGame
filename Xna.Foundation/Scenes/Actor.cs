#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public class Actor : ICloneable
    {
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        ActorModel actorModel;

        [ContentSerializer(Optional = true)]
        public ActorModel ActorModel
        {
            get { return actorModel; }
            set
            {
                if (actorModel == value) return;

                if (actorModel != null)
                {
                    actorModel.Actor = null;
                }

                actorModel = value;

                if (actorModel != null)
                {
                    actorModel.Actor = this;
                }
            }
        }

        CollisionBoundsConfig collisionBoundsConfig;

        [ContentSerializer(Optional = true)]
        public CollisionBoundsConfig CollisionBoundsConfig
        {
            get { return collisionBoundsConfig; }
            set { collisionBoundsConfig = value; }
        }

        string name;

        [ContentSerializerIgnore]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        bool enabled = true;

        [ContentSerializerIgnore]
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;

                enabled = value;

                if (EnabledChanged != null)
                {
                    EnabledChanged(this, EventArgs.Empty);
                }
            }
        }

        bool visible = true;

        [ContentSerializerIgnore]
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible == value) return;

                visible = value;

                if (VisibleChanged != null)
                {
                    VisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        [ContentSerializerIgnore]
        public Vector3 Position = Vector3.Zero;

        [ContentSerializerIgnore]
        public Matrix Translation;

        [ContentSerializerIgnore]
        public Matrix Orientation = Matrix.Identity;

        [ContentSerializerIgnore]
        public Matrix Scale = Matrix.Identity;

        [ContentSerializerIgnore]
        public Matrix Transform = Matrix.Identity;

        ICollisionBounds collisionBounds;

        [ContentSerializerIgnore]
        public ICollisionBounds CollisionBounds
        {
            get { return collisionBounds; }
            set { collisionBounds = value; }
        }

        IActorContext actorContext;

        [ContentSerializerIgnore]
        public IActorContext ActorContext
        {
            get { return actorContext; }
            set { actorContext = value; }
        }

        bool contentLoaded;

        [ContentSerializerIgnore]
        public bool ContentLoaded
        {
            get { return contentLoaded; }
        }

        /// <summary>
        /// 現在有効な GraphicsDevice を取得するヘルパ プロパティです。
        /// </summary>
        protected GraphicsDevice GraphicsDevice
        {
            get { return actorContext.GraphicsDevice; }
        }

        public virtual void LoadContent()
        {
            if (actorModel != null)
            {
                actorModel.LoadContent();
            }

            contentLoaded = true;
        }

        public virtual void UnloadContent()
        {
            if (actorModel != null)
            {
                actorModel.UnloadContent();
            }
            if (collisionBounds != null)
            {
                collisionBounds.Enabled = false;
            }

            contentLoaded = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateTransform(gameTime);
            UpdateActorModel(gameTime);
        }

        protected virtual void UpdateTransform(GameTime gameTime)
        {
            Matrix.CreateTranslation(ref Position, out Translation);

            Matrix scaleOrientation;
            Matrix.Multiply(ref Scale, ref Orientation, out scaleOrientation);
            Matrix.Multiply(ref scaleOrientation, ref Translation, out Transform);
        }

        protected virtual void UpdateActorModel(GameTime gameTime)
        {
            if (actorModel != null)
            {
                actorModel.Update(gameTime);
            }
        }

        public virtual object Clone()
        {
            var other = MemberwiseClone() as Actor;
            if (actorModel != null)
            {
                other.ActorModel = actorModel.Clone() as ActorModel;
            }
            return other;
        }
    }
}
