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
        Mice mice;

        Texture2D obstacle;
        List<Texture2D> bombs;
        List<Vector2> bombPostion;
        Vector2 obstaclePos;
        BoundingBox obstacleBox;


        //these are just placeholders to test win condition 
        Texture2D mouse;
        Vector2 mousePos;
        BoundingBox mouseBox;

        TimeSpan timeSpan = TimeSpan.FromMilliseconds(31000); //30 sec in ms
        bool timeup = false;
        bool win = false;

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
            mousePos = new Vector2(initialX -150, initialY - 150);

            // Just for Reference of position of mice
            float x = graphics.PreferredBackBufferWidth;
            float y = graphics.PreferredBackBufferHeight;
            bombs = new List<Texture2D>(4);
            bombPostion = new List<Vector2>(4);
            bombPostion.Add(new Vector2(x - 50f, y - 50f));
            bombPostion.Add(new Vector2(0f + 50f, y - 50f));
            bombPostion.Add(new Vector2(0f + 50f, 0f + 50f));
            bombPostion.Add(new Vector2(x - 50f, 0f + 50f));

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

            mouse = Content.Load<Texture2D>("mouse");



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
            obstacleBox = UpdateBoundingBox(obstacleBox, obstacle, obstaclePos);
            mouseBox = UpdateBoundingBox(mouseBox, mouse, mousePos);

            if(!timeup){
                var kstate = Keyboard.GetState(); //get keyboard input;

                //check win
                winCondition();

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
            for (int i = 0; i < 4; i++)
            {
                spriteBatch.
                    Draw(bombs[i], bombPostion[i], null, Color.White, 0f,
                            new Vector2(bombs[i].Width / 2, bombs[i].Height / 2), 0.3f, SpriteEffects.None, 0f);
            }
            mice.DrawMice(spriteBatch);
            spriteBatch.
            Draw(obstacle, obstaclePos, null, Color.White, 0f, 
            new Vector2(obstacle.Width / 2, obstacle.Height / 2), Vector2.One, SpriteEffects.None, 0f);

            if(!win){
                spriteBatch.
                       Draw(mouse, mousePos, null, Color.White, 0f,
                new Vector2(mouse.Width / 2, mouse.Height / 2), Vector2.One, SpriteEffects.None, 0f);

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
