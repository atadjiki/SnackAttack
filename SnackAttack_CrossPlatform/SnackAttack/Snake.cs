using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SnackAttack.Desktop
{
    public class Snake
    {


        List<Vector2> previousPositions;
        List<Vector2> positions;
        List<Texture2D> snakeBody;

        List<BoundingBox> snakeBoxes;

        KeyboardState previousKB;

        bool up = false, down = false, left = false, right = false;
        bool noKeyPressed = true;
        bool tailMoving = false;
        bool shrinkMode = false;

        float snakeSpeed = Variables.maxSpeed;
        int snakeLength = Variables.minLength;

        private static Snake instance = null;

        public Snake()
        {
            Initialize();
        }

        public static Snake Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Snake();
                }
                return instance;
            }
        }

        public void Initialize(){

            snakeBody = new List<Texture2D>(Variables.minLength);
            positions = new List<Vector2>(Variables.minLength);
            previousPositions = new List<Vector2>(Variables.minLength * Variables.spacing);
            snakeBoxes = new List<BoundingBox>();


            //add and increment offsets in loop

            //initialize all node positions 
            for (int i = 0; i < Variables.minLength; i++)
            {
                positions.Add(new Vector2(GraphicsManager.Instance.getInitialX() - 150, GraphicsManager.Instance.getInitialY()));
                snakeBoxes.Add(new BoundingBox());

            }
        }


        public void growSnake(int amount, Vector2 position)
        {

            positions.Insert(positions.Count - 1, position);
            snakeBody.Insert(positions.Count - 2, GraphicsManager.Instance.bodyAsset);
            snakeBoxes.Insert(positions.Count - 1, new BoundingBox());


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
            snakeBody.Add(GraphicsManager.Instance.headAsset);
            snakeBody.Add(GraphicsManager.Instance.tailAsset);
        }


        public void UpdateSnakePositions(KeyboardState kstate, GameTime gameTime, GraphicsDeviceManager graphics, bool doesIntersect)
        {
           
            //snakeBoxes[0] = Collision.UpdateBoundingBox(snakeBoxes[0], GraphicsManager.Instance.headAsset, positions[0]);

            UpdateSnakeBoxes();

            noKeyPressed = true;

            bool allowTail = true;
            bool allowHead = true;

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

            if (noKeyPressed && shrinkMode == false)
                return;


            //dont change any positions if the snake isnt moving 
            else if (!noKeyPressed)
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
                head.X = Math.Min(Math.Max(GraphicsManager.Instance.headAsset.Width / 2, head.X), graphics.PreferredBackBufferWidth - GraphicsManager.Instance.headAsset.Width / 2);
                head.Y = Math.Min(Math.Max(GraphicsManager.Instance.headAsset.Height / 2, head.Y), graphics.PreferredBackBufferHeight - GraphicsManager.Instance.headAsset.Height / 2);

                //check collision, if this move is allowed, store the position - if not, stay where we are!
                if (doesIntersect)
                {
                        snakeSpeed =  snakeSpeed / Variables.slowdown;

                    Console.WriteLine("Speed: " + snakeSpeed);

                    if (up)
                    {
                        head.Y += Variables.collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    }
                    else if (down)
                    {
                        head.Y -= Variables.collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (left)
                    {
                        head.X += Variables.collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (right)
                    {
                        head.X -= Variables.collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }

                if (snakeSpeed < Variables.maxSpeed)
                {
                    snakeSpeed++;
                }


                positions[0] = head;

                // loop through list, set positions for all nodes while incrementing positions
                for (int i = 1; i < positions.Count; i++)
                {

                    //set position
                    Vector2 temp = positions[i];

                    if (previousPositions.Count != 0 && previousPositions.Count > i * Variables.spacing)
                    {
                        temp = previousPositions.ToArray()[i * Variables.spacing];
                    }

                    //store back in list
                    positions[i] = temp;

                }

                //if there is room to grow, and the head has moved enough, increment snake 
                if (previousPositions.Count > Variables.spacing * positions.Count && positions.Count <= Variables.maxLength)
                {
                    Vector2 position = positions[positions.Count - 1];
                    growSnake(1, position);
                    snakeLength++;
                }

            }

            previousKB = kstate;

            }
        }

        private void UpdateSnakeBoxes()
        {
            for (int i = 0; i < snakeBoxes.Count; i++){

                if(i == 0){
                    snakeBoxes[0] = Collision.UpdateBoundingBox(snakeBoxes[0], GraphicsManager.Instance.headAsset, positions[0]);
                } else if(i == snakeBoxes.Count-1){
                    snakeBoxes[snakeBoxes.Count-1] = Collision.UpdateBoundingBox(snakeBoxes[snakeBoxes.Count-1], GraphicsManager.Instance.tailAsset, positions[snakeBoxes.Count-1]);
                } else{
                    snakeBoxes[i] = Collision.UpdateBoundingBox(snakeBoxes[i], GraphicsManager.Instance.bodyAsset, positions[i]);
                }


            }
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


            float maxDistance = 3.5f * GraphicsManager.Instance.bodyAsset.Width;
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


            if (previousPositions.Count >= positions.Count * Variables.spacing)
                previousPositions = previousPositions.GetRange(0, positions.Count * Variables.spacing);

            previousPositions.Reverse();

            for (int i = 0; i < positions.Count; i++)
            {

                //set position
                Vector2 temp = positions[i];

                if (previousPositions.Count != 0 && previousPositions.Count > i * Variables.spacing)
                {
                    temp = previousPositions.ToArray()[i * Variables.spacing];
                }

                //store back in list
                positions[i] = temp;

            }
        }

        public List<Texture2D> getSnakeBody(){
            return snakeBody;
        }

        public List<Vector2> getPositions(){
            return positions;
        }

        public List<BoundingBox> getSnakeBoxes(){
          
            return snakeBoxes;
        }

        public BoundingBox getHeadBox(){
           return snakeBoxes[0];
        }

        public void addToSnakeBody(Texture2D asset){
            snakeBody.Add(asset);
        }


    }


}
