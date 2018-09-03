using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnackAttack.Desktop
{
    public class GraphicsManager
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public SpriteFont font;

        public Texture2D background;

        public Texture2D pause;
        public Texture2D mouse;
        public Texture2D obstacle;

        public Texture2D headLeft;
        public Texture2D headRight;
        public Texture2D headUp;
        public Texture2D headDown;

        public Texture2D bodyLeft;
        public Texture2D bodyRight;
        public Texture2D bodyUp;
        public Texture2D bodyDown;

        public Texture2D tailLeft;
        public Texture2D tailRight;
        public Texture2D tailUp;
        public Texture2D tailDown;

        public Texture2D mouseLeft;
        public Texture2D mouseRight;
        public Texture2D mouseUp;
        public Texture2D mouseDown;

        public enum Direction { left, right, up, down };
        public enum SnakePart { head, tail, body };

        public Texture2D warp;
        public Texture2D powerUp;
        public Texture2D powerDown;


        public Vector2 pausePos;

        private static GraphicsManager instance = null;

        private GraphicsManager()
        {
        }

        public static GraphicsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GraphicsManager();
                }
                return instance;
            }
        }

        public void setGraphicsDeviceManager(GraphicsDeviceManager _graphics){
            graphics = _graphics;

            if (Variables.fullScreen)
                graphics.ToggleFullScreen();
            else{
                graphics.PreferredBackBufferWidth = Variables.screenWidth;
                graphics.PreferredBackBufferHeight = Variables.screenHeight;
            }
        }

        public void LoadContent(){
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);


        }

        public void loadSnakeHead(Texture2D up, Texture2D down, Texture2D left, Texture2D right)
        {
            //add snake head
            headUp = up;
            headDown = down;
            headLeft = left;
            headRight = right;

            Snake.Instance.addToSnakeBody(up);
        }

        public void loadSnakeTail(Texture2D up, Texture2D down, Texture2D left, Texture2D right)
        {

            tailUp = up;
            tailDown = down;
            tailLeft = left;
            tailRight = right;

            Snake.Instance.addToSnakeBody(tailUp);
        }

        public void loadSnakeBody(Texture2D up, Texture2D down, Texture2D left, Texture2D right)
        {
            bodyUp = up;
            bodyDown = down;
            bodyLeft = left;
            bodyRight = right;

        }

        public void loadMouse(Texture2D up, Texture2D down, Texture2D left, Texture2D right){

            mouseUp = up;
            mouseDown = down;
            mouseLeft = left;
            mouseRight = right;
        }

        public void loadWarp(Texture2D warpHole) {
            warp = warpHole;
        }

        public void loadPowers(Texture2D up, Texture2D down) {
            powerUp = up;
            powerDown = down;
        }

        public GraphicsDeviceManager GetGraphics(){
            return graphics;
        }

        public float getPreferredWidth(){
            return graphics.PreferredBackBufferWidth;
        }

        public float getPreferredHeight()
        {
            return graphics.PreferredBackBufferHeight;
        }

        public void DrawSnake()
        {
            //draw all snake nodes

            List<Vector2> positions = Snake.Instance.getPositions();
            List<Texture2D> snakeBody = Snake.Instance.getSnakeBody();
            List<Direction> directions = Snake.Instance.GetDirections();

            //draw tail
            spriteBatch.
                       Draw(getSnakeTexture(directions[directions.Count - 1], SnakePart.tail), positions[positions.Count - 1], null, Color.White, 0f,
                            new Vector2(snakeBody[snakeBody.Count - 1].Width / 2,
                                        snakeBody[snakeBody.Count - 1].Height / 2), Vector2.One, SpriteEffects.None, 0f);

            //draw body
            for (int i = 1; i < positions.Count - 1; i++)
            {
                spriteBatch.
                           Draw(getSnakeTexture(directions[i], SnakePart.body), positions[i], null, Color.White, 0f,
                     new Vector2(snakeBody[i].Width / 2,
                                 snakeBody[i].Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }

            //draw snake head
            spriteBatch.
                       Draw(getSnakeTexture(directions[0], SnakePart.head), positions[0], null, Color.White, 0f, 
                            new Vector2(snakeBody[0].Width / 2, 
                                        snakeBody[0].Height / 2), Vector2.One, SpriteEffects.None, 0f);





        }

        public void DrawObstacles()
        {

            List<Vector2> positions = Obstacles.Instance.getPositions();
     

            foreach (Vector2 position in positions)
            {

                spriteBatch.
                           Draw(obstacle, position, null, Color.White, 0f,
                new Vector2(obstacle.Width / 2, obstacle.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }


        }

        public void DrawMice()
        {

            Vector2 miceLocation = Mice.Instance.getMiceLocation();
            Texture2D miceBody = GraphicsManager.instance.mouse;

            spriteBatch.Draw(miceBody, miceLocation, null, Color.White, 0f, new Vector2(miceBody.Width / 2, miceBody.Height / 2), 0.7f, SpriteEffects.None, 0f);

        }

        public void DrawWarp() {
            List<Vector2> warpLocation = Mice.Instance.getWarpLocation();
            Texture2D warpBody = GraphicsManager.Instance.warp;
            int warpPoints = Mice.Instance.getMicePoints();
            for (int i = 0; i < warpPoints; i++) {
                spriteBatch.Draw(warpBody, warpLocation[i], null, Color.White, 0f, new Vector2(warpBody.Width / 2, warpBody.Height / 2), 0.7f, SpriteEffects.None, 0f);            
            }
        }


        public void DrawPowers() {

            foreach(Vector2 up in PickUps.Instance.getPowerUpPositions()){
            
                spriteBatch.Draw(powerUp, up, null, Color.White, 0f, new Vector2(powerUp.Width / 2, powerUp.Height / 2), 0.7f, SpriteEffects.None, 0f);
            }

            foreach (Vector2 down in PickUps.Instance.getPowerDownPositions())
            {

                spriteBatch.Draw(powerDown, down, null, Color.White, 0f, new Vector2(powerDown.Width / 2, powerDown.Height / 2), 0.7f, SpriteEffects.None, 0f);
            }
        }

        public void drawStartUI()
        {
            spriteBatch.
                       DrawString(font, Variables.welcomeMessage,
                                   new Vector2(getPreferredWidth() / 2 - 100,
                                               getPreferredHeight() / 2 - 75), Color.White);
        }

        public void drawBackground(){

            spriteBatch.Draw(background, new Vector2(getPreferredWidth()/2, getPreferredHeight()/2), null, Color.White, 0f,
                             new Vector2(getPreferredWidth()/2, getPreferredHeight() / 2), Vector2.One, SpriteEffects.None, 0f);

        }

        public void drawPauseUI()
        {
            spriteBatch.
                           Draw(pause, pausePos, null, Color.White, 0f,
                                new Vector2(pause.Width / 2, pause.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        }

        public void drawGameUI(String timerText)
        {
            spriteBatch.
                       DrawString(font, timerText,
                                   new Vector2(getPreferredWidth() - (11 * getPreferredWidth() / 12),
                                               getPreferredHeight() - (11 * getPreferredHeight() / 12)), Color.White);

            spriteBatch.
                       DrawString(font, "Speed: " + Snake.Instance.getSpeed(),
                                  new Vector2(getPreferredWidth() - (11 * getPreferredWidth() / 12),
                                              getPreferredHeight() - (10 * getPreferredHeight() / 12)), Color.White);

            spriteBatch.
                       DrawString(font, "Length: " + Snake.Instance.getSnakeLength() + "\n\nMax Length: " + Variables.maxLength ,
                                   new Vector2(getPreferredWidth() - (11 * getPreferredWidth() / 12),
                                               getPreferredHeight() - (9 * getPreferredHeight() / 12)), Color.White);
        }

        public void drawTimeUpUI()
        {

            spriteBatch.
                       DrawString(font, Variables.timeUpMessage,
                                   new Vector2(getPreferredWidth() / 2 - 50,
                                               getPreferredHeight() / 2), Color.White);

        }

        public void drawWinUI(TimeSpan timeSpan)
        {

            TimeSpan winTime = new TimeSpan(0, 0, Variables.time);



            Variables.winMessage = "You win! Time: " + ((Variables.time/1000) - timeSpan.Seconds).ToString() + " seconds. Press 'r' to Restart";

            spriteBatch.
                       DrawString(font, Variables.winMessage,
                                  new Vector2(getPreferredWidth() / 2 - 50,
                                               getPreferredHeight() / 2), Color.White);

        }

        public void drawGameActors()
        {

            if (Variables.obstacleMode)
                DrawObstacles();

            DrawWarp();

            DrawPowers();

            DrawSnake();

            DrawMice();
        }

        internal void DrawContent(string timerText, TimeSpan timeSpan)
        {

            spriteBatch.Begin();

            drawBackground();

            if (Game1.gameState == Game1.GameState.Start)
            {
                drawStartUI();
            }

            if (Game1.gameState == Game1.GameState.Playing)
            {
                
                drawGameActors();
                drawGameUI(timerText);
            }

            if (Game1.gameState == Game1.GameState.Paused)
            {
                drawPauseUI();
                drawGameActors();
                drawGameUI(timerText);
            }

            if (Game1.gameState == Game1.GameState.Won)
            {
                drawGameActors();
                drawWinUI(timeSpan);
            }

            if (Game1.gameState == Game1.GameState.TimeUp)
            {
                drawGameActors();
                drawTimeUpUI();
            }

            spriteBatch.End();

        }

        public float getInitialX(){
            return (getPreferredWidth() / 2);
        }

        public float getInitialY(){
            return getPreferredHeight() / 2;
        }

        public Texture2D getSnakeTexture(Direction direction, SnakePart part)
        {
            Texture2D result = null;
            if(part == SnakePart.head){
                if(direction == Direction.up){
                    return headUp;
                }
                else if (direction == Direction.down)
                {
                    return headDown;
                }
                else if (direction == Direction.left)
                {
                    return headLeft;
                }
                else if (direction == Direction.right)
                {
                    return headRight;
                }
            } 
            else if (part == SnakePart.body)
            {
                if (direction == Direction.up)
                {
                    return bodyUp;
                }
                else if (direction == Direction.down)
                {
                    return bodyDown;
                }
                else if (direction == Direction.left)
                {
                    return bodyLeft;
                }
                else if (direction == Direction.right)
                {
                    return bodyRight;
                }
            }
            else if (part == SnakePart.tail)
            {
                if (direction == Direction.up)
                {
                    return tailUp;
                }
                else if (direction == Direction.down)
                {
                    return tailDown;
                }
                else if (direction == Direction.left)
                {
                    return tailLeft;
                }
                else if (direction == Direction.right)
                {
                    return tailRight;
                }
            }

            return null;
        }
    }


}
