using System;
using System.Windows.Media;
using BattleShip.Properties;

namespace BattleShip.Model.Preferences
{
    public static class AppSettings
    {
        private static readonly MediaPlayer BgSound;

        public static bool GameSuccessful { get; set; }

        static AppSettings()
        {
            BgSound = new MediaPlayer();
            BgSound.Open(new Uri(@"..\..\Resources\audio\background.mp3", UriKind.Relative));
        }

        public static void BackgroundMusic()
        {
            if (Settings.Default.BackgroundMusic)
                ActivateBackgroundMusic();
            else
                DeactivateBackgroundMusic();
        }

        private static void ActivateBackgroundMusic()
        {
            BgSound.Play();
            BgSound.MediaEnded += MediaEnded;
        }

        private static void MediaEnded(object sender, EventArgs e)
        {
            if (Settings.Default.BackgroundMusic)
            {
                BgSound.Position = new TimeSpan(0, 0, 0);
                BgSound.Play();
            }
        }

        private static void DeactivateBackgroundMusic()
        {
            BgSound?.Pause();
        }
    }
}
