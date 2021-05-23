using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace farm.Models
{

    class ItemCard
    {
        public int Id;
        public RectangleF Rectangle;
        public Color Color;
        public string GroupName;
        public ItemCard(int id)
        {
            Id = id;

        }
    }

    class Shop
    {
        public KeyedCollection<int, IEntity> Entities;
        RectangleF rectangle;
        public Vector2 Position;
        public List<ItemCard> itemCards;
        public RectangleF nextButton;
        public RectangleF prevButton;
        internal Color nextButtonColor;
        internal Color prevButtonColor;
        public SpriteType currentSpriteType;
        List<SpriteType> spriteTypes;

        public Shop()
        {
            Entities = new KeyedCollection<int, IEntity>(e => e.Id);
           // Position = new Vector2(1184, 512);
            itemCards = new List<ItemCard>();
            currentSpriteType = SpriteType.Arcade;

            spriteTypes = new List<SpriteType>();
            spriteTypes.Add(SpriteType.Arcade);
            spriteTypes.Add(SpriteType.Wall);
        }
        public void Init()
        {

            int i = 0;
            foreach (Sprite item in Entities)
            {
                if (item.SpriteType == SpriteType.Arcade)
                {
                    var pos = Position;
                    pos.Y += i * 58;
                    itemCards.Add(new ItemCard(item.Id) { Rectangle = new RectangleF(pos.X - 4, pos.Y - 4, 128, 52), Color = Color.Black, GroupName = item.SpriteType.ToString() });
                    i++;
                }

            }
            i = 0;
            foreach (Sprite item in Entities)
            {
                if (item.SpriteType == SpriteType.Wall)
                {
                    var pos = Position;
                    pos.Y += i * 58;
                    itemCards.Add(new ItemCard(item.Id) { Rectangle = new RectangleF(pos.X - 4, pos.Y - 4, 128, 52), Color = Color.Black, GroupName = item.SpriteType.ToString() });
                    i++;
                }

            }



        }

        public void NextClick()
        {

            var inx = spriteTypes.IndexOf(currentSpriteType);
            if (inx + 1 < spriteTypes.Count())
            {
                inx++;
                currentSpriteType = spriteTypes[inx];
            }

        }
        public void PrevClick()
        {

            var inx = spriteTypes.IndexOf(currentSpriteType);
            if (inx > 0)
            {
                inx--;
                currentSpriteType = spriteTypes[inx];
            }

        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            int i = 0;
            spriteBatch.DrawString(font, "$Shop$", new Vector2(Position.X, Position.Y - 32 - 16), Color.Black);
            ItemCard ic = itemCards[0];
            switch (currentSpriteType)
            {
                case SpriteType.Wall:
                    spriteBatch.DrawString(font, "\nWalls ", new Vector2(Position.X, Position.Y - 32 - 16), Color.Black);

                    foreach (Sprite item in Entities)
                    {
                        var pos = Position;
                        if (item.SpriteType == SpriteType.Wall)
                        {

                            pos.Y += i * 58;
                            var texture = Texture2DExtention.GetPart(item.Texture2D, new Rectangle(0, 32, 32, 32));
                            spriteBatch.Draw(texture, new Vector2(pos.X , pos.Y), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                            spriteBatch.DrawString(font, " " + item.Name, new Vector2(pos.X + 32, pos.Y), Color.Black);
                            spriteBatch.DrawString(font, "Cost : $" + (item as Wall).Cost.ToString(), new Vector2(pos.X, pos.Y + 32), Color.Black);

                            rectangle = new RectangleF(pos.X - 4, pos.Y - 4, 128, 52);
                            ic = itemCards.Where(x => x.Id == item.Id).FirstOrDefault();
                            spriteBatch.DrawRectangle(ic.Rectangle, ic.Color, 2);
                            rectangle = new RectangleF(pos.X, pos.Y, 32, 32);
                            spriteBatch.DrawRectangle(rectangle, Color.Black, 2);
                            i++;

                        }
                    }
                    break;
                case SpriteType.Arcade:
                    spriteBatch.DrawString(font, "\nArcade Machines ", new Vector2(Position.X, Position.Y - 32 - 16), Color.Black);



                    foreach (Sprite item in Entities)
                    {
                        var pos = Position;
                        if (item.SpriteType == SpriteType.Arcade)
                        {

                            pos.Y += i * 58;
                            var texture = Texture2DExtention.GetPart(item.Texture2D, new Rectangle(0, 0, 32, 16));
                            spriteBatch.Draw(texture, new Vector2(pos.X - 14, pos.Y), null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                            spriteBatch.DrawString(font, " " + item.Name, new Vector2(pos.X + 32, pos.Y), Color.Black);
                            spriteBatch.DrawString(font, "Cost : $" + (item as Arcade).Cost.ToString(), new Vector2(pos.X, pos.Y + 32), Color.Black);

                            rectangle = new RectangleF(pos.X - 4, pos.Y - 4, 128, 52);
                            ic = itemCards.Where(x => x.Id == item.Id).FirstOrDefault();
                            spriteBatch.DrawRectangle(ic.Rectangle, ic.Color, 2);
                            rectangle = new RectangleF(pos.X, pos.Y, 32, 32);
                            spriteBatch.DrawRectangle(rectangle, Color.Black, 2);
                            i++;

                        }
                    }

                    break;
                default:
                    break;
            }



            nextButton = new RectangleF(ic.Rectangle.Position, new Size2(32, 22));
            nextButton.Y = (i + 1) * 64;
            prevButton = nextButton;
            nextButton.X += 64;



            spriteBatch.DrawString(font, ">", new Vector2(nextButton.Position.X + nextButton.Width - 11, nextButton.Position.Y + 4), nextButtonColor);
            spriteBatch.DrawRectangle(nextButton, nextButtonColor, 2);


            spriteBatch.DrawString(font, "<", new Vector2(prevButton.Position.X + 4, prevButton.Position.Y + 4), prevButtonColor);
            spriteBatch.DrawRectangle(prevButton, prevButtonColor, 2);
        }
    }

}
