using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnackAttack.Desktop
{
    public static class Collision
    {
        public static bool doesIntersect(BoundingBox a, BoundingBox b)
        {

            bool intersect = a.Intersects(b);
            if (intersect)
                Console.WriteLine("Intersection at: " + a.Max + "," + b.Max);
            return intersect;

        }

        public static BoundingBox UpdateBoundingBox(BoundingBox box, Texture2D texture, Vector2 pos)
        {
            box.Min.X = pos.X;
            box.Min.Y = pos.Y;
            box.Max.X = pos.X + texture.Width;
            box.Max.Y = pos.Y + texture.Height;

            return box;
        }

        public static bool CheckCollisions(BoundingBox headBox, List<Vector2> vectors, List<BoundingBox> boxes)
        {
            bool collision = false;

            for (int i = 0; i < vectors.Count; i++)
            {
                if (doesIntersect(headBox, boxes[i]))
                {
                    collision = true;
                    break;
                }
            }

            return collision;
        }
    }

}
