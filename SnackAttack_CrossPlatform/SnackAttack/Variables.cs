using System;
using Microsoft.Xna.Framework;

namespace SnackAttack.Desktop
{
    public class Variables
    {

        public static Color backgroundColor = Color.CornflowerBlue;

        public static bool DebugMode = true;

        public static string backgroundimage = "background";

        //note - make sure all fx are in .ogg or .mp3 format
        public static string backgroundMusic = "backgroundmusic";
        public static string warpFX = "warpFX";
        public static string powerUpFX = "powerUpFX";
        public static string powerDownFX = "powerDownFX";

        public static string welcomeMessage = "Welcome to Snake! \n\n Press Enter to Begin \n\n Controls: \n r - restart \n \n wasd - control head \n arrow keys - control tail";
        public static string timeUpMessage = "Time up! Press 'r' to Restart";
        public static string winMessage;

        //note - currently all textures are sized at 64x64 pixels

        public static string snakeHeadLeft = "Snake_head_left";
        public static string snakeHeadRight = "Snake_head_right";
        public static string snakeHeadUp = "Snake_head_up";
        public static string snakeHeadDown = "Snake_head_down";

        public static string snakeTongueUp = "snake_head_tongue_up";
        public static string snakeTongueDown = "snake_head_tongue_down";
        public static string snakeTongueLeft = "snake_head_tongue_left";
        public static string snakeTongueRight = "snake_head_tongue_right";

        public static string snakeBodyLeft = "Snake_body_left";
        public static string snakeBodyRight = "Snake_body_right";
        public static string snakeBodyUp = "Snake_body_up";
        public static string snakeBodyDown = "Snake_body_down"; 

        public static string snakeTailLeft = "Snake_tail_left";
        public static string snakeTailRight = "Snake_tail_right";
        public static string snakeTailUp = "Snake_tail_up";
        public static string snakeTailDown = "Snake_tail_down";

        public static string mouseLeft = "Mice_left";
        public static string mouseRight = "Mice_right";
        public static string mouseUp = "Mice_up";
        public static string mouseDown = "Mice_down";

        public static string obstacle = "brick";
        public static string pause = "pause";
        public static string timer = "Timer";

        public static string warp = "Warp";

        public static string powerUp = "good_power_up";
        public static string powerDown = "Bad_power_up";


        //game constants
        public static bool obstacleMode = false;
        public static int time = 31000; 


        //powerup constants
        public static bool pickUpsMode = true;
        public static bool pickUpsRespawn = true;
        public static int powerUpModifier = 10; //how much we will increase the snake
        public static int powerDownModifier = 10; //how much the snake will shrink if it can 
        public static int powerUpAmount = 3; //the number of power ups created
        public static int powerUpBonus = 5;
        public static int powerDownAmount = 4; //the number of power down objects created

        //snake constants
        public static float maxSpeed = 100f;
        public static int maxLength = 16;
        public static int minLength = 4;
        public static int spacing = 25; //track every nth head positions (0 will look really mushed)
        public static int collisionModifier = 50;
        public static int slowdown = 5;

        //animation constants
        public static int shrinkEveryNFrames = 15;
        public static int growEveryNFrames = 45;
        public static int tongueEveryNFrames = 45;

        //game screen variables
        public static bool fullScreen = false;
        public static int screenWidth = 800; //1920  800 1440 
        public static int screenHeight = 480; //1080 480 900

        public static bool audioOn = false;
        public static bool fxOn = true;
        public static bool musicOn = true;

    }
}
