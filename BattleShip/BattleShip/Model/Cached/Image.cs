using System;
using System.Windows.Media.Imaging;

namespace BattleShip.Model.Cached
{
    public class Image
    {
        public BitmapImage Source { get; }

        public Image(string imagePath)
        {
            Source = new BitmapImage(new Uri($"../../Resources/images/{imagePath}.png", UriKind.Relative));
        }
    }
}
