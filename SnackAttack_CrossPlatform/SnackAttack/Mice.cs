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
    public class Mice
    {
        float miceSpeed;
        //int collisionModifier = 1;
        float backWidth;
        float backHeight;
        Texture2D miceBody;
        public Vector2 miceLocation;
        private List<Vector2> micePointLocations;
        private int totalMicePoints;
        private float xOffset = 50f;
        private float yOffset = 50f;
        private Vector2 gotoLocation;
        private int index = 1;
        private int previousIndex;

        public Vector2 mousePos;
        public BoundingBox mouseBox;

        private static Mice instance = null;

        private Mice()
        {
            Initialize();
        }

        public static Mice Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Mice();
                }
                return instance;
            }
        }

        public void Initialize(){

            backWidth = GraphicsManager.Instance.getPreferredWidth();
            backHeight = GraphicsManager.Instance.getPreferredHeight();
            miceSpeed = 210f;
            totalMicePoints = 4;

            micePointLocations = new List<Vector2>(totalMicePoints);

            micePointLocations.Add(new Vector2(backWidth - xOffset, backHeight - yOffset));
            micePointLocations.Add(new Vector2(0f + xOffset, backHeight - yOffset));
            micePointLocations.Add(new Vector2(0f + xOffset, 0f + yOffset));
            micePointLocations.Add(new Vector2(backWidth - xOffset, 0f + yOffset));

            miceLocation = micePointLocations[0];
            gotoLocation = micePointLocations[1];
            previousIndex = 1;

            mouseBox = new BoundingBox();

            mousePos = new Vector2(GraphicsManager.Instance.getInitialX() - 150, GraphicsManager.Instance.getInitialY() - 150);
        }


        public Texture2D loadMice(Texture2D head)
        {
            miceBody = head;
            return miceBody;
        }

    /// <summary>
    /// updates the mice position after every tick of game 
    /// calculates where the mice needs to go after it reaches its location
    /// </summary>
    /// <param name = "gameTime"> which is snapshot of timing values
    /// <param name = "headPosition"> gives the head position of the snake to run away from it
        public Vector2 UpdateMicePosition(GameTime gameTime, bool snakeMouseCollision)
        {

            if(snakeMouseCollision)
                return new Vector2(GraphicsManager.Instance.getInitialX(), GraphicsManager.Instance.getInitialY());

            float x_direction;
            float y_direction;
            int xPercent;
            int yPercent;
            //getting the direction of mice

            previousIndex = index;
            if (miceLocation.X == gotoLocation.X && miceLocation.Y == gotoLocation.Y)
            {
                
                
                xPercent = (int)((Snake.Instance.getHeadPosition().X * 100) / backWidth);
                yPercent = (int)((Snake.Instance.getHeadPosition().Y * 100) / backHeight);
                int difference = xPercent - yPercent;
                Console.WriteLine("xPercent: " + xPercent + "YPercent: " + yPercent + " difference: " + difference + " Previous Index: " + previousIndex);
                if (xPercent >= 50) {
                    if (yPercent >= 50) {
                        if (difference >= 0) {
                            index = 2;
                            if (previousIndex == 0) {
                                index = 1;
                            }
                        } else {
                            if (previousIndex == 0) {
                                index = 3;
                            }
                        }
                    } else {
                        index = 1;
                        difference -= 50;
                        if (difference >= 0) {
                            if (previousIndex == 3) {
                                index = 0;
                            }
                        } else {
                            if (previousIndex == 3) {
                                index = 2;
                            }
                        }
                    }
                } else {
                    if (yPercent >= 50) {
                        index = 3;
                        difference += 50;
                        if (difference >= 0) {
                            if (previousIndex == 1) {
                                index = 2;
                            }
                        } else {
                            if (previousIndex == 1) {
                                index = 0;
                            }
                        }
                    } else {
                        index = 0;
                        if (difference >= 0) {
                            if (previousIndex == 2) {
                                index = 1;
                            } else if(index == 1) {
                                index = 2;
                            }
                        } else {
                            if (previousIndex == 2) {
                                index = 3;
                            }
                        }
                    }
                }

                gotoLocation = micePointLocations[index];

            }
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
                // Console.WriteLine(miceLocation.X + ":::::::" + miceLocation.Y);

            return miceLocation;
        }

        public Vector2 getMiceLocation(){
            return miceLocation;
        }

        public Texture2D getMiceBody(){
            return miceBody;
        }

    }

}
