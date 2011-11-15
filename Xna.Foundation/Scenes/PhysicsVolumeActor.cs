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
    /// 物理ボリュームを表す Actor です。
    /// </summary>
    public class PhysicsVolumeActor : VolumeActor
    {
        #region Fields and Properties

        ActorCollection<CharacterActor> characters = new ActorCollection<CharacterActor>();

        /// <summary>
        /// ボリューム内に存在する CharacterActor のコレクションを取得します。
        /// </summary>
        /// <remarks>
        /// このコレクションに CharacterActor を追加すると OnCharacterEntered メソッドが呼び出され、
        /// CharacterActor が物理ボリュームに入った時の処理が実行されます。
        /// 一方、このコレクションから Actor を削除すると OnCharacterLeaving メソッドが呼び出され、
        /// CharacterActor が物理ボリュームから出た時の処理が実行されます。
        /// </remarks>
        public ActorCollection<CharacterActor> Characters
        {
            get { return characters; }
        }

        IPhysicsVolumeContext physicsVolumeContext;

        [Browsable(false)]
        [ContentSerializerIgnore]
        public IPhysicsVolumeContext PhysicsVolumeContext
        {
            get { return physicsVolumeContext; }
            set { physicsVolumeContext = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        public PhysicsVolumeActor()
        {
            // CharacterActor の物理ボリュームへの入出で処理を適用するためのイベント ハンドラを関連付けます。
            characters.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnCharacterAdded);
            characters.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnCharacterRemoved);
        }

        /// <summary>
        /// コレクションに CharacterActor を追加した場合に呼び出され、
        /// OnCharacterEntered メソッドを呼び出します。
        /// </summary>
        /// <param name="sender">イベント元。</param>
        /// <param name="e">イベント データ。</param>
        void OnCharacterAdded(object sender, ActorCollectionEventArgs e)
        {
            OnCharacterEntered(e.Item as CharacterActor);
        }

        /// <summary>
        /// コレクションから CharacterActor を削除した場合に呼び出され、
        /// OnCharacterLeaving メソッドを呼び出します。
        /// </summary>
        /// <param name="sender">イベント元。</param>
        /// <param name="e">イベント データ。</param>
        void OnCharacterRemoved(object sender, ActorCollectionEventArgs e)
        {
            OnCharacterLeaving(e.Item as CharacterActor);
        }

        #endregion

        /// <summary>
        /// シーンに存在する CharacterActor が物理ボリューム内に存在するかどうかを判定し、
        /// 物理ボリューム内に存在する CharacterActor を自身のコレクションに追加します。
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            Characters.Clear();

            foreach (var character in PhysicsVolumeContext.Characters)
            {
                if (Shape.Contains(ref character.Position))
                {
                    Characters.Add(character);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// CharacterActor が物理ボリュームに入った時の処理を実行します。
        /// </summary>
        /// <param name="actor">物理ボリュームに入った CharacterActor。</param>
        protected virtual void OnCharacterEntered(CharacterActor actor)
        {
        }

        /// <summary>
        /// CharacterActor が物理ボリュームから出た時の処理を実行します。
        /// </summary>
        /// <param name="actor">物理ボリュームから出た CharacterActor。</param>
        protected virtual void OnCharacterLeaving(CharacterActor actor)
        {
        }
    }
}
