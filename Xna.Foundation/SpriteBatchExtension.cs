#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation
{
    public static class SpriteBatchExtension
    {
        #region Extension

        public static void Render(
            this SpriteBatch spriteBatch,
            Effect effect,
            Texture2D texture)
        {
            Render(spriteBatch, effect, texture, Vector2.Zero);
        }

        public static void Render(
            this SpriteBatch spriteBatch,
            Effect effect,
            Texture2D texture,
            Vector2 position)
        {
            Render(spriteBatch, effect, texture, ref position);
        }

        public static void Render(
            this SpriteBatch spriteBatch,
            Effect effect,
            Texture2D texture,
            ref Vector2 position)
        {
            var color = Color.White;
            Render(spriteBatch, effect, texture, ref position, ref color);
        }

        public static void Render(
            this SpriteBatch spriteBatch,
            Effect effect,
            Texture2D texture,
            ref Vector2 position,
            ref Color color)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    spriteBatch.Draw(texture, position, color);
                }
            }
            spriteBatch.End();
        }

        public static void Render(
            this SpriteBatch spriteBatch,
            Effect effect,
            Texture2D texture,
            Rectangle destinationRectangle)
        {
            Render(spriteBatch, effect, texture, ref destinationRectangle);
        }

        public static void Render(
            this SpriteBatch spriteBatch,
            Effect effect,
            Texture2D texture,
            ref Rectangle destinationRectangle)
        {
            var color = Color.White;
            Render(spriteBatch, effect, texture, ref destinationRectangle, ref color);
        }

        public static void Render(
            this SpriteBatch spriteBatch,
            Effect effect,
            Texture2D texture,
            ref Rectangle destinationRectangle,
            ref Color color)
        {
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            //{
            //    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        spriteBatch.Draw(texture, destinationRectangle, color);
            //    }
            //}
            //spriteBatch.End();

            var samplerState = texture.ResolveSamplerState();

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                samplerState,
                null,
                null,
                effect);
            spriteBatch.Draw(texture, destinationRectangle, Color.White);
            spriteBatch.End();
        }

        #endregion
    }
}
