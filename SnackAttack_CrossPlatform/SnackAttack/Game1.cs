using Microsoft.Xna.Framework;
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

        //these are just placeholders to test win condition 

        TimeSpan timeSpan; //30 sec in ms, extra second for startup :p

        KeyboardState currentKB, previousKB;


        public enum GameState{ Start, Playing, Paused, Won, TimeUp};
        public static GameState gameState;


        bool contentLoaded = false;

        public Game1()
        {

            GraphicsManager.Instance.setGraphicsDeviceManager(new GraphicsDeviceManager(this));
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

            timeSpan = TimeSpan.FromMilliseconds(Variables.time);


            Mice.Instance.mouseBox = new BoundingBox();

            Mice.Instance.mousePos = new Vector2(GraphicsManager.Instance.getInitialX() - 150, GraphicsManager.Instance.getInitialY() - 150);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            GraphicsManager.Instance.LoadContent();
            GraphicsManager.Instance.font = Content.Load<SpriteFont>("Timer");
            GraphicsManager.Instance.pause = Content.Load<Texture2D>("pause");
            GraphicsManager.Instance.pausePos = new Vector2(GraphicsManager.Instance.getPreferredWidth() - 100, GraphicsManager.Instance.getPreferredHeight() - 100);

            //load characters
            GraphicsManager.Instance.loadSnake(Content.Load<Texture2D>("blueball"), Content.Load<Texture2D>("redball"), Content.Load<Texture2D>("greenball"));
            GraphicsManager.Instance.mouse = Mice.Instance.loadMice(Content.Load<Texture2D>("mouse"));

            if(Variables.obstacleMode)
                GraphicsManager.Instance.obstacle = Content.Load<Texture2D>("brick");

            contentLoaded = true;


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

            if (!contentLoaded)
                return;

            ManageTimer(gameTime);

            if(Variables.obstacleMode){
                Obstacles.Instance.UpdateObstacleBoxes(GraphicsManager.Instance.obstacle);
            }

            Mice.Instance.mouseBox = Collision.UpdateBoundingBox(Mice.Instance.mouseBox, GraphicsManager.Instance.mouse, Mice.Instance.mousePos);

            if (gameState != GameState.TimeUp)
            {

                //check win
                winCondition();
                bool collision;

                if (Variables.obstacleMode)
                    collision = Obstacles.Instance.checkCollision();
                else
                  collision = false;

                Snake.Instance.UpdateSnakePositions(currentKB, gameTime, GraphicsManager.Instance.GetGraphics(), collision); //update snake 
                Mice.Instance.mousePos = Mice.Instance.UpdateMicePosition(gameTime);
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

            if(Collision.doesIntersect(Mice.Instance.mouseBox, Snake.Instance.headBox)){
                gameState = GameState.Won;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Variables.backgroundColor);

            GraphicsManager.Instance.DrawContent(getTimerText(), timeSpan);

            base.Draw(gameTime);
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
