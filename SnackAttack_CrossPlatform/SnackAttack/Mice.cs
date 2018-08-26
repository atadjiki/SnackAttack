using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SnackAttack.Desktop
{
    class Mice
    {
        float miceSpeed;
        int collisionModifier = 1;
        float backWidth;
        float backHeight;
        Texture2D miceBody;
        public Vector2 miceLocation;
        BoundingBox miceBox = new BoundingBox();
        private Vector2 snakeHeadLocation;
        private List<Vector2> micePointLocations;
        private int totalMicePoints;
        private float xOffset = 50f;
        private float yOffset = 50f;
        private Vector2 gotoLocation;
        private int index = 2;


        public Mice(float initialX, float initialY)
        {
            backWidth = initialX;
            backHeight = initialY;
            miceSpeed = 120f;
            totalMicePoints = 4;
            
            micePointLocations = new List<Vector2>(totalMicePoints);

            micePointLocations.Add(new Vector2(backWidth - xOffset, backHeight - yOffset));
            micePointLocations.Add(new Vector2(0f + xOffset, backHeight - yOffset));
            micePointLocations.Add(new Vector2(0f + xOffset, 0f + yOffset));
            micePointLocations.Add(new Vector2(backWidth - xOffset, 0f + yOffset));

            miceLocation = micePointLocations[0];
            gotoLocation = micePointLocations[1];
        }


        public void loadMice(Texture2D head)
        {
            miceBody = head;
        }

        public void UpdateMicePosition(GameTime gameTime, GraphicsDeviceManager graphics, Vector2 headPosition)
        {
            UpdateBoundingBox();
            float x_direction;
            float y_direction;

            //getting the direction of mice
            

            if (miceLocation.X == gotoLocation.X && miceLocation.Y == gotoLocation.Y)
            {
                index = index % 4;
                gotoLocation = micePointLocations[index];
                index += 1;

            }
            else
            {
                if (miceLocation.X == gotoLocation.X)
                {
                    x_direction = 0f;
                }
                else if (miceLocation.X > gotoLocation.X)
                {
                    x_direction = -1f;
                }
                else
                {
                    x_direction = 1f;
                }

                if (miceLocation.Y == gotoLocation.Y)
                {
                    y_direction = 0f;
                }
                else if (miceLocation.Y > gotoLocation.Y)
                {
                    y_direction = -1f;
                }
                else
                {
                    y_direction = 1f;
                }
                miceLocation.X += (float)Math.Round((miceSpeed * x_direction) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                miceLocation.Y += (float)Math.Round((miceSpeed * y_direction) * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public void DrawMice(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(miceBody, this.miceLocation, null, Color.White, 0f, new Vector2(miceBody.Width / 2, miceBody.Height / 2), 0.7f, SpriteEffects.None, 0f);
        }

        protected void UpdateBoundingBox()
        {
            //Vector2 location = this.miceLocation;
            //this.miceBox.Min.X = location.X;
            //this.miceBox.Min.Y = location.Y;
            //this.miceBox.Max.X = location.X + miceBody.Width;
            //this.miceBox.Max.Y = location.Y + miceBody.Height;

        }
    }

}
