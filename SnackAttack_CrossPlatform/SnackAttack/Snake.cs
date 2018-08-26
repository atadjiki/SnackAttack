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
        float maxSpeed;

        int snakeLength; // maximum amnount of snake nodes, including head
        int maxLength;
     //   int minLength;
        int spacing; //track every nth head positions (0 will look really mushed)
        int collisionModifier = 100;


        bool up = false, down = false, left = false, right = false;
        bool noKeyPressed = true;
        bool tailMoving = false;
        bool coilMode = false;

        List<Vector2> previousPositions;
        List<Texture2D> snakeBody;
        List<Vector2> positions;
        BoundingBox headBox = new BoundingBox();

        Texture2D headAsset;
        Texture2D bodyAsset;
        Texture2D tailAsset;
         


        public Snake(float initialX, float initialY)
        {
            snakeSpeed = 100f;
            maxSpeed = snakeSpeed;
            snakeLength = 2; //must always have at least a head and tail!
            maxLength = 5;
            //minLength = 2;
            spacing = 25;


            snakeBody = new List<Texture2D>(snakeLength);
            positions = new List<Vector2>(snakeLength);
            previousPositions = new List<Vector2>(snakeLength * spacing);


            //add and increment offsets in loop

            //initialize all node positions 
            for (int i = 0; i < snakeLength; i++)
            {
                positions.Add(new Vector2(initialX, initialY));

            }

        }

        public void growSnake(int amount, Vector2 position){

            positions.Insert(positions.Count-1, position);
            snakeBody.Insert(positions.Count - 2, bodyAsset);


        }

        public void shrinkSnake(){
            positions.RemoveAt(positions.Count - 1);

        }

        public Vector2 getHeadPosition(){
            return positions[0];
        }

        public void loadSnake(Texture2D head, Texture2D body, Texture2D tail)
        {
            //add snake head

            headAsset = head;
            snakeBody.Add(headAsset);

            bodyAsset = body;

            //add snake tail
            tailAsset = tail;
            snakeBody.Add(tailAsset);
        }

        public void UpdateSnakePositions(KeyboardState kstate, GameTime gameTime, GraphicsDeviceManager graphics, BoundingBox obstacleBox)
        {

            UpdateBoundingBox();
            noKeyPressed = true;
            coilMode = false;


            //the snake shouldnt move if no key is being pressed
            //also, depending on what key is pressed the head is either the first or last node
            //and the previous positions list should be reversed if moving the tail

            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.Right))
            {

                //if moving tail, reverse the previousposition queue if switching from front of snake
                if (!tailMoving){

                    positions.Reverse();
                    repopulatePreviousPositions();
                    snakeBody.Reverse();
                }
                    

                tailMoving = true;
                noKeyPressed = false;
            }
            else if (kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.S) || kstate.IsKeyDown(Keys.A) || kstate.IsKeyDown(Keys.D))
            {

                if (tailMoving){
                   
                    positions.Reverse();
                    repopulatePreviousPositions();
                    snakeBody.Reverse();
                }
                    
                tailMoving = false;
                noKeyPressed = false;
            }
            else if(kstate.IsKeyDown(Keys.LeftShift) || kstate.IsKeyDown(Keys.LeftShift)){
                coilMode = true;
            }
            if (noKeyPressed && !coilMode)
                return;

            //dont change any positions if the snake isnt moving 
            if (!noKeyPressed || coilMode)
            {

                //are we moving the head or the tail now?
                var head = positions[0];

                var lastPreviousPosition = head;

                //move head or tail first

                if (tailMoving)
                {
                    if (kstate.IsKeyDown(Keys.Up))
                    {
                        head.Y -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        up = true; down = false; left = false; right = false;
                        noKeyPressed = false;
                    }

                    else if (kstate.IsKeyDown(Keys.Down))
                    {
                        head.Y += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        up = false; down = true; left = false; right = false;
                        noKeyPressed = false;
                    }


                    else if (kstate.IsKeyDown(Keys.Left))
                    {
                        head.X -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        up = false; down = false; left = true; right = false;
                        noKeyPressed = false;
                    }

                    else if (kstate.IsKeyDown(Keys.Right))
                    {
                        head.X += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        up = false; down = false; left = false; right = true;
                        noKeyPressed = false;
                    }

                }

                else if (!tailMoving)
                {

                    if (kstate.IsKeyDown(Keys.W))
                    {
                        head.Y -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        up = true; down = false; left = false; right = false;
                        noKeyPressed = false;
                    }

                    else if (kstate.IsKeyDown(Keys.S))
                    {
                        head.Y += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        up = false; down = true; left = false; right = false;
                        noKeyPressed = false;
                    }


                    else if (kstate.IsKeyDown(Keys.A))
                    {
                        head.X -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        up = false; down = false; left = true; right = false;
                        noKeyPressed = false;
                    }

                    else if (kstate.IsKeyDown(Keys.D))
                    {
                        head.X += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        up = false; down = false; left = false; right = true;
                        noKeyPressed = false;
                    }
                }

                previousPositions.Insert(0, lastPreviousPosition);

                //this stops the snake from moving out of the screen :)
                head.X = Math.Min(Math.Max(headAsset.Width / 2, head.X), graphics.PreferredBackBufferWidth - headAsset.Width / 2);
                head.Y = Math.Min(Math.Max(headAsset.Height / 2, head.Y), graphics.PreferredBackBufferHeight - headAsset.Height / 2);

                //check collision, if this move is allowed, store the position - if not, stay where we are!
                if (headBox.Intersects(obstacleBox))
                {
                    Console.WriteLine("Intersection! at :" + headBox.Max + "," + obstacleBox.Max);


                    snakeSpeed = snakeSpeed / 5;

                    Console.WriteLine("Speed: " + snakeSpeed);

                    if (up)
                    {
                        head.Y += collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        
                    }
                    else if (down)
                    {
                        head.Y -= collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (left)
                    {
                        head.X += collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (right)
                    {
                        head.X -= collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }

                if(snakeSpeed < maxSpeed){
                    snakeSpeed++;
                }

                positions[0] = head;


                // loop through list, set positions for all nodes while incrementing positions
                for (int i = 1; i < snakeLength; i++)
                {

                    //set position
                    Vector2 temp = positions[i];

                    if (previousPositions.Count != 0 && previousPositions.Count > i * spacing)
                    {
                        temp = previousPositions.ToArray()[i * spacing];
                    }

                    //store back in list
                    positions[i] = temp;

                }

                //if there is room to grow, and the head has moved enough, increment snake 
                if (previousPositions.Count > spacing * positions.Count && positions.Count <= maxLength)
                {

                    Vector2 position = positions[positions.Count-1];
                    growSnake(1, position);
                    snakeLength++;
                }
                else if(snakeLength==maxLength){
                    //stop snake until opposite moves
                }


            }

        }

        public void DrawSnake(SpriteBatch spriteBatch)
        {
            //draw all snake nodes
   

            for (int i = 0; i < snakeLength; i++)
            {

                spriteBatch.
                Draw(snakeBody[i], positions[i], null, Color.White, 0f, new Vector2(snakeBody[i].Width / 2, snakeBody[i].Height / 2), Vector2.One, SpriteEffects.None, 0f);
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

        private void repopulatePreviousPositions(){



            if(previousPositions.Count >= positions.Count * spacing)
                previousPositions = previousPositions.GetRange(0, positions.Count * spacing);

            previousPositions.Reverse();

            for (int i = 0; i < positions.Count; i++)
            {

                //set position
                Vector2 temp = positions[i];

                if (previousPositions.Count != 0 && previousPositions.Count > i * spacing)
                {
                    temp = previousPositions.ToArray()[i * spacing];
                }



                //store back in list
                positions[i] = temp;

            }


        }
    }


}
