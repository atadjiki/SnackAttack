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

        Snake snake;

        Texture2D obstacle;
        Vector2 obstaclePos;
        BoundingBox obstacleBox;

        TimeSpan timeSpan = TimeSpan.FromMilliseconds(31000); //30 sec in ms
        bool timeup = false;

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

            obstacleBox = new BoundingBox();
            snake = new Snake(initialX, initialY);


            obstaclePos = new Vector2(initialX + 150, initialY - 200);

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

            //load snake assets
            snake.loadSnake(Content.Load<Texture2D>("blueball"), Content.Load<Texture2D>("redball"), Content.Load<Texture2D>("greenball"));

            obstacle = Content.Load<Texture2D>("ball");



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

            ManageTimer(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // TODO: Add your update logic here
            UpdateBoundingBox();

            if(!timeup){
                var kstate = Keyboard.GetState(); //get keyboard input;

                snake.UpdateSnakePositions(kstate, gameTime, graphics, doesIntersect(snake.headBox, obstacleBox)); //update snake 
            }

            base.Update(gameTime);

        }

        public bool doesIntersect(BoundingBox a, BoundingBox b){

            bool doesIntersect = a.Intersects(b);
            if (doesIntersect)
                Console.WriteLine("Intersection at: " + a.Max + "," + b.Max);
            return doesIntersect;

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

            spriteBatch.
            Draw(obstacle, obstaclePos, null, Color.White, 0f, 
            new Vector2(obstacle.Width / 2, obstacle.Height / 2), Vector2.One, SpriteEffects.None, 0f);


            spriteBatch.End();

            base.Draw(gameTime);
        }

        //keeps track of snake head bounding box
        protected void UpdateBoundingBox()
        {
            this.obstacleBox.Min.X = obstaclePos.X;
            this.obstacleBox.Min.Y = obstaclePos.Y;
            this.obstacleBox.Max.X = obstaclePos.X + obstacle.Width;
            this.obstacleBox.Max.Y = obstaclePos.Y + obstacle.Height;
        }

        public string getTimerText(){

            if(!timeup){
                return "Time: " + timeSpan.Seconds.ToString();
            } else{
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
