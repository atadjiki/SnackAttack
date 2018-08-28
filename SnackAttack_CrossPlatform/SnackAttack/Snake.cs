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
        int minLength;
        int spacing; //track every nth head positions (0 will look really mushed)
        int collisionModifier = 50;
        int slowdown = 5;
        int travelCount;


        bool up = false, down = false, left = false, right = false;
        bool noKeyPressed = true;
        bool tailMoving = false;
        bool shrinkMode = false;

        List<Vector2> previousPositions;
        List<Vector2> positions;
        List<Texture2D> snakeBody;

        public BoundingBox headBox = new BoundingBox();

        Texture2D headAsset;
        Texture2D bodyAsset;
        Texture2D tailAsset;

        KeyboardState previousKB;



        public Snake(float initialX, float initialY)
        {
            snakeSpeed = 100f;
            maxSpeed = snakeSpeed;
            snakeLength = 2; //must always have at least a head and tail!
            maxLength = 6;
            minLength = snakeLength; //must at minimum be snakelength
            spacing = 25;
            travelCount = 0;


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

        public void growSnake(int amount, Vector2 position)
        {

            positions.Insert(positions.Count - 1, position);
            snakeBody.Insert(positions.Count - 2, bodyAsset);


        }

        public void shrinkSnake()
        {
            positions.RemoveAt(positions.Count - 1);

        }

        public Vector2 getHeadPosition()
        {
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

        public void UpdateSnakePositions(KeyboardState kstate, GameTime gameTime, GraphicsDeviceManager graphics, bool doesIntersect)
        {

            UpdateBoundingBox();
            noKeyPressed = true;
            shrinkMode = false;


            //the snake shouldnt move if no key is being pressed
            //also, depending on what key is pressed the head is either the first or last node
            //and the previous positions list should be reversed if moving the tail

            //if the head is moving but the snake has reached max lenghth, we need to disallow further movement of the head
            //UNTIL the user switches controls

            bool allowTail = true;
            bool allowHead = true;

            //cant move head and tail simultaneously 
            //if(kstate.IsKeyDown(Keys.LeftShift) && (kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.S) || kstate.IsKeyDown(Keys.A) || kstate.IsKeyDown(Keys.D))
            //   && (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.Right)))
            //{
            //    return;
            //}

            ////if the tail moved last, and is currently being moved, and the snake is at max length, return 
            //if (previousKB.IsKeyDown(Keys.W) || previousKB.IsKeyDown(Keys.S) || previousKB.IsKeyDown(Keys.A) || previousKB.IsKeyDown(Keys.D))
            //{
            //    if (kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.D) || kstate.IsKeyDown(Keys.A) || kstate.IsKeyDown(Keys.D))
            //    {
            //        if (!checkDistance(positions[0], positions[positions.Count - 1]))
            //        {
            //            allowHead = false;
            //        }

            //    }

            //}
            //else if (previousKB.IsKeyDown(Keys.Up) || previousKB.IsKeyDown(Keys.Down) || previousKB.IsKeyDown(Keys.Left) || previousKB.IsKeyDown(Keys.Right))
            //{
            //    if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.Right))
            //    {
            //        if (!checkDistance(positions[0], positions[positions.Count - 1]))
            //        {
            //            allowTail = false;
            //        }
            //    }

            //}
            //else if (previousKB.IsKeyDown(Keys.Up) || previousKB.IsKeyDown(Keys.Down) || previousKB.IsKeyDown(Keys.Left) || previousKB.IsKeyDown(Keys.Right))
            //{
            //    if (kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.D) || kstate.IsKeyDown(Keys.A) || kstate.IsKeyDown(Keys.D))
            //    {
            //        if (!checkDistance(positions[0], positions[positions.Count - 1]))
            //        {
            //            allowHead = true;
            //        }

            //    }

            //}
            //else if (previousKB.IsKeyDown(Keys.W) || previousKB.IsKeyDown(Keys.D) || previousKB.IsKeyDown(Keys.A) || previousKB.IsKeyDown(Keys.D))
            //{
            //    if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.Right))
            //    {
            //        if (!checkDistance(positions[0], positions[positions.Count - 1]))
            //        {
            //            allowTail = true;
            //        }
            //    }

            //}






            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.Right))
            {

                //if moving tail, reverse the previousposition queue if switching from front of snake
                if (!tailMoving)
                {

                    positions.Reverse();
                    repopulatePreviousPositions();
                    snakeBody.Reverse();
                }


                tailMoving = true;
                noKeyPressed = false;
            }
            else if (kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.S) || kstate.IsKeyDown(Keys.A) || kstate.IsKeyDown(Keys.D))
            {

                if (tailMoving)
                {

                    positions.Reverse();
                    repopulatePreviousPositions();
                    snakeBody.Reverse();
                }

                tailMoving = false;
                noKeyPressed = false;
            }
            else if (kstate.IsKeyDown(Keys.LeftShift) || kstate.IsKeyDown(Keys.LeftShift))
            {
                shrinkMode = true;
            }
            if (noKeyPressed && !shrinkMode)
                return;


            //dont change any positions if the snake isnt moving 
            else if (!noKeyPressed || shrinkMode)
            {
                if((tailMoving && allowTail) || (!tailMoving && allowHead)){

                
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
                if (doesIntersect)
                {

                    snakeSpeed = snakeSpeed / slowdown;

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

                if (snakeSpeed < maxSpeed)
                {
                    snakeSpeed++;
                }


                positions[0] = head;

                // loop through list, set positions for all nodes while incrementing positions
                for (int i = 1; i < positions.Count; i++)
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
                    Vector2 position = positions[positions.Count - 1];
                    growSnake(1, position);
                    snakeLength++;
                }

            }

            previousKB = kstate;

            }
        }

        public void DrawSnake(SpriteBatch spriteBatch)
        {
            //draw all snake nodes


            for (int i = 0; i < positions.Count; i++)
            {

                spriteBatch.
                Draw(snakeBody[i], positions[i], null, Color.White, 0f, new Vector2(snakeBody[i].Width / 2, snakeBody[i].Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }

        }

        //keeps track of snake head bounding box
        public void UpdateBoundingBox()
        {
            this.headBox.Min.X = positions[0].X;
            this.headBox.Min.Y = positions[0].Y;
            this.headBox.Max.X = positions[0].X + snakeBody[0].Width;
            this.headBox.Max.Y = positions[0].Y + snakeBody[0].Height;
        }

        public float getSpeed()
        {
            return snakeSpeed;
        }

        public float getSnakeDistance()
        {
            return Vector2.Distance(positions[0],positions[positions.Count - 1]);
        }

        public int getSnakeLength()
        {
            return positions.Count;
        }

        public bool checkDistance(Vector2 head, Vector2 tail)
        {


            float maxDistance = 3.5f * bodyAsset.Width;
            float distance = Vector2.Distance(head, tail);

            if (distance > maxDistance)
            {
                Console.WriteLine("Snake too long! " + distance);
                return false;
            }

            else
            {
                return true;
            }

        }

        public float getDistance(){


          return Vector2.Distance(positions[0], positions[positions.Count -1]);
        }

        private void repopulatePreviousPositions()
        {


            if (previousPositions.Count >= positions.Count * spacing)
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
