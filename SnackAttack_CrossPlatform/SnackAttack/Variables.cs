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

        public static bool obstacleMode = false;

        public static int time = 31000;
    }
}
