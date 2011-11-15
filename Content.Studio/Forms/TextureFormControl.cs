#region Using

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Win.Xna.Framework;
using Willcraftia.Win.Xna.Framework.Forms;
using Willcraftia.Win.Xna.Framework.Design;

#endregion

namespace Willcraftia.Content.Studio.Forms
{
    public class TextureFormControl : RuntimeContentFormControl
    {
        Texture2D texture;
        SpriteBatch spriteBatch;

        protected override void Initialize()
        {
            if (!DesignMode)
            {
                spriteBatch = new SpriteBatch(GraphicsDevice);
            }

            base.Initialize();
        }

        public override void LoadContent()
        {
            if (RuntimeContent == null) return;

            base.LoadContent();

            texture = RuntimeContent.LoadContent(ContentManager) as Texture2D;
        }

        public override void UnloadContent()
        {
            texture = null;

            base.UnloadContent();
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(BackColor.ToXnaColor());

            if (texture != null)
            {
                float w = (float) texture.Width;
                float h = (float) texture.Height;

                float ratio = w / h;

                w = (float) Width;
                h = w / ratio;

                if (Height < h)
                {
                    h = Height;
                    w = h * ratio;
                }

                int centerX = (int) (Width * 0.5f);
                int centerY = (int) (Height * 0.5f);

                int halfSizeX = (int) (w * 0.5f);
                int halfSizeY = (int) (h * 0.5f);

                int x = centerX - halfSizeX;
                int y = centerY - halfSizeY;

                var rect = new Rectangle(x, y, (int) w, (int) h);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(texture, rect, Color.White);
                spriteBatch.End();
            }
        }
    }
}
