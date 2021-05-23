using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Text;

namespace farm.Models
{
    public interface IEntity : ICollisionActor
    {

        public int Id { get; set; }
        RectangleF rectParams { get; set; }

        void Update(GameTime gameTime, List<IEntity> entities);
        void Draw(SpriteBatch spriteBatch);

        SpriteEffects Effect { get; set; }
    }
}
