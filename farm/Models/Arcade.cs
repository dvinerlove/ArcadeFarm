using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace farm.Models
{
    public class Arcade : Sprite
    {
        public Color Color { get; set; }

        public int CoinPrice { get; set; }
        public int Bank { get; internal set; }

        public Arcade(RectangleF rectangle, Texture2D texture) : base(rectangle, texture)
        {
            Color = Color.FromNonPremultiplied(202, 115, 115, 150);
            Effect = SpriteEffects.None;
            SpriteType = SpriteType.Arcade;
            CoinPrice = 1;
            Bank = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D, new Vector2(rectParams.X - 16, rectParams.Y - 28), null, Color, 0, Vector2.Zero, 2, Effect, 0);
           ////Debug.WriteLine(rectParams);
           // base.Draw(spriteBatch);
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            base.OnCollision(collisionInfo);
        }

        public override void Update(GameTime gameTime, List<IEntity> entities)
        {
            rectParams = (RectangleF)Bounds;
        }
    }
}
