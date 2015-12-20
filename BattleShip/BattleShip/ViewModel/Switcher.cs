using System;
using System.Windows.Controls;

namespace BattleShip.ViewModel
{
    public static class Switcher
    {
        public static Frame CurrentFrame;

        public static void Switch(string newPage)
        {
            CurrentFrame.Source = new Uri($"../../View/{newPage}.xaml", UriKind.Relative);
        }
    }
}
