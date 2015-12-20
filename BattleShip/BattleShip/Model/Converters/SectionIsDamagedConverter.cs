using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BattleShip.Model.Cached;

namespace BattleShip.Model.Converters
{
    [ValueConversion(typeof(bool), typeof(ImageSource))]
    internal class SectionIsDamagedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var mediaFactory = MediaFactory.Init;

                var isDamaged = System.Convert.ToBoolean(value);
                return isDamaged
                    ? mediaFactory.GetImage("attacks/attackResult_Hit")
                    : mediaFactory.GetImage("attacks/attackResult_Unknown");
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
