using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BattleShip.Model.Cached;
using BattleShip.Properties;
using BattleShip.View;
using GameEngine.Attacks;

namespace BattleShip.Model.Converters
{
    [ValueConversion(typeof(AttackResult), typeof(ImageSource))]
    internal class AttackResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var mediaFactory = MediaFactory.Init;

                GameWindow.HumanAttackMade = !GameWindow.HumanAttackMade;

                var attackResult = (AttackResult) value;
                
                var playAttackSound = !GameWindow.HumanAttackMade;

                if (!GameWindow.GameStarted || !Settings.Default.AttacksSounds)
                    playAttackSound = false;

                switch (attackResult)
                {
                    case AttackResult.Miss:
                        if (playAttackSound)
                            mediaFactory.PlayMedia("miss");

                        return mediaFactory.GetImage("attacks/attackResult_Miss");

                    case AttackResult.Hit:
                        if (playAttackSound)
                            mediaFactory.PlayMedia("hit");

                        return mediaFactory.GetImage("attacks/attackResult_Hit");

                    // case AttackResult.Unknown:
                    default:
                        if (playAttackSound)
                            mediaFactory.PlayMedia("forbidden");

                        return mediaFactory.GetImage("attacks/attackResult_Unknown");
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
