﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using SnackAttack.Desktop;

namespace SnackAttack
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Color backgroundColor = Color.CornflowerBlue;

        Snake snake;
        Mice mice;

        Texture2D obstacle;
        Vector2 obstaclePos;
        BoundingBox obstacleBox;

        Texture2D pause;
        Vector2 pausePos;


        //these are just placeholders to test win condition 
        Texture2D mouse;
        Vector2 mousePos;
        BoundingBox mouseBox;

        TimeSpan timeSpan = TimeSpan.FromMilliseconds(31000); //30 sec in ms
        bool timeup = false;
        bool win = false;

        bool paused = false;

        KeyboardState currentKB, previousKB;

        private SpriteFont font;


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
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            float initialX = graphics.PreferredBackBufferWidth / 2; //get middle of the screen 
            float initialY = graphics.PreferredBackBufferHeight / 2;

            mouseBox = new BoundingBox();
            obstacleBox = new BoundingBox();
            snake = new Snake(initialX, initialY);
            mice = new Mice(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            obstaclePos = new Vector2(initialX + 150, initialY - 200);
            pausePos = new Vector2(graphics.PreferredBackBufferWidth - 100, graphics.PreferredBackBufferHeight - 100);
            mousePos = new Vector2(initialX -150, initialY - 150);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //load timer font
            font = Content.Load<SpriteFont>("Timer");
            pause = Content.Load<Texture2D>("pause");

            //load snake assets
            snake.loadSnake(Content.Load<Texture2D>("blueball"), Content.Load<Texture2D>("redball"), Content.Load<Texture2D>("greenball"));

            obstacle = Content.Load<Texture2D>("redball");

            mouse = mice.loadMice(Content.Load<Texture2D>("mouse"));



        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here 

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if(currentKB != null)
                previousKB = currentKB;
            currentKB = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || currentKB.IsKeyDown(Keys.Escape))
                Exit();

            if (currentKB.IsKeyUp(Keys.P) && previousKB.IsKeyDown(Keys.P))
                paused = !paused;    

            if(paused)
                return;



            // TODO: Add your update logic here
            if(!paused){

                ManageTimer(gameTime);

                obstacleBox = UpdateBoundingBox(obstacleBox, obstacle, obstaclePos);
                mouseBox = UpdateBoundingBox(mouseBox, mouse, mousePos);

                if (!timeup)
                {

                    //check win
                    winCondition();

                    snake.UpdateSnakePositions(currentKB, gameTime, graphics, doesIntersect(snake.headBox, obstacleBox)); //update snake 
                    mousePos = mice.UpdateMicePosition(gameTime, graphics, snake.getHeadPosition());
                }
            }


            base.Update(gameTime);

        }

        public bool doesIntersect(BoundingBox a, BoundingBox b){

            bool intersect = a.Intersects(b);
            if (intersect)
                Console.WriteLine("Intersection at: " + a.Max + "," + b.Max);
            return intersect;

        }

        public void winCondition(){

            if(doesIntersect(snake.headBox, mouseBox)){

                timeup = true;
                win = true;  
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            spriteBatch.Begin();

            // TODO: Add your drawing code here
            spriteBatch.DrawString(font, getTimerText(), 
                                   new Vector2(graphics.PreferredBackBufferWidth - (11*graphics.PreferredBackBufferWidth/12), 
                                               graphics.PreferredBackBufferHeight - (11*graphics.PreferredBackBufferHeight/12)), Color.Black);

            spriteBatch.DrawString(font, "Speed: " + snake.getSpeed(),
                                   new Vector2(graphics.PreferredBackBufferWidth - (11 * graphics.PreferredBackBufferWidth / 12),
                                               graphics.PreferredBackBufferHeight - (10 * graphics.PreferredBackBufferHeight / 12)), Color.Black);

            snake.DrawSnake(spriteBatch);
            
            mice.DrawMice(spriteBatch, win);

            spriteBatch.
            Draw(obstacle, obstaclePos, null, Color.White, 0f, 
            new Vector2(obstacle.Width / 2, obstacle.Height / 2), Vector2.One, SpriteEffects.None, 0f);

            if(paused){
                spriteBatch.
                           Draw(pause, pausePos, null, Color.White, 0f,
                                new Vector2(pause.Width / 2, pause.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //keeps track of snake head bounding box
        protected BoundingBox UpdateBoundingBox(BoundingBox box, Texture2D texture, Vector2 pos)
        {
            box.Min.X = pos.X;
            box.Min.Y = pos.Y;
            box.Max.X = pos.X + texture.Width;
            box.Max.Y = pos.Y + texture.Height;

            return box;
        }

        public string getTimerText(){

            if(!timeup && !win){
                return "Time: " + timeSpan.Seconds.ToString();
            }

            else if(win){
                return "You Win!";
            }

            else{
                return "Timeup!";
            }
        }

        private void ManageTimer(GameTime gameTime){

            timeSpan -= gameTime.ElapsedGameTime;
            if (timeSpan < TimeSpan.Zero)

            {
                timeup = true;

            }

        }

    }
}
