using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SnackAttack
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //ball stuff
       // Texture2D textureBall;
       // Texture2D textureBall2;
        Vector2 headPosition;
        public float snakeSpeed;

        int snakeLength = 4; // amnount of snake nodes. keep in mind there is always a head node and tail node.
 
        float xIncrement = 25;
        float yIncrement = 25;

        List<Texture2D> snakeList;
        List<Vector2> positions;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

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
            snakeList = new List<Texture2D>();
            positions = new List<Vector2>();

            //initialize all positions
            headPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            //add and increment offsets in loop

            float initialX = graphics.PreferredBackBufferWidth / 2;
            float initialY = graphics.PreferredBackBufferHeight / 2;

            float xOffset = 0;
            float yOffset = 0;


            for (int i = 0; i < snakeLength; i++)
            {
                positions.Add(new Vector2(initialX + xOffset, initialY + yOffset));
                xOffset += xIncrement;
               
            }



            snakeSpeed = 100f;

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


            //add snake head
            snakeList.Add(Content.Load<Texture2D>("ball"));

            //add snake body
            for (int i = 1; i < snakeLength-1; i++)
            {
                snakeList.Add(Content.Load <Texture2D>("ball"));
            }

            //add snake tail
            snakeList.Add(Content.Load<Texture2D>("ball"));

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float xOffset = 0;
            float yOffset = 0;

            // TODO: Add your update logic here
         
            

            // loop through list, set positions for all nodes while incrementing positions
            for (int i = 0; i < snakeLength; i++)
            {

                //set position
                Vector2 temp = positions[i];

                var kstate = Keyboard.GetState();

                if (kstate.IsKeyDown(Keys.Up))
                    temp.Y -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (kstate.IsKeyDown(Keys.Down))
                    temp.Y += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (kstate.IsKeyDown(Keys.Left))
                    temp.X -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (kstate.IsKeyDown(Keys.Right))
                    temp.X += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                temp.X = Math.Min(Math.Max(snakeList[i].Width / 2, temp.X), graphics.PreferredBackBufferWidth - snakeList[i].Width / 2);
                temp.Y = Math.Min(Math.Max(snakeList[i].Height / 2, temp.Y), graphics.PreferredBackBufferHeight - snakeList[i].Height / 2);
                positions[i] = temp;


                //increment position
                xOffset += xIncrement;
                yOffset += yIncrement;
                


            }

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            for (int i = 0; i < snakeLength; i++)
            {

                spriteBatch.
                Draw(snakeList[i], positions[i], null, Color.White, 0f, new Vector2(snakeList[i].Width / 2, snakeList[0].Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }
            
            spriteBatch.End();

            //headPosition.X += 50;


            base.Draw(gameTime);
        } 
    }
}
