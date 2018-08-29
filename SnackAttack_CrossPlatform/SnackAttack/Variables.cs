using System;
using Microsoft.Xna.Framework;

namespace SnackAttack.Desktop
{
    public class Variables
    {

        public static Color backgroundColor = Color.CornflowerBlue;


        public static string welcomeMessage = "Welcome to Snake! \n\n Press Enter to Begin \n\n Controls: \n r - restart \n shift - shrink \n wasd - control head \n arrow keys - control tail";
        public static string timeUpMessage = "Time up! Press 'r' to Restart";
        public static string winMessage;

        //content names
        public static string snakeHead = "blueball";
        public static string snakeBody = "redball";
        public static string snakeTail = "greenball";

        public static string obstacle = "brick";

        public static string pause = "pause";
        public static string timer = "Timer";

        public static string mouse = "mouse";






        public static bool obstacleMode = false;

        public static int time = 31000;
    }
}
