using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace farm.Models
{
    public class Charecter : Sprite
    {
        List<PointN> AvoidancePoints = new List<PointN>();
        public IEntity targetObject;
        public TiledMap tiledMap;
        public Vector2 LastPosition { get; set; }
        public float Speed { get; private set; }
        public bool Playing { get; private set; }

        private int Money;

        public SpriteFont SpriteFont;
        public Charecter(RectangleF rectangle, Texture2D texture, SpriteFont font) : base(rectangle, texture)
        {
            SpriteFont = font;
            //
            // 
            Origin = rectangle.Position;
            Speed = 50;
            Money = Random.Next(50, 500);
            SpriteType = SpriteType.Charecter;
        }

        public override void Update(GameTime gameTime, List<IEntity> entities)
        {

            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;



            if (targetObject == null)
            {


                NewDirCoolDown(gameTime, entities);
                ObstacleAvoidance(gameTime, entities);
                if (targetObject != null)
                {
                    AvoidancePoints.Clear();
                }

            }
            else
            {
                FindMachine(gameTime, entities);
                InserCoinCoolDown(gameTime, entities);
            }

            Bounds.Position = new Vector2(Origin.X - rectParams.Width / 2, Origin.Y - rectParams.Height / 2);
            rectParams = (RectangleF)Bounds;
        }

        private void FindMachine(GameTime gameTime, List<IEntity> entities)
        {
            if (AvoidancePoints == null || AvoidancePoints.Count < 1)
            {
                string[] logFile = GetMapArray(entities, targetObject.Bounds.Position);
                if (logFile.Length > 0)
                {
                    var logList = new List<string>(logFile);
                    int j = 0;
                    var pathSearch = PathSearch(logList);
                    if (pathSearch != null)
                        foreach (var str in pathSearch.ToArray())
                        {
                            for (int i = 0; i < str.Length; i++)
                            {
                                if (str[i] == '*')
                                {
                                    AvoidancePoints.Add(new PointN((i * 32) + 16, (j * 32) - 16 + 32, 1));
                                }
                            }
                            j++;
                        }
                    SetNewPosition();
                }
            }
            else
            {
                if (Vector2.Distance(Origin, LastPosition) < 32 || LastPosition == Vector2.Zero)
                {
                    SetNewPosition();
                }
                Move(gameTime, entities);
                int counter = 0;
                for (int i = 0; i < AvoidancePoints.Count; i++)
                {
                    counter += AvoidancePoints[i].id;
                }

                if (counter < 2)
                {

                    //}
                    //else
                    //{
                    //    NewDeriction();
                    //}
                    foreach (var item in AvoidancePoints)
                    {
                        item.id = 0;
                    }

                    if (targetObject.Effect == SpriteEffects.None)
                        LastPosition = new Vector2(targetObject.Bounds.Position.X - 32, targetObject.Bounds.Position.Y);
                    else
                        LastPosition = new Vector2(targetObject.Bounds.Position.X + 32, targetObject.Bounds.Position.Y);
                }

            }
        }

        public Vector2 ExitLocation { get; internal set; }



        public string[] GetMapArray(List<IEntity> entities, Vector2 target)
        {
            RectangleF targetRectangle;

            if (targetObject == null)
            {
                NewDeriction();
                targetRectangle = new RectangleF(target, new Size2(32, 32));
               //Debug.WriteLine("targetRectangle " + targetRectangle);
            }
            if (target != Vector2.Zero)
            {
                targetRectangle = new RectangleF(target, new Size2(32, 32));
               //Debug.WriteLine("targetRectangle " + targetRectangle);
            }
            else
            {
                targetRectangle = (RectangleF)targetObject.Bounds;
                targetRectangle = new RectangleF(target, new Size2(32, 32));
            }
           //Debug.WriteLine("targetRectangle " + targetRectangle);

            string[] tilemap = new string[tiledMap.Height];
            bool isPlayerSet = false;
            bool isEnemySet = false;
            RectangleF aaa = (RectangleF)Bounds;
            if (targetRectangle != null)
                for (int j = 0; j < tiledMap.Height; j++)
                    for (int i = 0; i < tiledMap.Width; i++)
                    {
                        if (entities != null)
                        {
                            bool a = false;
                            for (int i1 = 0; i1 < entities.Count; i1++)
                            {
                                a = false;
                                Sprite entity = (Sprite)entities[i1];
                                if (aaa.Intersects(new RectangleF((i * 32), (j * 32) + 2, 32, 32)) && !isEnemySet)

                                {
                                    isEnemySet = true;
                                    tilemap[j] += "A";

                                    a = true;
                                    break;
                                }
                                if (targetRectangle.Intersects(new RectangleF((i * 32), j * 32, 32, 32)) && !isPlayerSet)
                                {
                                    tilemap[j] += "B"; i++;
                                    isPlayerSet = true;
                                    a = true;
                                    break;
                                }
                                else
                                if (entity.rectParams.Intersects(new RectangleF((i * 32), j * 32, 16, 16)))
                                {
                                    if ((entity as Sprite).SpriteType == SpriteType.Wall)
                                    {
                                        tilemap[j] += "|";
                                        a = true;
                                    }
                                    //if (!isPlayerSet)
                                    //{

                                    //    tilemap[j] += "B";
                                    //    a = true;
                                    //    isPlayerSet = true;
                                    //}
                                    if (!a)
                                    {
                                        tilemap[j] += " "; a = true;
                                    }
                                    break;
                                }
                            }
                            if (!a)
                            {
                                tilemap[j] += " ";
                            }
                        }
                    }
            return tilemap;
        }



        private void SetNewPosition()
        {
            double lowest_price = double.MaxValue;
            counter = 0;
            foreach (var a in AvoidancePoints)
            {
                var o = new Vector2(Bounds.Position.X, Bounds.Position.Y);

                if (Vector2.Distance(o, a.ToVector2()) < 32)
                    a.id = 0;
                if (a != null && a.id != 0)
                    if (Vector2.Distance(a.ToVector2(), o) <= lowest_price)
                    {
                        lowest_price = Vector2.Distance(a.ToVector2(), o);
                        LastPosition = (a.ToVector2());
                        counter++;
                    }
            }
        }
        int counter = 0;
        private void Move(GameTime gameTime, List<IEntity> entities)
        {

            Vector2 moveDir1 = Vector2.Zero;
            if (Money < 1||tired)
            {
                moveDir1 = ExitLocation - new Vector2(Bounds.Position.X, Bounds.Position.Y);
                moveDir1.Normalize();
                Origin += moveDir1 * ((Speed) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (Vector2.Distance(Bounds.Position, ExitLocation) < 25)
                {
                    IsRemoved = true;
                }
                if (moveDir1 != Vector2.Zero)
                    if (moveDir1.X > 0)
                    {
                        facing = Facing.Right;
                        Effect = SpriteEffects.None;
                    }
                    else
                    {
                        facing = Facing.Left;
                        Effect = SpriteEffects.FlipHorizontally;
                    }
            }
            else
            {


                if (counter == 0 && targetObject != null)
                {



                    if (Vector2.Distance(Bounds.Position, LastPosition) < 1)
                    {
                        (targetObject as Arcade).Parent = this;
                        Playing = true;
                        Effect = (targetObject as Arcade).Effect;
                    }
                    else
                    if (Vector2.Distance(Bounds.Position, LastPosition) < 25)
                    {

                        moveDir1 = LastPosition - new Vector2(Bounds.Position.X, Bounds.Position.Y);
                        moveDir1.Normalize();
                        Origin += moveDir1 * ((Speed) * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    }
                    else
                    {
                        moveDir1 = LastPosition - Origin;
                        moveDir1.Normalize();
                        Origin += moveDir1 * ((Speed) * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    }


                }
                else
                {
                    moveDir1 = LastPosition - Origin;
                    moveDir1.Normalize();
                    Origin += moveDir1 * ((Speed) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }


                if (moveDir1 != Vector2.Zero)
                    if (moveDir1.X > 0)
                    {
                        facing = Facing.Right;
                        Effect = SpriteEffects.None;
                    }
                    else
                    {
                        facing = Facing.Left;
                        Effect = SpriteEffects.FlipHorizontally;
                    }



            }

        }
        private static List<Tile> GetWalkableTiles(List<string> map, Tile currentTile, Tile targetTile)
        {
            var possibleTiles = new List<Tile>()
            {
                new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
                new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            };

            possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

            var maxX = map.First().Length - 2;
            var maxY = map.Count - 1;
            try
            {
                return possibleTiles
                   .Where(tile => tile.X >= 0 && tile.X <= maxX)
                   .Where(tile => tile.Y >= 0 && tile.Y <= maxY)
                   .Where(tile => map[tile.Y][tile.X] == ' ' || map[tile.Y][tile.X] == 'B')
                   .ToList();
            }
            catch
            {

                return null;
            }

        }

        static List<string> PathSearch(List<string> map)
        {

            var start = new Tile();
            int c = 0;
            for (int i = 0; i < map.Count; i++)
            {
                if (map[i].Contains("A"))
                    c++;
                if (map[i].Contains("B"))
                    c++;
            }
            if (c >= 2)
            {

                start.Y = map.FindIndex(x => x.Contains("A"));
                start.X = map[start.Y].IndexOf("A");


                var finish = new Tile();
                finish.Y = map.FindIndex(x => x.Contains("B"));
                finish.X = map[finish.Y].IndexOf("B");

                start.SetDistance(finish.X, finish.Y);

                var activeTiles = new List<Tile>();
                activeTiles.Add(start);
                var visitedTiles = new List<Tile>();

                while (activeTiles.Any())
                {
                    var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

                    if (checkTile.X == finish.X && checkTile.Y == finish.Y)
                    {
                        //We found the destination and we can be sure (Because the the OrderBy above)
                        //That it's the most low cost option. 
                        var tile = checkTile;
                        Console.WriteLine("Retracing steps backwards...");
                        while (true)
                        {
                            Console.WriteLine($"{tile.X} : {tile.Y}");
                            if (map[tile.Y][tile.X] == ' ')
                            {
                                var newMapRow = map[tile.Y].ToCharArray();
                                newMapRow[tile.X] = '*';
                                map[tile.Y] = new string(newMapRow);
                            }
                            tile = tile.Parent;
                            if (tile == null)
                            {
                                Console.WriteLine("Map looks like :");
                                map.ForEach(x => Console.WriteLine(x));
                                Console.ReadLine();
                                return map;
                                //Console.WriteLine("Done!");

                                //return;
                            }
                        }
                    }

                    visitedTiles.Add(checkTile);
                    activeTiles.Remove(checkTile);

                    var walkableTiles = GetWalkableTiles(map, checkTile, finish);
                    if (walkableTiles == null)
                    {

                        return null;
                    }
                    else
                        foreach (var walkableTile in walkableTiles)
                        {
                            //We have already visited this tile so we don't need to do so again!
                            if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                                continue;

                            //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                            if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                            {
                                var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                                if (existingTile.CostDistance > checkTile.CostDistance)
                                {
                                    activeTiles.Remove(existingTile);
                                    activeTiles.Add(walkableTile);
                                }
                            }
                            else
                            {
                                //We've never seen this tile before so add it to the list. 
                                activeTiles.Add(walkableTile);
                            }
                        }
                }

            }
            return null;
        }


        private float _inserCoinTimer = 0;
        private bool ableToInserCoin = false;
        private int ableToInsertCoinInt = -1;
        public TimeSpan inserCoinCoolDown = TimeSpan.FromSeconds(2);
        private readonly Random Random = new Random();
        public void InserCoinCoolDown(GameTime gameTime, List<IEntity> entities)
        {
            _inserCoinTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ableToInsertCoinInt != Convert.ToInt32(_inserCoinTimer))
            {
                if (Convert.ToInt32(_inserCoinTimer) % inserCoinCoolDown.TotalSeconds == 0)
                {
                    ableToInsertCoinInt = Convert.ToInt32(_inserCoinTimer);
                    ableToInserCoin = true;
                    if (Random.Next(20) == 15)
                    {
                        tired = true;
                        (targetObject as Arcade).Parent = null;
                        targetObject = null;
                        Playing = false;
                    }

                }
            }
        }

        private float _attackTimer = 0;
        private bool ableToShoot = false;
        private int ableToShootInt = -1;
        public TimeSpan dirCoolDown = TimeSpan.FromSeconds(3.5f);
        public void NewDirCoolDown(GameTime gameTime, List<IEntity> entities)
        {
            _attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ableToShootInt != Convert.ToInt32(_attackTimer))
            {
                if (Convert.ToInt32(_attackTimer) % dirCoolDown.TotalSeconds == 0)
                {
                    ableToShootInt = Convert.ToInt32(_attackTimer);
                    ableToShoot = true;
                    if (targetObject == null)
                    {
                        NewDeriction();
                    }
                    if (Random.Next(10) > 3)
                    {
                        List<Arcade> arcades = new List<Arcade>();
                        foreach (Sprite item in entities)
                        {
                            if (item.SpriteType == SpriteType.Arcade)
                            {
                                if (Money < 2)
                                {
                                    tired = true;
                                    if (targetObject != null)
                                        (targetObject as Arcade).Parent = null;
                                    targetObject = null;
                                    Playing = false;
                                    Money = 0;
                                    break;
                                }
                                else
                                if (item.Parent == null && targetObject == null)
                                {
                                    arcades.Add((Arcade)item);
                                }
                            }
                        }
                        if (arcades.Count() != 0)
                        {
                            var arcade = arcades[Random.Next(arcades.Count())];
                            arcade.Parent = this;
                            targetObject = arcade;
                            AvoidancePoints.Clear();
                        }

                    }

                }
            }

        }
        Vector2 target = Vector2.Zero;
        private void NewDeriction()
        {
            AvoidancePoints.Clear();
            do
            {
                target = new Vector2(Random.Next(32, tiledMap.WidthInPixels - 96), Random.Next(32, tiledMap.HeightInPixels - 32));

            } while (Random.NextDouble() < 0.5);


            if (targetObject != null)
            {
                if (target != new Vector2(targetObject.Bounds.Position.X, targetObject.Bounds.Position.Y))
                {
                    target = targetObject.Bounds.Position;
                }

            }
            //if ( == 1)
            //{
            //    if (Random.Next(0, 2) == 1)
            //        LastPosition = new Vector2(Bounds.Position.X, Bounds.Position.Y + 96);
            //    else
            //        LastPosition = new Vector2(Bounds.Position.X, Bounds.Position.Y - 96);
            //}
            //else
            //{
            //    if (Random.Next(0, 2) == 1)
            //        LastPosition = new Vector2(Bounds.Position.X + 96, Bounds.Position.Y);
            //    else
            //        LastPosition = new Vector2(Bounds.Position.X - 96, Bounds.Position.Y);
            //}
        }

        private void ObstacleAvoidance(GameTime gameTime, List<IEntity> entities)
        {

            if (AvoidancePoints == null || AvoidancePoints.Count < 1)
            {
                string[] logFile = GetMapArray(entities, target);
                if (logFile.Length > 0)
                {
                    var logList = new List<string>(logFile);
                    int j = 0;
                    var pathSearch = PathSearch(logList);
                    if (pathSearch != null)
                        foreach (var str in pathSearch.ToArray())
                        {
                            for (int i = 0; i < str.Length; i++)
                            {
                                if (str[i] == '*')
                                {
                                    AvoidancePoints.Add(new PointN((i * 32) + 16, (j * 32) - 16 + 32, 1));
                                }
                            }
                            j++;
                        }
                    SetNewPosition();
                }
            }
            else
            {
                if (Vector2.Distance(Origin, LastPosition) < 32 || LastPosition == Vector2.Zero)
                {
                    SetNewPosition();
                }
                Move(gameTime, entities);
                int counter = 0;
                for (int i = 0; i < AvoidancePoints.Count; i++)
                {
                    counter += AvoidancePoints[i].id;
                }
                if (counter < 3 && targetObject == null)
                {

                    NewDeriction();

                }

                if (counter < 3 && targetObject != null)
                {


                    //NewDeriction();


                    //if (Vector2.Distance(targetObject.Bounds.Position, Bounds.Position) < 64)

                    //{
                    //    if (targetObject.Effect == SpriteEffects.None)
                    //        LastPosition = new Vector2(targetObject.Bounds.Position.X - 32, targetObject.Bounds.Position.Y);
                    //    else
                    //        LastPosition = new Vector2(targetObject.Bounds.Position.X + 32, targetObject.Bounds.Position.Y);
                    //}
                    //else
                    //{
                    //    NewDeriction();
                    //}
                    //foreach (var item in AvoidancePoints)
                    //{
                    //    item.id = 0;
                    //}
                    //List<Arcade> arcades = new List<Arcade>();
                    //if (targetObject == null)
                    //{
                    //    foreach (Sprite item in entities)
                    //    {
                    //        if (item.SpriteType == SpriteType.Arcade && item.Parent == null)
                    //        {
                    //            arcades.Add((Arcade)item);

                    //        }
                    //    }
                    //    targetObject = arcades[Random.Next(arcades.Count)];
                    //}


                }
                else
                    if (/* && */targetObject != null)
                {
                    // NewDeriction();
                }
            }
        }
        float counterDrawMoney = 0;
        private bool tired = false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            //foreach (var item in AvoidancePoints)
            //{
            //    spriteBatch.DrawRectangle(new RectangleF((float)item.x, (float)item.y, 32, 32), Color.White);
            //}

            if (Playing && ableToInserCoin && !tired)
            {
                if (counterDrawMoney == 0)
                {

                    (targetObject as Arcade).Bank += (targetObject as Arcade).CoinPrice;
                }
                spriteBatch.DrawString(SpriteFont, "+$" + (targetObject as Arcade).CoinPrice, new Vector2(Bounds.Position.X, Bounds.Position.Y - 36 + counterDrawMoney),
                    Color.FromNonPremultiplied((int)(0 - counterDrawMoney * 10), (int)(0 - counterDrawMoney * 10), (int)(0 - counterDrawMoney * 10), (int)(255 + counterDrawMoney * 10)));
                counterDrawMoney -= 0.5f;

                if (counterDrawMoney < -25)
                {

                    ableToInserCoin = false;
                    counterDrawMoney = 0;
                    if (Money - (targetObject as Arcade).CoinPrice >= 0)
                    {
                        Money -= (targetObject as Arcade).CoinPrice;
                        inserCoinCoolDown = TimeSpan.FromSeconds(Random.Next(6, 15));

                    }
                    else
                    {
                        tired = true;
                        (targetObject as Arcade).Parent = null;
                        targetObject = null;
                        Playing = false;
                    }
                }

            }
            if (Playing)
            {
                spriteBatch.Draw(Texture2D, new Vector2(rectParams.Position.X - 16, rectParams.Position.Y - 32), null, Color.White, 0, Vector2.Zero, 2, Effect, 0);

            }
            else
            {
                if (facing == Facing.Right)
                    spriteBatch.Draw(Texture2D, new Vector2(rectParams.Position.X - 16, rectParams.Position.Y - 32), null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(Texture2D, new Vector2(rectParams.Position.X - 16, rectParams.Position.Y - 32), null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.FlipHorizontally, 0);

            }


            // base.Draw(spriteBatch);
        }
    }
    internal class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public Tile Parent { get; set; }

        //The distance is essentially the estimated distance, ignoring walls to our target. 
        //So how many tiles left and right, up and down, ignoring walls, to get there. 
        public void SetDistance(int targetX, int targetY)
        {
            this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
        }
    }
    class PointN
    {

        public double x, y;
        public int id;

        public PointN(double newX, double newY, int newId)
        {

            x = newX;
            y = newY;
            id = newId;
        }

        internal Vector2 ToVector2()
        {
            return new Vector2((float)x, (float)y);
        }
    }
}
