using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SnackAttack.Desktop
{
    public class Snake
    {
        //constants
        float snakeSpeed;
        int snakeLength; // amnount of snake nodes, including head
        int spacing; //track every nth head positions (0 will look really mushed)
        int collisionModifier = 10;

        Queue<Vector2> previousPositions;
        List<Texture2D> snakeBody;
        List<Vector2> positions;

        BoundingBox headBox = new BoundingBox();

        public Snake(float initialX, float initialY)
        {
            snakeSpeed = 100f;
            snakeLength = 16;
            spacing = 15;

            snakeBody = new List<Texture2D>(snakeLength);
            positions = new List<Vector2>(snakeLength);
            previousPositions = new Queue<Vector2>(snakeLength * spacing);


            //add and increment offsets in loop

            //initialize all node positions 
            for (int i = 0; i < snakeLength; i++)
            {
                positions.Add(new Vector2(initialX, initialY));

            }

        }

        public void loadSnake(Texture2D head, Texture2D body, Texture2D tail){
            //add snake head
            
            snakeBody.Add(head);

            //add snake body
            for (int i = 0; i < snakeLength; i++)
            {
                snakeBody.Add(body);
            }

            //add snake tail
            snakeBody.Add(tail);
        }

        public void UpdateSnakePositions(KeyboardState kstate, GameTime gameTime, GraphicsDeviceManager graphics, BoundingBox obstacleBox)
        {

            UpdateBoundingBox();
            var head = positions[0];

            bool up = false, down = false, left = false, right = false;


            //store previous head position in queue
            if (previousPositions.Count == snakeLength * spacing)
            {
                previousPositions.Dequeue();
            }
            previousPositions.Enqueue(positions[0]);


            //move head first

            if (kstate.IsKeyDown(Keys.Up))
            {
                head.Y -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                up = true;
            }

            if (kstate.IsKeyDown(Keys.Down))
            {
                head.Y += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                down = true;
            }


            if (kstate.IsKeyDown(Keys.Left))
            {
                head.X -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                left = true;
            }

            if (kstate.IsKeyDown(Keys.Right))
            {
                head.X += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                right = true;
            }

            // head.X = Math.Min(Math.Max(snakeBody[0].Width / 2, head.X), graphics.PreferredBackBufferWidth - snakeBody[0].Width / 2);
            // head.Y = Math.Min(Math.Max(snakeBody[0].Height / 2, head.Y), graphics.PreferredBackBufferHeight - snakeBody[0].Height / 2);

            //check collision, if this move is allowed, store the position - if not, stay where we are!
            if (headBox.Intersects(obstacleBox)){
                Console.WriteLine("Intersection! at :" + headBox.Max + "," + obstacleBox.Max);

                if(up){
                    head.Y += collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else if (down){
                    head.Y -= collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else if (left){
                    head.X += collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else if(right){
                    head.X -= collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }


            }


            positions[0] = head;

            // loop through list, set positions for all nodes while incrementing positions
            for (int i = 1; i < snakeLength; i++)
            {

                //set position
                Vector2 temp = positions[i];

                if (previousPositions.Count != 0 && previousPositions.ToArray().Length > i * spacing)
                {
                    temp = previousPositions.ToArray()[i * spacing];
                }


                //store back in list
                positions[i] = temp;

            }
        }

        public void DrawSnake(SpriteBatch spriteBatch){
            //draw all snake nodes
            for (int i = 0; i < snakeLength; i++)
            {

                spriteBatch.
                Draw(snakeBody[i], positions[i], null, Color.White, 0f, new Vector2(snakeBody[i].Width / 2, snakeBody[0].Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }
        }

        //keeps track of snake head bounding box
        protected void UpdateBoundingBox()
        {
            this.headBox.Min.X = positions[0].X;
            this.headBox.Min.Y = positions[0].Y;
            this.headBox.Max.X = positions[0].X + snakeBody[0].Width;
            this.headBox.Max.Y = positions[0].Y + snakeBody[0].Height;
        }
    }


}
