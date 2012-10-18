using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Brian_s_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldState;
        MouseState mouse;
        SpriteFont lucidaConsole;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        int[,] board = new int[18, 13];//background board
        Object[,] objectboard = new Object[18, 13];//board for selectable items(ships, planets, etc.)
        List<Object> movelist = new List<Object>();
        List<Object> attackmovelist = new List<Object>();
        List<Ship> shiplist = new List<Ship>();
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            for (int i = 0; i < 18; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    board[i, j] = 0;
                    objectboard[i, j] = null;
                }
            }
            board[5, 8] = 1;           
            oldState = Keyboard.GetState();
            mouse = Mouse.GetState();
            int test = graphics.GraphicsDevice.Viewport.Width;
            int testx = graphics.GraphicsDevice.Viewport.Height;
            this.IsMouseVisible = true;
            lucidaConsole = Content.Load<SpriteFont>("SpriteFont1");
            selectedinfo = new info("", "", "");
            selected = false;
            Viewport v = new Viewport();
            tiled = 1;
            message = "";
            
            v.Height = graphics.GraphicsDevice.DisplayMode.Height;
            v.Width = graphics.GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
            graphics.GraphicsDevice.Viewport = v;
            graphics.ToggleFullScreen();
            base.Initialize();
        }
        Texture2D spacetiled;
        Texture2D space;
        Texture2D sun;
        Texture2D ship_tex;
        Texture2D ship1_tex;
        Vector2 shipPos;
        Ship ship;
        Ship ship2;
        Ship ship3;
        info selectedinfo;
        Object objectselected;
        string message;
        bool selected;
        int tiled;
        float angle;
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            shipPos.X = (2);
            shipPos.Y = (2);            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spacetiled = Content.Load<Texture2D>("space_emptytiled");
            space = Content.Load<Texture2D>("space_empty");
            sun = Content.Load<Texture2D>("star_red");
            ship_tex = Content.Load<Texture2D>("scoutship");
            ship1_tex = Content.Load<Texture2D>("ship1");
            shiplist.Add(ship = new Ship(1,1, shipPos, ship1_tex));
            shiplist.Add(ship2 = new Ship(2,2, new Vector2(4, 4), ship_tex));
            shiplist.Add(ship3 = new Ship(3,1, new Vector2(6, 6), ship1_tex));
            objectboard[4, 4] = ship2;
            objectboard[2, 2] = ship;
            objectboard[6, 6] = ship3;
            angle = 0;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        //gets the manhattan distance from one point to another
        public float ManhattanDist(Vector2 pos1, Vector2 pos2)
        {
            float resultx = 0;
            float resulty = 0;
            resultx = pos1.X - pos2.X;
            resulty = pos1.Y - pos2.Y;
            return Math.Abs(resultx) + Math.Abs(resulty);
        }
        //returns the direction vector from one point to another
        public Vector2 DirectionVector(Vector2 start, Vector2 end)
        {
            Vector2 result;
            result.X = end.X - start.X;
            result.Y = end.Y - start.Y;
            return result;
        }
        //if the space clicked is too far away change the destination to something inside the move range
        protected void NormalizeByMove(Ship newShip, Vector2 dest)
        {
            if (ManhattanDist(newShip.getPosition(), dest) <= newShip.getMove())
            {
                newShip.destination = dest;
                return;
            }
            if (newShip.getPosition().X > dest.X)
            {
                dest.X = dest.X + 1;
            }
            else if (newShip.getPosition().X < dest.X)
            {
                dest.X = dest.X - 1;
            }
            else
            {
                if (newShip.getPosition().Y > dest.Y)
                {
                    dest.Y = dest.Y + 1;
                }
                else if (newShip.getPosition().Y < dest.Y)
                {
                    dest.Y = dest.Y - 1;
                }
            }
            NormalizeByMove(newShip, dest);
            return;
        }
        //moves objects in the move list
        public void Move(GameTime gameTime)
        {
            if (movelist.Count != 0)
            {
                Ship temp = (Ship)movelist.ElementAt(0);
                if (temp.getPosition().X != temp.destination.X)
                {
                    if ((temp.getPosition().X < temp.destination.X - .02 | temp.getPosition().X > temp.destination.X + .02))
                    {
                        Vector2 direction = DirectionVector(temp.getPosition(), temp.destination);
                        temp.setPosition(temp.getPosition() + (direction * (float)gameTime.ElapsedGameTime.TotalSeconds));
                    }
                    else
                    {
                        temp.setPosition(temp.destination);
                        movelist.Remove(temp);
                    }
                }
                else
                {
                    if ((temp.getPosition().Y < temp.destination.Y - .02 | temp.getPosition().Y > temp.destination.Y + .02))
                    {
                        Vector2 direction = DirectionVector(temp.getPosition(), temp.destination);
                        temp.setPosition(temp.getPosition() + (direction * (float)gameTime.ElapsedGameTime.TotalSeconds));
                    }
                    else
                    {
                        temp.setPosition(temp.destination);
                        movelist.Remove(temp);
                    }
                }
            }
        }
        //attacks the target and moves the ships into position
        public void AttackMove(GameTime gameTime)
        {
            if (attackmovelist.Count != 0)
            {
                Ship self = (Ship)attackmovelist.ElementAt(0);
                Ship target = (Ship)objectboard[(int)self.destination.X, (int)self.destination.Y];
                if (target.team != self.team)//only attack if they are on the other side
                {
                    target.health = target.health - self.damage;
                }
                if (self.getPosition().X < self.destination.X && self.getPosition().Y == self.destination.Y)
                {
                    self.destination.X = self.destination.X - 1;
                }
                else if (self.getPosition().X > self.destination.X && self.getPosition().Y == self.destination.Y)
                {
                    self.destination.X = self.destination.X + 1;
                }
                else if (self.getPosition().X < self.destination.X && self.getPosition().Y > self.destination.Y)
                {
                    self.destination.X = self.destination.X - 1;
                    self.destination.Y = self.destination.Y + 1;
                }
                else if (self.getPosition().X > self.destination.X && self.getPosition().Y > self.destination.Y)
                {
                    self.destination.X = self.destination.X + 1;
                    self.destination.Y = self.destination.Y + 1;
                }
                else if (self.getPosition().X < self.destination.X && self.getPosition().Y < self.destination.Y)
                {
                    self.destination.X = self.destination.X - 1;
                    self.destination.Y = self.destination.Y - 1;
                }
                else if (self.getPosition().X > self.destination.X && self.getPosition().Y < self.destination.Y)
                {
                    self.destination.X = self.destination.X + 1;
                    self.destination.Y = self.destination.Y - 1;
                }
                else if (self.getPosition().Y < self.destination.Y && self.getPosition().X == self.destination.X)
                {
                    self.destination.Y = self.destination.Y - 1;
                }
                else if (self.getPosition().Y > self.destination.Y && self.getPosition().X == self.destination.X)
                {
                    self.destination.Y = self.destination.Y + 1;
                }
                attackmovelist.Remove(self);
                movelist.Add(self);
            }
        }
        //removes dead ships
        public void RemoveDead()
        {
            for (int i = 0; i < shiplist.Count; i++)
            {
                Ship inspect = shiplist[i];
                if (inspect.health <= 0)
                {
                    shiplist.Remove(inspect);
                    objectboard[(int)inspect.getPosition().X, (int)inspect.getPosition().Y] = null;
                }
            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (this.IsActive)
            {
                KeyboardState newstate = Keyboard.GetState();
                MouseState old = mouse;
                mouse = Mouse.GetState();
                //escape exits the game
                if (newstate.IsKeyDown(Keys.Escape))
                {
                    this.Exit();
                }
                //tiles the board with white lines so player can see the separate squares
                if (newstate.IsKeyDown(Keys.T))
                {
                    if (!oldState.IsKeyDown(Keys.T))
                    {
                        tiled = tiled * -1;
                    }
                }
                //A adjusts the angle, testing purposes
                if (newstate.IsKeyDown(Keys.A))
                {
                    angle += (float).1;
                }
                //handles mouse clicks
                if (mouse.RightButton == ButtonState.Released)
                {
                    if (old.RightButton != ButtonState.Released)
                    {
                        int x = mouse.X;
                        int y = mouse.Y;
                        x = x / 64;
                        y = y / 64;
                        //if there is something selected already(previous click hit something)
                        if (selected)
                        {
                            //if the new position clicked is empty
                            if (objectboard[x, y] == null)
                            {
                                Type type = objectselected.GetType();
                                if (type.Name == "Ship")
                                {
                                    Ship temp = (Ship)objectselected;
                                    //if the new position is within moving distance, move there
                                    if (ManhattanDist(temp.getPosition(), new Vector2(x, y)) <= temp.getMove())
                                    {
                                        temp.destination = new Vector2(x, y);
                                    }
                                    //else modify the move so that it is within the moving distance
                                    else
                                    {
                                        NormalizeByMove(temp, new Vector2(x, y));
                                    }
                                    //add the object to the move list, with the approriate destination
                                    movelist.Add(objectselected);
                                }
                            }
                            else //otherwise the space clicked contains an object
                            {
                                Type type = objectselected.GetType();
                                if (type.Name == "Ship")
                                {
                                    Ship temp = (Ship)objectselected;
                                    if (ManhattanDist(temp.getPosition(), new Vector2(x, y)) <= temp.getMove())
                                    {
                                        temp.destination = new Vector2(x, y);
                                        attackmovelist.Add(objectselected);
                                    }
                                    else
                                    {
                                        NormalizeByMove(temp, new Vector2(x, y));
                                        movelist.Add(objectselected);
                                    }                                    
                                }
                            }
                            selected = false;
                        }
                    }
                }
                    if (mouse.LeftButton == ButtonState.Released)
                    {
                        if (old.LeftButton != ButtonState.Released)
                        {
                            int x = mouse.X;
                            int y = mouse.Y;
                            x = x / 64;
                            y = y / 64;
                            //if there is something selected already(previous click got something
                            /*if (selected)
                            {
                                //if the new position clicked is empty
                                if (objectboard[x, y] == null)
                                {
                                    Type type = objectselected.GetType();
                                    if (type.Name == "Ship")
                                    {
                                        Ship temp = (Ship)objectselected;
                                        if (ManhattanDist(temp.getPosition(), new Vector2(x, y)) <= temp.getMove())
                                        {
                                            temp.destination = new Vector2(x, y);
                                        }
                                        else
                                        {
                                            NormalizeByMove(temp, new Vector2(x, y));
                                        }
                                        movelist.Add(objectselected);
                                    }
                                }
                                selected = false;
                            }*/
                            try
                            {
                                if (movelist.Count == 0)
                                {
                                    selectedinfo = objectboard[x, y].getInfo();
                                    objectselected = objectboard[x, y];
                                    selected = true;
                                }
                            }
                            catch (Exception e)
                            {
                                System.Console.WriteLine(e.Data);
                            }

                        }
                    }
                    for (int i = 0; i < 18; i++)
                    {
                        for (int j = 0; j < 13; j++)
                        {
                            objectboard[i, j] = null;
                        }
                    }
                    for (int compare = 0; compare < shiplist.Count; compare++)
                    {
                        objectboard[(int)shiplist[compare].getPosition().X, (int)shiplist[compare].getPosition().Y] = shiplist[compare];
                    }
                    oldState = newstate;
                }
                Move(gameTime);
                AttackMove(gameTime);
                RemoveDead();
                base.Update(gameTime);
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Maroon);
            
            for (int i = 0; i < 18; i++){
                for (int j = 0; j < 13; j++){
                    if (board[i,j] == 1){
                        Vector2 pos = new Vector2(i*64,j*64);
                        spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                        spriteBatch.Draw(sun, pos, Color.White);
                        spriteBatch.End();
                    }
                    else{
                        Vector2 pos = new Vector2(i*64, j*64);
                        spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                        if (tiled == 1)
                        {
                            spriteBatch.Draw(spacetiled, pos, Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(space, pos, Color.White);
                        }
                        spriteBatch.End();
                    }
                }
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                Vector2 displayPos = new Vector2(10, graphics.GraphicsDevice.DisplayMode.Height - 30);
                Vector2 descPos = new Vector2(110, graphics.GraphicsDevice.DisplayMode.Height - 30);
                Vector2 miscPos = new Vector2(400, graphics.GraphicsDevice.DisplayMode.Height - 30);
                Vector2 messagePos = new Vector2(graphics.GraphicsDevice.DisplayMode.Width - 200, graphics.GraphicsDevice.DisplayMode.Height - 30);
                if (selected)
                {
                    spriteBatch.DrawString(lucidaConsole, selectedinfo.name + ":", displayPos, Color.White);
                    spriteBatch.DrawString(lucidaConsole, selectedinfo.description, descPos, Color.White);
                    spriteBatch.DrawString(lucidaConsole, selectedinfo.misc, miscPos, Color.White);
                }
                spriteBatch.DrawString(lucidaConsole, message, messagePos, Color.White);
                //spriteBatch.Draw(ship.ship, ship.getPosition() * 64, Color.White);
                for (int x = 0; x < shiplist.Count; x++)
                {
                    spriteBatch.Draw(shiplist[x].ship, new Vector2(shiplist[x].getPosition().X * 64 + 32, shiplist[x].getPosition().Y * 64 + 32), null, Color.White, angle, new Vector2(32, 32), 1, SpriteEffects.None, 0);
                }
                spriteBatch.End();
            }
            // TODO: Add your drawing code here
            

            base.Draw(gameTime);
        }
    }
}
