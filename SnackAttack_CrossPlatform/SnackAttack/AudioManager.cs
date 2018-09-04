using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace SnackAttack.Desktop
{
    public class AudioManager
    {

        public SoundEffect powerUp;
        public SoundEffect powerDown;
        public SoundEffect warp;
        public SoundEffect pop;
        public SoundEffect success;
        public SoundEffect failure;
        public SoundEffect grow;
        public SoundEffect shrink;

        public Song backgroundMusic;


        private static AudioManager instance = null;

        private AudioManager()
        {
        }

        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AudioManager();
                }
                return instance;
            }
        }

        public void LoadMusic(Song _backgroundMusic){
            backgroundMusic = _backgroundMusic;
        }

        public void LoadSoundEffects(SoundEffect _powerUp, SoundEffect _powerDown, 
                                     SoundEffect _warp, SoundEffect _grow, 
                                     SoundEffect _shrink, SoundEffect _success, SoundEffect _failure){
            powerUp = _powerUp;
            powerDown = _powerDown;
            warp = _warp;
            grow = _grow;
            shrink = _shrink;
            success = _success;
            failure = _failure;
        }

        public void playPowerUp(){
            powerUp.Play();
        }

        public void playPowerDown()
        {
            powerDown.Play();
        }

        public void playPowerWarp()
        {
            warp.Play();
        }

        public void playMusic(){
            MediaPlayer.Play(backgroundMusic);
        }

        public void stopMusic(){
            MediaPlayer.Stop();
        }
    }
}
