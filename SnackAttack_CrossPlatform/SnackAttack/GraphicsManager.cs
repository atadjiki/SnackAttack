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

        public Texture2D pause;
        public Texture2D mouse;
        public Texture2D obstacle;
        public Texture2D headAsset;
        public Texture2D bodyAsset;
        public Texture2D tailAsset;


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
        }

        public void LoadContent(){
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);


        }

        public void loadSnake(Texture2D head, Texture2D body, Texture2D tail)
        {
            //add snake head

            headAsset = head;
            Snake.Instance.addToSnakeBody(headAsset);
            bodyAsset = body;

            //add snake tail
            tailAsset = tail;
            Snake.Instance.addToSnakeBody(tailAsset);
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

            for (int i = 0; i < positions.Count; i++)
            {

                spriteBatch.
                Draw(snakeBody[i], positions[i], null, Color.White, 0f, new Vector2(snakeBody[i].Width / 2, snakeBody[i].Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }

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
            Texture2D miceBody = Mice.Instance.getMiceBody();

            spriteBatch.Draw(miceBody, miceLocation, null, Color.White, 0f, new Vector2(miceBody.Width / 2, miceBody.Height / 2), 0.7f, SpriteEffects.None, 0f);

        }

        public void drawStartUI()
        {



            spriteBatch.
                       DrawString(font, Variables.welcomeMessage,
                                   new Vector2(getPreferredWidth() / 2 - 100,
                                               getPreferredHeight() / 2 - 75), Color.White);
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
                       DrawString(font, "Length: " + Snake.Instance.getSnakeDistance(),
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



            Variables.winMessage = "You win! Time: " + (winTime.Seconds - timeSpan.Seconds).ToString() + " seconds. Press 'r' to Restart";

            spriteBatch.
                       DrawString(font, Variables.winMessage,
                                  new Vector2(getPreferredWidth() / 2 - 50,
                                               getPreferredHeight() / 2), Color.White);

        }

        public void drawGameActors()
        {

            if (Variables.obstacleMode)
                DrawObstacles();

                DrawSnake();

                DrawMice();


        }

        internal void DrawContent(string timerText, TimeSpan timeSpan)
        {

            spriteBatch.Begin();

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
    }


}
