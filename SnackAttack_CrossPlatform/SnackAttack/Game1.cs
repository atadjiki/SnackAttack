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
        float initialX;
        float initialY;

        Snake snake;
        Mice mice;


        Texture2D pause;
        Vector2 pausePos;


        //these are just placeholders to test win condition 
        Texture2D mouse;
        Vector2 mousePos;
        BoundingBox mouseBox;

        TimeSpan timeSpan; //30 sec in ms, extra second for startup :p
        int time = 31000;

        KeyboardState currentKB, previousKB;
        private SpriteFont font;

        enum GameState{ Start, Playing, Paused, Won, TimeUp};
        GameState gameState;

        string welcomeMessage;
        string timeUpMessage;
        string winMessage;

        bool obstacleMode = false;
        Obstacles obstacles;

        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameState = GameState.Start;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            timeSpan = TimeSpan.FromMilliseconds(time);

            welcomeMessage =
            "Welcome to Snake! \n\n Press Enter to Begin \n\n Controls: \n r - restart \n shift - shrink \n wasd - control head \n arrow keys - control tail";

            timeUpMessage = "Time up! Press 'r' to Restart";


            initialX = (graphics.PreferredBackBufferWidth / 2) - 150; //get middle of the screen 
            initialY = graphics.PreferredBackBufferHeight / 2;

            mouseBox = new BoundingBox();
            snake = new Snake(initialX, initialY);
            mice = new Mice(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            pausePos = new Vector2(graphics.PreferredBackBufferWidth - 100, graphics.PreferredBackBufferHeight - 100);
            mousePos = new Vector2(initialX - 150, initialY - 150);

            if (obstacleMode)
                obstacles = new Obstacles(Content.Load<Texture2D>("brick"));

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

            //load timer font
            font = Content.Load<SpriteFont>("Timer");
            pause = Content.Load<Texture2D>("pause");

            //load snake assets
            snake.loadSnake(Content.Load<Texture2D>("blueball"), Content.Load<Texture2D>("redball"), Content.Load<Texture2D>("greenball"));


            mouse = mice.loadMice(Content.Load<Texture2D>("mouse"));



        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {



            if (gameState == GameState.Start)
            {

                handleStartMenuInputs();
            }

            else if (gameState == GameState.Playing)
            {

                handleInputs();

                updateGame(gameTime);
            }
            else if (gameState == GameState.Paused)
            {
                handleInputs();
            }
            else if (gameState == GameState.Won)
            {
                handleInputs();
            }
            else if (gameState == GameState.TimeUp){
                handleInputs();
            }


            base.Update(gameTime);

        }

        private void updateGame(GameTime gameTime)
        {
            ManageTimer(gameTime);

            if(obstacleMode){
                obstacles.UpdateObstacleBoxes();
            }

            mouseBox = Collision.UpdateBoundingBox(mouseBox, mouse, mousePos);

            if (gameState != GameState.TimeUp)
            {

                //check win
                winCondition();
                bool collision;

                if (obstacleMode)
                    collision = obstacles.checkCollision(snake.headBox);
                else
                  collision = false;

                snake.UpdateSnakePositions(currentKB, gameTime, graphics, collision); //update snake 
                mousePos = mice.UpdateMicePosition(gameTime, snake.getHeadPosition());
            }
        }

        private void handleStartMenuInputs()
        {
            if (currentKB != null)
                previousKB = currentKB;

            currentKB = Keyboard.GetState();

            if(gameState == GameState.Start){
                if(currentKB.IsKeyDown(Keys.Enter)){
                    startGame();
                }
            }
        }

        private void handleInputs()
        {
            if (currentKB != null)
                previousKB = currentKB;

            currentKB = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || currentKB.IsKeyDown(Keys.Escape))
                Exit();
            if (currentKB.IsKeyUp(Keys.P) && previousKB.IsKeyDown(Keys.P) && gameState == GameState.Paused)
                gameState = GameState.Playing;
            else if (currentKB.IsKeyUp(Keys.P) && previousKB.IsKeyDown(Keys.P) && gameState == GameState.Playing)
                gameState = GameState.Paused;
            if (gameState == GameState.Paused)
                return;
            if (gameState == GameState.Won && currentKB.IsKeyDown(Keys.R)){
                Initialize();
                gameState = GameState.Start;
                return;
            }
            if(gameState == GameState.Playing && currentKB.IsKeyDown(Keys.R)){
                Initialize();
                gameState = GameState.Start;
            }
            if (gameState == GameState.Paused && currentKB.IsKeyDown(Keys.R)){
                Initialize();
                gameState = GameState.Start;
            }
            if(gameState == GameState.TimeUp && currentKB.IsKeyDown(Keys.R)){
                Initialize();
                gameState = GameState.Start;
            }
        }

        public void winCondition(){

            if(Collision.doesIntersect(snake.headBox, mouseBox)){
                gameState = GameState.Won;
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

            if(gameState == GameState.Start){
                drawStartUI();
            }

            if (gameState == GameState.Playing)
            {

                drawGameActors();
                drawGameUI();
            }

            if(gameState == GameState.Paused){
                drawPauseUI();
                drawGameActors();
                drawGameUI();
            }

            if(gameState == GameState.Won){
                drawGameActors();
                drawWinUI();
            }

            if(gameState == GameState.TimeUp){
                drawGameActors();
                drawTimeUpUI();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawStartUI(){



            spriteBatch.
                       DrawString(font, welcomeMessage,
                                   new Vector2(graphics.PreferredBackBufferWidth / 2 - 100,
                                               graphics.PreferredBackBufferHeight / 2 -75), Color.White);
        }

        private void drawPauseUI(){
            spriteBatch.
                           Draw(pause, pausePos, null, Color.White, 0f,
                                new Vector2(pause.Width / 2, pause.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }

        private void drawGameUI()
        {
            spriteBatch.
                       DrawString(font, getTimerText(),
                                   new Vector2(graphics.PreferredBackBufferWidth - (11 * graphics.PreferredBackBufferWidth / 12),
                                               graphics.PreferredBackBufferHeight - (11 * graphics.PreferredBackBufferHeight / 12)), Color.White);

            spriteBatch.
                       DrawString(font, "Speed: " + snake.getSpeed(),
                                   new Vector2(graphics.PreferredBackBufferWidth - (11 * graphics.PreferredBackBufferWidth / 12),
                                               graphics.PreferredBackBufferHeight - (10 * graphics.PreferredBackBufferHeight / 12)), Color.White);

            spriteBatch.
                       DrawString(font, "Length: " + snake.getSnakeDistance(),
                                   new Vector2(graphics.PreferredBackBufferWidth - (11 * graphics.PreferredBackBufferWidth / 12),
                                               graphics.PreferredBackBufferHeight - (9 * graphics.PreferredBackBufferHeight / 12)), Color.White);
        }

        private void drawTimeUpUI(){

            spriteBatch.
                       DrawString(font, timeUpMessage,
                                   new Vector2(graphics.PreferredBackBufferWidth / 2 - 50,
                                               graphics.PreferredBackBufferHeight / 2), Color.White);

        }

        private void drawWinUI()
        {

            TimeSpan winTime = new TimeSpan(0, 0, time);



            winMessage = "You win! Time: " + (winTime.Seconds - timeSpan.Seconds).ToString() + " seconds. Press 'r' to Restart";

            spriteBatch.
                       DrawString(font, winMessage,
                                   new Vector2(graphics.PreferredBackBufferWidth / 2 - 50,
                                               graphics.PreferredBackBufferHeight / 2), Color.White);

        }

        private void drawGameActors()
        {

            if(obstacleMode)
                obstacles.DrawObstacles(spriteBatch);

            snake.DrawSnake(spriteBatch);

            bool win = false; if (gameState == GameState.Won) win = true;
            mice.DrawMice(spriteBatch, win);


        }


        public string getTimerText(){

            return "Time: " + timeSpan.Seconds.ToString();
        }

        private void ManageTimer(GameTime gameTime){

            timeSpan -= gameTime.ElapsedGameTime;
            if (timeSpan < TimeSpan.Zero)

            {
                gameState = GameState.TimeUp;

            }

        }

       

        private void startGame(){

            gameState = GameState.Playing;
            Initialize();
        }
    }
}
