using farm.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace farm
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Vector2 _worldPosition;
        Vector2 Square;
        private Vector2 _mousePoint;
        Size2 size = new Size2(32, 32);
        SpriteFont _font;
        MouseState mouseState;
        private OrthographicCamera _camera;
        private Matrix transformMatrix;
        private DisplayMode Display;
        Texture2D blackLines;
        Texture2D AM;
        List<IEntity> Entities;
        Shop Shop;
        IEntity _peekedEntity;
        Player Player;
        Charecter Charecter;

        TiledMap _tiledMap;
        TiledMapRenderer _tiledMapRenderer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Entities = new List<IEntity>();
            Shop = new Shop();
            Player = new Player();
            Player.Money = 1000;
        }
        RectangleF RectangleF;
        private int _peekedArcadeId;
        private KeyboardState _previousKey;
        private KeyboardState _currentKey;
        private MouseState _previousMouseKey;
        private MouseState _currentMouseKey;
        Color _peekColor;
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;//MonoGame.Extended.Screens.Screen.AllScreens[0]; // Change 0 for other screens


            _graphics.PreferredBackBufferWidth = Display.Width;
            _graphics.PreferredBackBufferHeight = Display.Height;

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            _camera = new OrthographicCamera(viewportAdapter)
            {
                // Position = new Vector2(-Display.Width / 2, -Display.Height / 2)
            };


            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            _tiledMap = Content.Load<TiledMap>("tilemap");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
            _camera.Position = new Vector2((-_graphics.PreferredBackBufferWidth / 2 + _tiledMap.HeightInPixels / 2) - 32, (-_tiledMap.WidthInPixels / 2) - 32);
            _camera.Position = new Vector2(_camera.Position.X - _camera.Position.X % 32, (_camera.Position.Y - _camera.Position.Y % 32) - 14);
            RectangleF.Position = new Point2(_camera.Center.X - _tiledMap.WidthInPixels / 2, _camera.Position.Y);
            RectangleF.Position = new Point2((RectangleF.Position.X - RectangleF.Position.X % 32) + 32, (RectangleF.Position.Y - RectangleF.Position.Y % 32) + 128 + 128 + 32);
            RectangleF.Width = _tiledMap.WidthInPixels - 64;
            RectangleF.Height = _tiledMap.HeightInPixels - 128;
            _camera.Zoom = 2;
            TiledMapObject[] walls = _tiledMap.GetLayer<TiledMapObjectLayer>("walls").Objects;

            foreach (var en in walls)
            {
                string type;
                en.Properties.TryGetValue("type", out type);


                if (type == "wall")
                {
                    Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
                    var wall = new Wall(new RectangleF(en.Position.X, ((en.Position.Y) - en.Size.Height), en.Size.Width, en.Size.Height), texture);
                    Entities.Add(wall);
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            blackLines = Content.Load<Texture2D>("BlackLines");
            _font = Content.Load<SpriteFont>("font");
            AM = Content.Load<Texture2D>("AM_1");
            Shop.Entities.Add(new Arcade(new RectangleF(Square, size), AM) { Id = 1, Name = "Arcade_1", CoinPrice = 5, Cost = 150 });
            AM = Content.Load<Texture2D>("AM_2");
            Shop.Entities.Add(new Arcade(new RectangleF(Square, size), AM) { Id = 2, Name = "Arcade_2", CoinPrice = 15, Cost = 250 });
            AM = Content.Load<Texture2D>("AM_3");
            Shop.Entities.Add(new Arcade(new RectangleF(Square, size), AM) { Id = 3, Name = "Arcade_3", CoinPrice = 20, Cost = 400 });
            AM = Content.Load<Texture2D>("wall_1");
            Shop.Entities.Add(new Wall(new RectangleF(Square, size), AM) { Id = 4, Name = "Wall ", Cost = 15 });
            Shop.Position = new Vector2(RectangleF.Position.X + _tiledMap.HeightInPixels - 96, RectangleF.Position.Y + 32);
            Shop.Init();
            _peekedArcadeId = 1;

            // TODO: use this.Content to load your game content here
        }
        RectangleF MeneRectangle = new RectangleF();
        RectangleF SubMeneRectangle1 = new RectangleF();
        RectangleF SubMeneRectangle2 = new RectangleF();
        RectangleF SubMeneRectangle3 = new RectangleF();


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            SpawnCoolDown(gameTime, Entities);
            mouseState = Mouse.GetState();
            _worldPosition = _camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));
            Square = new Vector2((_worldPosition.X - _worldPosition.X % 32), _worldPosition.Y - _worldPosition.Y % 32);
            _peekColor = Color.Black;
            _mousePoint = new Vector2(_worldPosition.X, _worldPosition.Y);


            if (ShowMenu)
            {
                MeneRectangle.Position = new Vector2(ShowMenuPosition.X - 64 - 16, ShowMenuPosition.Y - 96);
                MeneRectangle.Size = new Size2(96 * 2, 32 * 2);
                SubMeneRectangle1.Position = new Vector2(ShowMenuPosition.X - 64 - 16, ShowMenuPosition.Y - 96);
                SubMeneRectangle3.Size = SubMeneRectangle2.Size = SubMeneRectangle1.Size = new Size2(64, 64);
                SubMeneRectangle2.Position = new Vector2(ShowMenuPosition.X - 64 - 16 + 64, ShowMenuPosition.Y - 96);
                SubMeneRectangle3.Position = new Vector2(ShowMenuPosition.X - 64 - 16 + 64 + 64, ShowMenuPosition.Y - 96);
            }
            else
            {


                if (_peekedEntity != null && (_peekedEntity as Sprite).SpriteType == SpriteType.Wall)
                {
                    foreach (Sprite item in Entities)
                    {
                        _peekColor = Color.Yellow;

                        if (item.FromPlayer)
                        {
                            RectangleF rect1 = new RectangleF(new Vector2(Square.X, Square.Y), new Size2(32, 32));
                            if (item.rectParams.Intersects(rect1) || item.Bounds.Intersects(_peekedEntity.Bounds))
                            {
                                _peekColor = Color.Red;
                                break;
                            }
                        }

                    }
                }

                if (_peekedEntity != null && (_peekedEntity as Sprite).SpriteType == SpriteType.Arcade)
                {
                    Shop.Entities.Where(x => x.Id == _peekedEntity.Id).FirstOrDefault().Bounds.Position = new Point2(Square.X, Square.Y);
                    Shop.Entities.Where(x => x.Id == _peekedEntity.Id).FirstOrDefault().rectParams = new RectangleF(new Point2(Square.X, Square.Y), size);
                    foreach (Sprite item in Entities)
                    {

                        RectangleF rect1 = new RectangleF(new Vector2(Square.X, Square.Y), new Size2(32, 32));

                        if (item.SpriteType == SpriteType.Wall)
                        {
                            if (item.rectParams.Intersects(rect1) || item.Bounds.Intersects(_peekedEntity.Bounds))
                            {
                                _peekColor = Color.Red;
                                break;
                            }
                            if (item.Bounds.Position.X - item.Bounds.Position.X % 32 == Square.X + 32 && item.Bounds.Position.Y - 8 < Square.Y && item.rectParams.Height + item.rectParams.Y > Square.Y)
                            {
                                _peekColor = Color.Yellow;
                                _peekedEntity.Effect = SpriteEffects.None;
                                break;
                            }
                            else if (item.Bounds.Position.X - item.Bounds.Position.X % 32 == Square.X - 32 && item.Bounds.Position.Y - 8 < Square.Y && item.rectParams.Height + item.rectParams.Y > Square.Y)
                            {
                                _peekColor = Color.Yellow;
                                _peekedEntity.Effect = SpriteEffects.FlipHorizontally;
                                break;
                            }
                            else
                            {

                                _peekColor = Color.Red;
                            }
                        }





                    }
                    foreach (Sprite item in Entities)
                    {
                        if ((item as Sprite).SpriteType == SpriteType.Arcade && item != _peekedEntity)
                        {

                            RectangleF rect1 = new RectangleF(new Vector2(Square.X - 32, Square.Y - 32), new Size2(96, 96));
                            // RectangleF rect1 = new RectangleF(new Vector2(Square.X , Square.Y), new Size2(32,32));

                            if (item.Bounds.Intersects(rect1))
                            {

                                _peekColor = Color.Red;
                                break;
                            }

                        }
                    }


                }
                else
                {
                    foreach (Sprite item in Entities)
                    {
                        if (item.SpriteType.ToString() == SpriteType.Wall.ToString()&& item.FromPlayer)
                        {
                            RectangleF rect1 = new RectangleF(new Vector2(Square.X, Square.Y), new Size2(32, 32));
                            if (item.Bounds.Intersects(rect1))
                            {
                                _peekColor = Color.Wheat;
                                if (_currentMouseKey.RightButton == ButtonState.Pressed && _previousMouseKey.RightButton == ButtonState.Released)
                                {
                                    ShowMenu = true;
                                    ShowMenuPosition = item.Bounds.Position;
                                    ShowMenuEntity = item;

                                }
                                break;
                            }
                        }
                        if (item.SpriteType.ToString() == SpriteType.Arcade.ToString())
                        {

                            RectangleF rect1 = new RectangleF(new Vector2(Square.X, Square.Y), new Size2(32, 32));
                            if (item.Bounds.Intersects(rect1))
                            {

                                _peekColor = Color.Wheat;
                                if (_currentMouseKey.LeftButton == ButtonState.Pressed && _previousMouseKey.LeftButton == ButtonState.Released)
                                {
                                    Player.Money += (item as Arcade).Bank;
                                    (item as Arcade).Bank = 0;
                                    ShowMenu = false;
                                }
                                if (_currentMouseKey.RightButton == ButtonState.Pressed && _previousMouseKey.RightButton == ButtonState.Released)
                                {
                                    ShowMenu = true;
                                    ShowMenuPosition = item.Bounds.Position;
                                    ShowMenuEntity = item;

                                }
                                break;
                            }
                        }
                    }
                }
                foreach (Sprite item in Entities)
                {
                    item.Update(gameTime, Entities);
                }

            }
            Input(gameTime);

            PostUpdate();
            base.Update(gameTime);
        }


        private void PostUpdate()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if ((Entities[i] as Sprite).IsRemoved)
                {
                    Entities.RemoveAt(i);
                    i--;
                }
            }
        }

        #region spawner

        private float _spawnTimer = 0;
        private bool ableToSpawn = false;
        private int ableToSpawnInt = -1;
        public TimeSpan spawnCoolDown = TimeSpan.FromSeconds(8);
        private readonly Random Random = new Random();

        public bool ShowMenu { get; private set; }
        public Vector2 ShowMenuPosition { get; private set; }
        public Sprite ShowMenuEntity { get; private set; }
        public Color SubMenuColor1 { get; private set; }
        public Color SubMenuColor2 { get; private set; }
        public Color SubMenuColor3 { get; private set; }

        public void SpawnCoolDown(GameTime gameTime, List<IEntity> entities)
        {
            _spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ableToSpawnInt != Convert.ToInt32(_spawnTimer))
            {
                if (Convert.ToInt32(_spawnTimer) % spawnCoolDown.TotalSeconds == 0)
                {
                    ableToSpawnInt = Convert.ToInt32(_spawnTimer);
                    ableToSpawn = true;
                    Spawner(gameTime, entities);
                }
            }
        }
        void Spawner(GameTime gameTime, List<IEntity> entities)
        {
            foreach (Sprite item in entities.ToArray())
            {
                if (item.SpriteType == SpriteType.Arcade && item.Parent == null)
                {

                    AM = Content.Load<Texture2D>("C_1");
                    Charecter = new Charecter(new RectangleF(256, 416 + 64, 32, 32), AM, _font);
                    Charecter.ExitLocation = new Vector2(256, 416 + 64);
                    // Charecter = new Charecter(new RectangleF(128, 128, 32, 32), AM, _font);
                    Charecter.tiledMap = _tiledMap;
                    Entities.Add(Charecter);
                    // Charecter.targetObject = item;
                    //item.Parent = Charecter;
                    Charecter.LastPosition = new Vector2(224, 352);
                    ableToSpawn = false;
                    spawnCoolDown = TimeSpan.FromSeconds(Random.Next(8, 30));
                    break;

                }
            }


        }
        #endregion


        private void Input(GameTime gameTime)
        {
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();
            _previousMouseKey = _currentMouseKey;
            _currentMouseKey = Mouse.GetState();
            Random random = new Random();


            if (ShowMenu && MeneRectangle.Intersects(new RectangleF(_mousePoint, new Size2(1, 1))))
            {
                SubMenuColor1 = Color.White;
                SubMenuColor2 = Color.White;
                SubMenuColor3 = Color.White;
                if (SubMeneRectangle1.Intersects(new RectangleF(_mousePoint, new Size2(1, 1))))
                {
                    SubMenuColor1 = Color.Yellow;
                    if (_currentMouseKey.LeftButton == ButtonState.Pressed && _previousMouseKey.LeftButton == ButtonState.Released)
                    {

                        _peekedEntity = Shop.Entities.Where(x => (x as Sprite).Name == ShowMenuEntity.Name).FirstOrDefault();
                        Player.Money += ShowMenuEntity.Cost;
                        Entities.Remove(ShowMenuEntity);
                        ShowMenu = false;

                    }
                }
                else
                if (SubMeneRectangle2.Intersects(new RectangleF(_mousePoint, new Size2(1, 1))))
                {
                    SubMenuColor2 = Color.Yellow;
                    if (_currentMouseKey.LeftButton == ButtonState.Pressed && _previousMouseKey.LeftButton == ButtonState.Released)
                    {
                        Player.Money += ShowMenuEntity.Cost;
                        Entities.Remove(ShowMenuEntity);
                        ShowMenu = false;

                    }
                }
                else
                if (SubMeneRectangle3.Intersects(new RectangleF(_mousePoint, new Size2(1, 1))))
                {
                    if (_currentMouseKey.LeftButton == ButtonState.Pressed && _previousMouseKey.LeftButton == ButtonState.Released)
                    {
                        ShowMenu = false;

                    }
                    SubMenuColor3 = Color.Yellow;
                }

            }
            else

            if (RectangleF.Intersects(new RectangleF(_mousePoint, new Size2(1, 1))))
            {
                if (_peekedEntity != null && _peekColor == Color.Yellow /*&& (_peekedEntity as Sprite).SpriteType == SpriteType.Arcade*/)
                {
                    if (_currentMouseKey.LeftButton == ButtonState.Pressed && _previousMouseKey.LeftButton == ButtonState.Released &&
                        Player.Money - (_peekedEntity as Sprite).Cost >= 0)
                    {

                        switch ((_peekedEntity as Sprite).SpriteType)
                        {
                            case SpriteType.Wall:
                                var wall = new Wall(_peekedEntity.rectParams, (_peekedEntity as Sprite).Texture2D);
                                (wall as Wall).Bounds.Position = Square;
                                (wall as Wall).SpriteType = SpriteType.Wall;
                                (wall as Wall).Cost = (_peekedEntity as Wall).Cost;
                                wall.Name = (_peekedEntity as Wall).Name;
                                (wall as Wall).FromPlayer = true;
                                wall.Effect = _peekedEntity.Effect;

                                Player.Money -= (_peekedEntity as Wall).Cost;
                                bool btmp = false;
                                foreach (Sprite item in Entities)
                                {
                                    var tmp = new RectangleF(new Point2(wall.Bounds.Position.X, wall.Bounds.Position.Y - 32), wall.rectParams.Size);
                                    if (item.SpriteType == SpriteType.Wall)
                                    {
                                        if (tmp.Intersects(item.Bounds))
                                        {

                                            Entities.Insert(Entities.IndexOf(item) + 1, (IEntity)wall);
                                            //Debug.WriteLine("Sex");
                                            btmp = true;
                                            break;
                                        }
                                    }
                                }
                                if (!btmp)
                                    Entities.Insert(0, (IEntity)wall);
                                _peekedEntity = null;

                                break;
                            case SpriteType.Arcade:
                                //a.Id = random.Next(1000);

                                Arcade arcade = new Arcade(_peekedEntity.rectParams, (_peekedEntity as Sprite).Texture2D); //(_peekedEntity as Arcade).Clone();
                                arcade.Color = Color.White;
                                arcade.Name = (_peekedEntity as Arcade).Name;
                                arcade.Bounds.Position = Square;
                                arcade.SpriteType = SpriteType.Arcade;
                                arcade.CoinPrice = (_peekedEntity as Arcade).CoinPrice;
                                arcade.Cost = (_peekedEntity as Arcade).Cost;
                                arcade.Effect = _peekedEntity.Effect;
                                arcade.FromPlayer = true;
                                Player.Money -= (_peekedEntity as Arcade).Cost;
                                Entities.Add((IEntity)arcade);
                                _peekedEntity = null;
                                break;
                            default:
                                break;
                        }


                    }
                }

                if (_peekedEntity != null)
                    if (Player.Money - (_peekedEntity as Sprite).Cost < 0)
                    {
                        _peekedEntity = null;
                    }
            }
            else


            if (Shop.nextButton.Intersects(new RectangleF(_mousePoint, new Size2(2, 2))))
            {
                Shop.nextButtonColor = Color.Yellow;
                if (_currentMouseKey.LeftButton == ButtonState.Pressed && _previousMouseKey.LeftButton == ButtonState.Released)
                    Shop.NextClick();
            }
            else
            {
                Shop.nextButtonColor = Color.Black;
            }
            if (Shop.prevButton.Intersects(new RectangleF(_mousePoint, new Size2(2, 2))))
            {
                Shop.prevButtonColor = Color.Yellow;
                if (_currentMouseKey.LeftButton == ButtonState.Pressed && _previousMouseKey.LeftButton == ButtonState.Released)
                    Shop.PrevClick();
            }
            else
            {
                Shop.prevButtonColor = Color.Black;
            }

            foreach (var item in Shop.itemCards)
            {
                if (item.Rectangle.Intersects(new RectangleF(_mousePoint, new Size2(2, 2))))
                {

                    if ((item.GroupName).ToString() == Shop.currentSpriteType.ToString())
                    {
                        item.Color = Color.Yellow;
                        if (_currentMouseKey.LeftButton == ButtonState.Pressed && _previousMouseKey.LeftButton == ButtonState.Released)
                        {
                            item.Color = Color.White;
                            _peekedEntity = Shop.Entities.Where(x => x.Id == item.Id).FirstOrDefault();
                            //Debug.WriteLine(_peekedEntity);

                        }
                    }



                }
                else
                {
                    item.Color = Color.Black;



                }
            }


        }

        protected override void Draw(GameTime gameTime)
        {

            transformMatrix = _camera.GetViewMatrix();
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0, 150, 150, 255));

            //  _tiledMapRenderer.
            _tiledMapRenderer.Draw(0, transformMatrix);
            _tiledMapRenderer.Draw(1, transformMatrix);
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);
            if (RectangleF.Intersects(new RectangleF(_mousePoint, new Size2(1, 1))))
            {
                _spriteBatch.DrawRectangle(Square, size, _peekColor, thickness: 2);
                if (_peekedEntity != null)
                    Shop.Entities.Where(x => x.Id == _peekedEntity.Id).FirstOrDefault().Draw(_spriteBatch);
            }

            DrawShop();


            foreach (Sprite item in Entities)
            {
                if (item.SpriteType == SpriteType.Charecter)
                    item.Draw(_spriteBatch);
            }
            foreach (Sprite item in Entities)
            {
                if (item.SpriteType != SpriteType.Charecter)
                    item.Draw(_spriteBatch);
            }

            Shop.Draw(_spriteBatch, _font);

            _spriteBatch.DrawRectangle(RectangleF, Color.Black, 2);
            _spriteBatch.DrawString(_font, "$" + Player.Money, new Vector2(RectangleF.X - 128, RectangleF.Y /*- 32*/), Color.Black);
            _spriteBatch.End();

            _tiledMapRenderer.Draw(2, transformMatrix);



            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(blackLines, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), null, Color.White);

            _spriteBatch.End();
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

            if (ShowMenu)
            {
                Texture2D texture2D = Content.Load<Texture2D>("menu");
                _spriteBatch.Draw(texture2D, new Vector2(ShowMenuPosition.X - 64 - 16, ShowMenuPosition.Y - 96), Color.White);
                //_spriteBatch.DrawRectangle(MeneRectangle, Color.Black);
                //_spriteBatch.DrawRectangle(SubMeneRectangle1, SubMenuColor1);
                //_spriteBatch.DrawRectangle(SubMeneRectangle2, SubMenuColor2);
                //_spriteBatch.DrawRectangle(SubMeneRectangle3, SubMenuColor3);
            }
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void DrawShop()
        {

        }
    }
}
