using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnackAttack.Desktop
{
    public class Obstacles
    {
        Texture2D obstacle;
        List<Vector2> obstacles;
        List<BoundingBox> obstacleBoxes;

        public Obstacles(Texture2D texture){
            obstacle = texture;
            obstacles = new List<Vector2>();
            obstacleBoxes = new List<BoundingBox>();
            InitializeObstacles();
            
        }

        public void DrawObstacles(SpriteBatch spriteBatch)
        {


            foreach (Vector2 position in obstacles)
            {

                spriteBatch.
                           Draw(obstacle, position, null, Color.White, 0f,
                new Vector2(obstacle.Width / 2, obstacle.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }


        }

        public void InitializeObstacles()
        {


            //add obstacles here
            for (int i = 0; i < 6; i++)
                obstacles.Add(new Vector2(600, 50 + (i * 50)));

            for (int i = 0; i < 3; i++)
                obstacles.Add(new Vector2(200 + (i * 50), 50));

            for (int i = 0; i < 3; i++)
                obstacles.Add(new Vector2(200 + (i * 50), 350));

            //dont modify this
            for (int i = 0; i < obstacles.Count; i++)
                obstacleBoxes.Add(new BoundingBox());
        }

        public void UpdateObstacleBoxes()
        {
            for (int i = 0; i < obstacles.Count; i++)
            {

                obstacleBoxes[i] = Collision.UpdateBoundingBox(obstacleBoxes[i], obstacle, obstacles[i]);
            }

        }

        public bool checkCollision(BoundingBox headBox){
            return Collision.CheckCollisions(headBox, obstacles, obstacleBoxes);
        }

       



    }
}
