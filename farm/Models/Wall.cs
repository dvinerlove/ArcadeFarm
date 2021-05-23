using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace farm.Models
{
    public class Wall : Sprite
    {
        public Wall(RectangleF rectangle, Texture2D texture) : base(rectangle, texture)
        {
            SpriteType = SpriteType.Wall;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (FromPlayer)
            {

                var texture = Texture2DExtention.GetPart(Texture2D, new Rectangle(0, 32, 32, 32));
                spriteBatch.Draw(texture, rectParams.Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                 texture = Texture2DExtention.GetPart(Texture2D, new Rectangle(0, 0, 32, 32));
                spriteBatch.Draw(texture, new Vector2(rectParams.Position.X, rectParams.Position.Y-32), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }

        }

        public override void Update(GameTime gameTime, List<IEntity> entities)
        {
            base.Update(gameTime, entities);
        }
    }
}
