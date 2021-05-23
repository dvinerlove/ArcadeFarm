using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Text;

namespace farm.Models
{
    public enum SpriteType
    {
        Wall,
        Arcade,
        Charecter
    }
    public enum Facing
    {
        Right, Left
    }
    public class Sprite : IEntity, ICloneable
    {
        public IShapeF Bounds { get; set; }
        public Facing facing;
        public Texture2D Texture2D { get; set; }
        public Vector2 Velocity;

        public Vector2 Origin;
        public Vector2 Position;
        public Vector2 Direction;
        public Sprite Parent;
        public bool IsRemoved = false;
        public float _rotation;
        public SpriteType SpriteType { get; set; }
        public Vector2 HP;

        public MouseState _previousMouseKey;
        public MouseState _currentMouseKey;

        public bool IsSolid = false;
        public string sType;

        public Sprite(RectangleF rectangle, Texture2D texture)
        {
            Bounds = rectangle;
            rectParams = (RectangleF)Bounds;
            Texture2D = texture;
        }

        public float LifeSpan { get; set; }
        public Vector2 MousePosition { get; set; }
        public int Id { get; set; }
        public RectangleF rectParams { get; set; }
        public SpriteEffects Effect { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public bool FromPlayer = false;

        public virtual void Update(GameTime gameTime, List<IEntity> entities)
        {

            rectParams = (RectangleF)Bounds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D, (rectParams).ToRectangle(), Color.White);
        }

        public virtual void OnCollision(CollisionEventArgs collisionInfo)
        {
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
