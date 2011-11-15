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
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// キャラクタを表す Actor です。
    /// </summary>
    /// <remarks>
    /// 剛体と共に位置と姿勢が管理されるのは CharacterActor だけです。
    /// 換言すると、剛体と共に位置と姿勢を管理したい任意の Actor は、
    /// CharacterActor としてシーンに登録する必要があります。
    /// </remarks>
    public class CharacterActor : Actor
    {
        #region Fields and Properties

        RigidBodyConfig rigidBodyConfig = new RigidBodyConfig();

        [ContentSerializer(AllowNull = false)]
        public RigidBodyConfig RigidBodyConfig
        {
            get { return rigidBodyConfig; }
        }

        bool rigidBodyOrientationIgnored;

        [DefaultValue(false)]
        public bool RigidBodyOrientationIgnored
        {
            get { return rigidBodyOrientationIgnored; }
            set { rigidBodyOrientationIgnored = value; }
        }

        Dictionary<ActionSoundKey, string> actionSounds = new Dictionary<ActionSoundKey, string>();
        public Dictionary<ActionSoundKey, string> ActionSounds
        {
            get { return actionSounds; }
        }

        IRigidBody rigidBody;

        [ContentSerializerIgnore]
        public IRigidBody RigidBody
        {
            get { return rigidBody; }
            set { rigidBody = value; }
        }

        #endregion

        #region UnloadContent

        public override void UnloadContent()
        {
            if (rigidBody != null)
            {
                rigidBody.Enabled = false;
            }

            base.UnloadContent();
        }

        #endregion

        #region Update

        protected override void UpdateTransform(GameTime gameTime)
        {
            if (rigidBody != null)
            {
                // IRigidBody と Actor の位置ベクトルを同期させます。
                rigidBody.GetPosition(out Position);
                if (!rigidBodyOrientationIgnored)
                {
                    // IRigidBody と Actor の姿勢行列を同期させます。
                    rigidBody.GetOrientation(out Orientation);
                }
            }

            base.UpdateTransform(gameTime);
        }

        #endregion
    }
}
