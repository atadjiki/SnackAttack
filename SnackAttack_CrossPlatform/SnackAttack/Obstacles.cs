﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnackAttack.Desktop
{

    /*
     * This class is no longer in use, but manages and initializes obstacles
     * that are placed in the game world. 
     * If obstacles is turned on in Variables, this will create the decided obstacles
     * and update their collision boxes. The snake cannot cross these but the mouse can. 
     * 
     */
    public class Obstacles
    {

        List<Vector2> obstacles;
        List<BoundingBox> obstacleBoxes;

        private static Obstacles instance = null;

        private Obstacles()
        {
            Initialize();
        }

        public static Obstacles Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Obstacles();
                }
                return instance;
            }
        }

        public void Initialize(){
            obstacles = new List<Vector2>();
            obstacleBoxes = new List<BoundingBox>();
            InitializeObstacles();
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

        public void UpdateObstacleBoxes(Texture2D obstacle)
        {
            for (int i = 0; i < obstacles.Count; i++)
            {

                obstacleBoxes[i] = Collision.UpdateBoundingBox(obstacleBoxes[i], obstacle, obstacles[i]);
            }

        }

        public bool checkCollision(){
            return Collision.CheckCollisions(Snake.Instance.getSnakeBoxes(), obstacles, obstacleBoxes);
        }

        public List<Vector2> getPositions(){
            return obstacles;
        }

        public List<BoundingBox> getObstacleBoxes(){
            return obstacleBoxes;
        }

    }
}
