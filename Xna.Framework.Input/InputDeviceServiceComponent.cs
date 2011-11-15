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
        /// InputDevice を初期化します。
        /// </summary>
        public override void Initialize()
        {
            inputDevice.Initialize();
        }

        /// <summary>
        /// マウスの初期座標を画面中央に修正します。
        /// </summary>
        protected override void LoadContent()
        {
            var viewport = GraphicsDevice.Viewport;
            inputDevice.GetMouse().Initialize(viewport.Width / 2, viewport.Height / 2);

            base.LoadContent();
        }

        /// <summary>
        /// InputDevice の状態をキャプチャーします。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        public override void Update(GameTime gameTime)
        {
            inputDevice.Update(gameTime);
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            // コンポーネントの Enabled と入力デバイスを同期させます。
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