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

        public void LoadSoundEffects(SoundEffect _powerUp, SoundEffect _powerDown, SoundEffect _warp){
            powerUp = _powerUp;
            powerDown = _powerDown;
            warp = _warp;
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
