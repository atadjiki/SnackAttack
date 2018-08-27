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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Color backgroundColor = Color.CornflowerBlue;
        float initialX;
        float initialY;

        Snake snake;
        Mice mice;


        Texture2D obstacle;
        List<Vector2>obstacles;
        List<BoundingBox> obstacleBoxes;

        Texture2D pause;
        Vector2 pausePos;


        //these are just placeholders to test win condition 
        Texture2D mouse;
        Vector2 mousePos;
        BoundingBox mouseBox;

        TimeSpan timeSpan; //30 sec in ms, extra second for startup :p
        bool timeup;

        KeyboardState currentKB, previousKB;
        private SpriteFont font;

        enum GameState{ Start, Playing, Paused, Won};
        GameState gameState;


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

            initialX = graphics.PreferredBackBufferWidth / 2; //get middle of the screen 
            initialY = graphics.PreferredBackBufferHeight / 2;

            mouseBox = new BoundingBox();
            obstacleBoxes = new List<BoundingBox>();
            snake = new Snake(initialX, initialY);
            mice = new Mice(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            obstacles = new List<Vector2>();
            InitializeObstacles();
            pausePos = new Vector2(graphics.PreferredBackBufferWidth - 100, graphics.PreferredBackBufferHeight - 100);
            mousePos = new Vector2(initialX -150, initialY - 150);

            gameState = GameState.Playing;
            timeSpan = TimeSpan.FromMilliseconds(31000);
            timeup = false;

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

            if (currentKB.IsKeyUp(Keys.P) && previousKB.IsKeyDown(Keys.P) && gameState == GameState.Paused)
                gameState = GameState.Playing;
            else if (currentKB.IsKeyUp(Keys.P) && previousKB.IsKeyDown(Keys.P) && gameState == GameState.Playing)
                gameState = GameState.Paused;

            if(gameState == GameState.Paused)
                return;

            if (gameState == GameState.Won && currentKB.IsKeyDown(Keys.R))
            {
                Initialize();
                return;
            }


            // TODO: Add your update logic here
            if(gameState == GameState.Playing){

                ManageTimer(gameTime);

                UpdateObstacleBoxes();

                mouseBox = UpdateBoundingBox(mouseBox, mouse, mousePos);

                if (!timeup)
                {

                    //check win
                    winCondition();
                    bool collision = CheckCollisions();
                    snake.UpdateSnakePositions(currentKB, gameTime, graphics, collision); //update snake 
                    mousePos = mice.UpdateMicePosition(gameTime, snake.getHeadPosition());
                }
            }


            base.Update(gameTime);

        }

        private bool CheckCollisions()
        {
            bool collision = false;

            for (int i = 0; i < obstacles.Count; i++){
                if(doesIntersect(snake.headBox, obstacleBoxes[i])){
                    collision = true;
                    break;
                }
            }

            return collision;
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

            // TODO: Add your drawing code here
            spriteBatch.
                       DrawString(font, getTimerText(), 
                                   new Vector2(graphics.PreferredBackBufferWidth - (11*graphics.PreferredBackBufferWidth/12), 
                                               graphics.PreferredBackBufferHeight - (11*graphics.PreferredBackBufferHeight/12)), Color.Black);

            spriteBatch.
                       DrawString(font, "Speed: " + snake.getSpeed(),
                                   new Vector2(graphics.PreferredBackBufferWidth - (11 * graphics.PreferredBackBufferWidth / 12),
                                               graphics.PreferredBackBufferHeight - (10 * graphics.PreferredBackBufferHeight / 12)), Color.Black);

            snake.DrawSnake(spriteBatch);

            bool win = false; if (gameState == GameState.Won) win = true;
            mice.DrawMice(spriteBatch, win);

            DrawObstacles(spriteBatch);

            if(gameState == GameState.Paused){
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

        protected void UpdateObstacleBoxes()
        {
            for (int i = 0; i < obstacles.Count; i++){

                obstacleBoxes[i] = UpdateBoundingBox(obstacleBoxes[i], obstacle, obstacles[i]);
            }
            
        }

        public string getTimerText(){

            if(!timeup && gameState != GameState.Won){
                return "Time: " + timeSpan.Seconds.ToString();
            }

            else if(gameState == GameState.Won){
                return "You Win! Press 'r' to restart.";
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

        private void DrawObstacles(SpriteBatch spriteBatch){


            foreach(Vector2 position in obstacles){

                spriteBatch.
                           Draw(obstacle, position, null, Color.White, 0f,
                new Vector2(obstacle.Width / 2, obstacle.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }


        }

        private void InitializeObstacles(){

            obstacles.Add(new Vector2(initialX + 150, initialY - 200));


            for (int i = 0; i < obstacles.Count; i++)
                obstacleBoxes.Add(new BoundingBox());
        }
    }
}
