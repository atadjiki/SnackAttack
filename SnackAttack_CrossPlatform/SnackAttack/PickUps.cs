using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnackAttack.Desktop
{
    /*
     * A class for initializing and managing the powerups that spawn on the screen.
     * Powerups are randomly generated and placed. 
     * Can be controlled through the Variables class. 
     */ 
    public class PickUps
    {

        List<Vector2> powerUpPositions;
        List<Vector2> powerDownPositions;

        List<BoundingBox> powerUpBoxes;
        List<BoundingBox> powerDownBoxes;

        private static PickUps instance = null;

        private PickUps()
        {
            Initialize();
        }

        public static PickUps Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PickUps();
                }
                return instance;
            }
        }

        public void Initialize()
        {
            powerUpPositions = new List<Vector2>();
            powerDownPositions = new List<Vector2>();

            powerUpBoxes = new List<BoundingBox>();
            powerDownBoxes = new List<BoundingBox>();

            InitializePickUps();
        }

        public void InitializePickUps()
        {

            //add pickups here

            Random rnd = new Random();

            for (int i = 0; i < Variables.powerUpAmount; i++){

                bool validLocation = false;

                //check that the snake isnt already at this location
                while (!validLocation){
                    Vector2 powerUpLocation = new Vector2();

                    powerUpLocation.X = (float)rnd.Next(50, Variables.screenWidth - 50);
                    powerUpLocation.Y = (float)rnd.Next(50, Variables.screenHeight - 50);

                    if (Vector2.Distance(powerUpLocation, Snake.Instance.getHeadPosition()) > 50)
                    {
                        powerUpPositions.Add(powerUpLocation);
                        validLocation = true;
                    }
                }      
            }

            for (int i = 0; i < Variables.powerDownAmount; i++)
            {

                bool validLocation = false;

                while (!validLocation)
                {
                    Vector2 powerDownLocation = new Vector2();

                    powerDownLocation.X = (float)rnd.Next(50, Variables.screenWidth - 50);
                    powerDownLocation.Y = (float)rnd.Next(50, Variables.screenHeight - 50);

                    if (Vector2.Distance(powerDownLocation, Snake.Instance.getHeadPosition()) > 50)
                    {
                        powerDownPositions.Add(powerDownLocation);
                        validLocation = true;
                    }
                }
            }


            //dont modify this
            for (int i = 0; i < powerUpPositions.Count; i++)
                powerUpBoxes.Add(new BoundingBox());

            for (int i = 0; i < powerDownPositions.Count; i++)
                powerDownBoxes.Add(new BoundingBox());

            Console.WriteLine("Counting " + powerUpPositions.Count + " power-ups.");
            Console.WriteLine("Counting " + powerDownPositions.Count + " power-downs.");


        }

        //draw new collision boxes for all the pickups
        // at the moment they dont move, but if they do we would need this

        public void UpdatePickUpBoxes(Texture2D powerUp, Texture2D powerDown)
        {
            //powerups
            for (int i = 0; i < powerUpPositions.Count; i++)
            {

                powerUpBoxes[i] = Collision.UpdateBoundingBox(powerUpBoxes[i], powerUp, powerUpPositions[i]);
            }

            //powerdowns
            for (int i = 0; i < powerDownPositions.Count; i++)
            {

                powerDownBoxes[i] = Collision.UpdateBoundingBox(powerDownBoxes[i], powerDown, powerDownPositions[i]);
            }

        }

        //check to see if the snake has collided with the powerup, if so, remove it from the game
        public bool checkPowerUpCollision()
        {
            int result = Collision.CheckPickUpCollisions(Snake.Instance.getSnakeBoxes(), powerUpPositions, powerUpBoxes);

            if(result < 0){
                return false;
            } else{
                //delete the collided with power up and report collision
                powerUpPositions.RemoveAt(result);
                powerUpBoxes.RemoveAt(result);
                return true;
            }
        }

        public bool checkPowerDownCollision()
        {
            int result = Collision.CheckPickUpCollisions(Snake.Instance.getSnakeBoxes(), powerDownPositions, powerDownBoxes);

            if (result < 0)
            {
                return false;
            }
            else
            {
                //delete the collided with power up and report collision
                powerDownPositions.RemoveAt(result);
                powerDownBoxes.RemoveAt(result);
                return true;
            }
        }

        //if all powerups are gone, and the bool is true, respawn them
        public void checkReplenishPickUps(){

            if(Variables.pickUpsRespawn){
                if(powerUpPositions.Count == 0 && powerDownPositions.Count == 0)
                    InitializePickUps();
            }
        }

        public List<Vector2> getPowerUpPositions()
        {
            return powerUpPositions;
        }

        public List<BoundingBox> getPowerUpBoxes()
        {
            return powerUpBoxes;
        }

        public List<Vector2> getPowerDownPositions()
        {
            return powerDownPositions;
        }

        public List<BoundingBox> getPowerDownBoxes()
        {
            return powerDownBoxes;
        }

    }


}