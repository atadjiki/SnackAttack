using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnackAttack.Desktop
{
    public static class Collision
    {

        //returns true if the two bounding boxes' rectangles intersect
        public static bool doesIntersect(BoundingBox a, BoundingBox b)
        {

            bool intersect = a.Intersects(b);
            if (intersect)
                Console.WriteLine("Intersection at: " + a.Max + "," + b.Max);
            return intersect;

        }

        //draws a new box based on the size and position 
        public static BoundingBox UpdateBoundingBox(BoundingBox box, Texture2D texture, Vector2 pos)
        {
            box.Min.X = pos.X;
            box.Min.Y = pos.Y;
            box.Max.X = pos.X + texture.Width;
            box.Max.Y = pos.Y + texture.Height;

            return box;
        }

        //checks to see if box collides with any of boxesB at positions vectorsB
        public static bool CheckCollisions(BoundingBox box, List<Vector2> vectorsB, List<BoundingBox> boxesB)
        {
            bool collision = false;

            for (int i = 0; i < vectorsB.Count; i++)
            {
                if (doesIntersect(box, boxesB[i]))
                {
                    collision = true;
                    break;
                }
            }

            return collision;
        }

        //like the previous method but checks two lists with each other
        public static bool CheckCollisions(List<BoundingBox> boxesA, List<Vector2> vectorsB, List<BoundingBox> boxesB)
        {
            bool collision = false;

            for (int i = 0; i < vectorsB.Count; i++)
            {
                foreach(BoundingBox box in boxesA){

                    if (doesIntersect(box, boxesB[i]))
                    {
                        collision = true;
                        break;
                    }
                }

            }

            return collision;
        }

        public static int CheckPickUpCollisions(List<BoundingBox> boxesA, List<Vector2> vectorsB, List<BoundingBox> boxesB)
        {
            int collidingIndex = -1;

            for (int i = 0; i < vectorsB.Count; i++)
            {
                foreach (BoundingBox box in boxesA)
                {

                    if (doesIntersect(box, boxesB[i]))
                    {

                        return i;
                    }
                }

            }

            return collidingIndex;
        }

        public static bool CheckMouseSnakeCollisions()
        {

            List<Vector2> snakeBody = Snake.Instance.getPositions();
            if (snakeBody.Count == 0) return false;
            snakeBody.RemoveAt(0);
            snakeBody.RemoveAt(snakeBody.Count - 1);

            List<BoundingBox> snakeBoxes = Snake.Instance.getSnakeBoxes();
            if (snakeBoxes.Count == 0) return false;
            snakeBoxes.RemoveAt(0);
            snakeBoxes.RemoveAt(snakeBoxes.Count - 1);


            return CheckCollisions(Mice.Instance.mouseBox, snakeBody, snakeBoxes);
        }
    }

}
