using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SnackAttack.Desktop
{
    public class Snake
    {


        List<Vector2> previousPositions; //where the snake head has been
        List<GraphicsManager.Direction> previousDirections; //previous texture directions
        List<Vector2> positions; //the current positions of the snake body nodes
        List<GraphicsManager.Direction> directions;
        List<Texture2D> snakeBody; 

        List<BoundingBox> snakeBoxes; //collision boxes for snake

        KeyboardState previousKB;

        GraphicsManager.Direction direction;
        bool noKeyPressed = true;
        bool tailMoving = false;


        int framesPassedTail = 0; //for animations
        int framesPassedHead = 0;

        float snakeSpeed = Variables.maxSpeed; 
        int snakeLength = Variables.minLength;

        private static Snake instance = null;

        private static readonly Random rnd = new Random();

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

        public void Initialize()
        {

            snakeBody = new List<Texture2D>(Variables.minLength);
            positions = new List<Vector2>(Variables.minLength);
            directions = new List<GraphicsManager.Direction>(Variables.minLength);
            previousPositions = new List<Vector2>(Variables.minLength * Variables.spacing);
            previousDirections = new List<GraphicsManager.Direction>(Variables.minLength * Variables.spacing);
            snakeBoxes = new List<BoundingBox>();

            //initialize all node positions 
            for (int i = 0; i < Variables.minLength; i++)
            {
                positions.Add(new Vector2(GraphicsManager.Instance.getInitialX() - 150, GraphicsManager.Instance.getInitialY()));
                snakeBoxes.Add(new BoundingBox());
                directions.Add(GraphicsManager.Direction.up);
            }

            //insert snake head
            snakeBody.Add(GraphicsManager.Instance.headUp);
            for (int i = 1; i < Variables.minLength - 1; i++)
            {
                snakeBody.Add(GraphicsManager.Instance.bodyUp);
            }
            snakeBody.Add(GraphicsManager.Instance.tailUp);

        }

        //used for colliding with a powerup, 
        //will either add nodes to the snake, or if there is spillover
        //increase the max length
        public void growSnake(int amount, Vector2 position)
        {

            if(positions.Count + amount > Variables.maxLength)
            {
                Variables.maxLength += Variables.powerUpBonus;
                return;
            }


            for (int i = 0; i < amount; i++)
            {

                if (positions.Count < Variables.maxLength)
                {
                    positions.Insert(positions.Count - 1, position);
                    snakeBody.Insert(positions.Count - 2, GraphicsManager.Instance.bodyUp);
                    snakeBoxes.Insert(positions.Count - 1, new BoundingBox());
                    directions.Insert(positions.Count - 1, GraphicsManager.Direction.up);
                    snakeBody[snakeBody.Count - 1] = GraphicsManager.Instance.getSnakeTexture(direction, GraphicsManager.SnakePart.tail);
                } 

            }

        }

        //simply inserts a single node into the snake
        public void growSnake(Vector2 position)
        {

            if (positions.Count < Variables.maxLength)
            {
                positions.Insert(positions.Count - 1, position);
                snakeBody.Insert(positions.Count - 2, GraphicsManager.Instance.bodyUp);
                snakeBoxes.Insert(positions.Count - 1, new BoundingBox());
                directions.Insert(positions.Count - 1, GraphicsManager.Direction.up);
                snakeBody[snakeBody.Count - 1] = GraphicsManager.Instance.getSnakeTexture(direction, GraphicsManager.SnakePart.tail);
            }



        }


        //removes nodes from the snake
        public void shrinkSnake(int amount, GraphicsManager.Direction direction)
        {


            for (int i = 0; i < amount; i++)
            {

                if (positions.Count > 0 && positions.Count > Variables.minLength)
                {
                    positions.RemoveAt(positions.Count - 1);
                    snakeBody.RemoveAt(snakeBody.Count - 1);
                    snakeBoxes.RemoveAt(snakeBoxes.Count - 1);
                    directions.RemoveAt(directions.Count - 1);
                    snakeBody[snakeBody.Count - 1] = GraphicsManager.Instance.getSnakeTexture(direction, GraphicsManager.SnakePart.tail);
                }

            }

        }

        public Vector2 getHeadPosition()
        {
            return positions[0];
        }

        public void UpdateSnakePositions(KeyboardState kstate, GameTime gameTime, GraphicsDeviceManager graphics, bool doesIntersect, bool powerUp, bool powerDown)
        {

            //redraw all collision boxes
            UpdateSnakeBoxes();

            //check pick up collisions
            if (powerUp)
            {
                if(Variables.fxOn && Variables.audioOn)
                    AudioManager.Instance.playPowerUp();
                powerUpSnake();

            }
            if (powerDown)
            {
                if (Variables.fxOn && Variables.audioOn)
                    AudioManager.Instance.playPowerDown();
                powerDownSnake();
            }


            noKeyPressed = true;

            bool allowTail = true;
            bool allowHead = true;
            bool shrinkMode = false;


            if (kstate.IsKeyDown(Keys.LeftShift)){

                shrinkMode = true;
                noKeyPressed = false;

            }
            else if (kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.S) || kstate.IsKeyDown(Keys.A) || kstate.IsKeyDown(Keys.D))
            {

                tailMoving = false;
                noKeyPressed = false;
            }

            if (noKeyPressed == true)
                return;


            if(positions.Count == Variables.maxLength){
                allowHead = false;
                allowTail = false;

            }

            //if player is holding shift, shrink a node every n frames
            if (shrinkMode)
            {

                if (framesPassedTail < Variables.shrinkEveryNFrames)
                {
                    framesPassedTail++;
                    return;
                }
                else
                {

                    framesPassedTail = 0;
                    shrinkSnake(1, direction);
                    if (Variables.audioOn && Variables.fxOn)
                    {
                        AudioManager.Instance.shrink.Play();
                    }

                    return;
                }

            }


            //dont change any positions if the snake isnt moving 
            if (!noKeyPressed)
            {

                if ((tailMoving && allowTail) || (!tailMoving && allowHead))
                {

                    //are we moving the head or the tail now?
                    var head = positions[0];

                    var lastPreviousPosition = head;
                    var lastPreviousDirection = direction;


                    //move head or tail first
                    if (!tailMoving)
                    {

                        //apply movement 
                        if (kstate.IsKeyDown(Keys.W))
                        {
                            head.Y -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            direction = GraphicsManager.Direction.up;
                            noKeyPressed = false;
                        }

                        else if (kstate.IsKeyDown(Keys.S))
                        {
                            head.Y += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            direction = GraphicsManager.Direction.down;
                            noKeyPressed = false;
                        }


                        else if (kstate.IsKeyDown(Keys.A))
                        {
                            head.X -= snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            direction = GraphicsManager.Direction.left;
                        }

                        else if (kstate.IsKeyDown(Keys.D))
                        {
                            head.X += snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            direction = GraphicsManager.Direction.right;
                            noKeyPressed = false;
                        }
                    }

                    //record the head's movement 
                    previousPositions.Insert(0, lastPreviousPosition);
                    previousDirections.Insert(0, lastPreviousDirection);

                    //this stops the snake from moving out of the screen :)
                    head.X = Math.Min(Math.Max(GraphicsManager.Instance.headUp.Width / 2, head.X), graphics.PreferredBackBufferWidth - GraphicsManager.Instance.headUp.Width / 2);
                    head.Y = Math.Min(Math.Max(GraphicsManager.Instance.headUp.Height / 2, head.Y), graphics.PreferredBackBufferHeight - GraphicsManager.Instance.headUp.Height / 2);

                    //check collision, if this move is allowed, store the position - if not, stay where we are!
                    //now that we dont use obstacles anymore this if statement is obsolete 
                    if (doesIntersect)
                    {
                        snakeSpeed = snakeSpeed / Variables.slowdown;

                        Console.WriteLine("Speed: " + snakeSpeed);

                        if (direction == GraphicsManager.Direction.up)
                        {
                            head.Y += Variables.collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        }
                        else if (direction == GraphicsManager.Direction.down)
                        {
                            head.Y -= Variables.collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else if (direction == GraphicsManager.Direction.left)
                        {
                            head.X += Variables.collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else if (direction == GraphicsManager.Direction.right)
                        {
                            head.X -= Variables.collisionModifier * snakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                    }

                    //if the snake has slowed down, it gradually gets back to its max speed
                    if (snakeSpeed < Variables.maxSpeed)
                    {
                        snakeSpeed++;
                    }

                    //save the new head and direction variables
                    positions[0] = head;
                    directions[0] = direction;

                    ////set the correct orientation asset
                    snakeBody[0] = GraphicsManager.Instance.getSnakeTexture(directions[0], GraphicsManager.SnakePart.head);
                    snakeBody[snakeBody.Count - 1] = GraphicsManager.Instance.getSnakeTexture(directions[directions.Count - 1], GraphicsManager.SnakePart.tail);

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


                        //update texture
                        if (previousDirections.Count != 0 && previousDirections.Count > i * Variables.spacing)
                        {


                            if (i != positions.Count - 1)
                                snakeBody[i] = GraphicsManager.Instance.getSnakeTexture(previousDirections.ToArray()[i * Variables.spacing],
                                                                                    GraphicsManager.SnakePart.body);
                            else if (i == positions.Count - 1)
                            {
                                snakeBody[i] = GraphicsManager.Instance.getSnakeTexture(previousDirections.ToArray()[i * Variables.spacing],
                                                                                            GraphicsManager.SnakePart.tail);
                            }

                            directions[i] = previousDirections.ToArray()[i * Variables.spacing];

                        }

                    }

                    //if the snake is not at max length, add a new node every n frames
                    if (framesPassedHead < Variables.growEveryNFrames)
                    {
                        framesPassedHead++;

                    }
                    else
                    {

                        framesPassedHead = 0;
                        if (previousPositions.Count > Variables.spacing * positions.Count && positions.Count < Variables.maxLength)
                        {
                            Vector2 position = positions[positions.Count - 1];
                            growSnake(position);
                            snakeLength++;
                            if (Variables.audioOn && Variables.fxOn)
                            {
                                AudioManager.Instance.grow.Play();
                            }
                        }


                    }


                }

                //save the keyboard state so we can tell if keys can being held down etc
                previousKB = kstate;

            }
        }

        //no longer using this method 
        public bool checkSnakeCollisions()
        {
            //if the head or tail touch a body node, delete it
            if (positions.Count == 0 || positions == null) return false;

            var head = positions[0];
            var tail = positions[positions.Count - 1];

            for (int i = 1; i < positions.Count - 1; i++)
            {
                var body = positions[i];

                if (Vector2.Distance(tail, body) < (2 * Variables.spacing / 3))
                {

                    Console.WriteLine("Snake collided with itself at " + body.X + "," + body.Y);
                    Console.WriteLine("Positions:" + positions.Count + ", Max Length:" + Variables.maxLength);

                    // shrinkSnake();
                    return true;

                }
            }

            return false;
        }

        //redraws the collision boxes for all snake nodes based on their textures
        private void UpdateSnakeBoxes()
        {
            for (int i = 0; i < snakeBoxes.Count; i++)
            {

                if (i == 0)
                {
                    snakeBoxes[0] = Collision.UpdateBoundingBox(snakeBoxes[0], GraphicsManager.Instance.headUp, positions[0]);
                }
                else if (i == snakeBoxes.Count - 1)
                {
                    snakeBoxes[snakeBoxes.Count - 1] = Collision.UpdateBoundingBox(snakeBoxes[snakeBoxes.Count - 1], GraphicsManager.Instance.tailUp, positions[snakeBoxes.Count - 1]);
                }
                else
                {
                    snakeBoxes[i] = Collision.UpdateBoundingBox(snakeBoxes[i], GraphicsManager.Instance.bodyUp, positions[i]);
                }


            }
        }

        public float getSpeed()
        {
            return snakeSpeed;
        }


        //this is a physical length between the head and tail node
        public float getSnakeDistance()
        {
            return Vector2.Distance(positions[0], positions[positions.Count - 1]);
        }

        //this is a distance in terms of nodes
        public int getSnakeLength()
        {
            return positions.Count;
        }


        //this measures the snake's physical distance. we no longer use this method
        public bool checkDistance(Vector2 head, Vector2 tail)
        {


            float maxDistance = 3.5f * GraphicsManager.Instance.bodyUp.Width;
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

        public float getDistance()
        {


            return Vector2.Distance(positions[0], positions[positions.Count - 1]);
        }

        //this was for when we enabled bi-directional movement and flipped the snake 
        //simply reverses the previous positions and throws away extra indexes
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

        private void repopulatePreviousDirections()
        {
            if (previousDirections.Count >= snakeBody.Count * Variables.spacing)
                previousDirections = previousDirections.GetRange(0, snakeBody.Count * Variables.spacing);

            previousDirections.Reverse();

            for (int i = 0; i < snakeBody.Count; i++)
            {

                //set position
                Texture2D temp = snakeBody[i];



                if (previousDirections.Count != 0 && previousDirections.Count > i * Variables.spacing)
                {

                    if (i == snakeBody.Count - 1)
                    {
                        temp = GraphicsManager.Instance.getSnakeTexture(previousDirections.ToArray()[i * Variables.spacing],
                                                                        GraphicsManager.SnakePart.tail);
                    }
                    else
                    {
                        temp = GraphicsManager.Instance.getSnakeTexture(previousDirections.ToArray()[i * Variables.spacing],
                                                                        GraphicsManager.SnakePart.body);
                    }



                }

                //store back in list
                snakeBody[i] = temp;

            }
        }


        public void powerUpSnake()
        {

            if (positions.Count > 0 && positions != null)
            {
                Vector2 position = positions[positions.Count - 1];
                growSnake(Variables.powerUpModifier, position);
            }

        }

        public void powerDownSnake()
        {

            if (positions.Count > 0 && positions != null)
            {
                Vector2 position = positions[positions.Count - 1];
                shrinkSnake(Variables.powerDownModifier, GraphicsManager.Direction.up);
            }
        }

        public List<Texture2D> getSnakeBody()
        {
            return snakeBody;
        }

        public List<Vector2> getPositions()
        {
            return positions;
        }

        public List<BoundingBox> getSnakeBoxes()
        {

            return snakeBoxes;
        }

        public BoundingBox getHeadBox()
        {
            return snakeBoxes[0];
        }

        public void addToSnakeBody(Texture2D asset)
        {
            snakeBody.Add(asset);
        }

        public List<GraphicsManager.Direction> GetDirections()
        {
            return directions;
        }
    }
}
