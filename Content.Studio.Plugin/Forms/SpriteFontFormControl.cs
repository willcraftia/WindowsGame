#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Framework;
using Willcraftia.Win.Xna.Framework;
using Willcraftia.Win.Xna.Framework.Design;
using Willcraftia.Content.Studio.Forms;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Forms
{
    public sealed class SpriteFontFormControl : RuntimeContentFormControl
    {
        SpriteFont spriteFont;
        SpriteBatch spriteBatch;

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        public override void LoadContent()
        {
            if (RuntimeContent == null) return;

            base.LoadContent();

            spriteFont = RuntimeContent.LoadContent(ContentManager) as SpriteFont;
        }

        public override void UnloadContent()
        {
            spriteFont = null;

            base.UnloadContent();
        }

        protected override void Draw()
        {
            var bgColor = BackColor.ToXnaColor();
            GraphicsDevice.Clear(bgColor);

            // 実行時コンテンツをロードできていない場合には描画処理を行いません。
            if (spriteFont == null) return;

            var size = spriteFont.MeasureString("X");

            var position = Vector2.Zero;

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, "Numeric Characters:", position, Color.White);
            position.Y += size.Y;
            spriteBatch.DrawString(spriteFont, "    1234567890", position, Color.White);
            position.Y += size.Y;
            spriteBatch.DrawString(spriteFont, "Alphabetical Characters:", position, Color.White);
            position.Y += size.Y;
            spriteBatch.DrawString(spriteFont, "    abcdefghijklmnopqrstuvwxyz", position, Color.White);
            position.Y += size.Y;
            spriteBatch.DrawString(spriteFont, "    ABCDEFGHIJKLMNOPQRSTUVWXYZ", position, Color.White);
            spriteBatch.End();
        }
    }
}
