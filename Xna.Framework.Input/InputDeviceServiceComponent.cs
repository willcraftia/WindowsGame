#region Using

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Input
{
    public sealed class InputDeviceServiceComponent : DrawableGameComponent, IInputDeviceService
    {
        public InputDeviceServiceComponent(Game game)
            : base(game)
        {
            Visible = false;
            inputDevice.Enabled = Enabled;
        }

        /// <summary>
        /// InputDevice �����������܂��B
        /// </summary>
        public override void Initialize()
        {
            inputDevice.Initialize();
        }

        /// <summary>
        /// �}�E�X�̏������W����ʒ����ɏC�����܂��B
        /// </summary>
        protected override void LoadContent()
        {
            var viewport = GraphicsDevice.Viewport;
            inputDevice.GetMouse().Initialize(viewport.Width / 2, viewport.Height / 2);

            base.LoadContent();
        }

        /// <summary>
        /// InputDevice �̏�Ԃ��L���v�`���[���܂��B
        /// </summary>
        /// <param name="gameTime">�O��� Update ���Ăяo����Ă���̌o�ߎ��ԁB</param>
        public override void Update(GameTime gameTime)
        {
            inputDevice.Update(gameTime);
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            // �R���|�[�l���g�� Enabled �Ɠ��̓f�o�C�X�𓯊������܂��B
            inputDevice.Enabled = Enabled;

            base.OnEnabledChanged(sender, args);
        }

        #region IInputDeviceService

        InputDevice inputDevice = new InputDevice();
        public IInputDevice InputDevice
        {
            get { return inputDevice; }
        }

        #endregion
    }
}