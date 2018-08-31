using System;
using Microsoft.Xna.Framework;

namespace SnackAttack.Desktop
{
    public class Variables
    {

        public static Color backgroundColor = Color.CornflowerBlue;

        public static string backgroundimage = "background";

        public static string welcomeMessage = "Welcome to Snake! \n\n Press Enter to Begin \n\n Controls: \n r - restart \n \n wasd - control head \n arrow keys - control tail";
        public static string timeUpMessage = "Time up! Press 'r' to Restart";
        public static string winMessage;

        public static string snakeHeadLeft = "Snake_head_left";
        public static string snakeHeadRight = "Snake_head_right";
        public static string snakeHeadUp = "Snake_head_up";
        public static string snakeHeadDown = "Snake_head_down";

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


        //game constants
        public static bool obstacleMode = false;
        public static int time = 31000;

        //snake constants
        public static float maxSpeed = 100f;
        public static int maxLength = 16;
        public static int minLength = 4;
        public static int spacing = 25; //track every nth head positions (0 will look really mushed)
        public static int collisionModifier = 50;
        public static int slowdown = 5;


        //game screen variables
        public static bool fullScreen = false;
        public static int screenWidth = 800;
        public static int screenHeight = 480;


    }
}
